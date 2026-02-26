# Async Processing POC: MSMQ-style flow on Azure Service Bus (.NET 8 microservices)

## Objective
Build a small, end-to-end POC that mirrors the documented flow:

- **In Queue** → **Listener** routes to **Process** or **Wait**
- **Process Queue** → **Processor** calls a downstream service (mock) and completes or sends to **Error**
- **Error Queue** → blocked sessions remain blocked until an admin unblocks/replays
- **Wait Queue** → **Wait Timer** periodically re-checks and moves to **Process** when safe
- **Session / ordering** → messages for the same membership/session id must not overtake each other

This POC is intentionally simple and focuses on understanding the full async flow.

---

## What you’ll build
Folder (suggested): `BUPA/poc/async-servicebus-poc/`

- `Producer.Api` (HTTP API): enqueues messages to **In Queue**
- `Listener.Worker` (.NET Worker): consumes **In**, routes to **Wait** or **Process**
- `Processor.Worker` (.NET Worker): consumes **Process**, calls `BossMock.Api`, sends failures to **Error**
- `Waiter.Worker` (.NET Worker): timer-based mover from **Wait** → **Process**
- `Admin.Api` (HTTP API): view/unblock/replay (minimal “Sync Admin Tool” equivalent)
- `BossMock.Api` (HTTP API): downstream dependency to simulate success/failure
- `Contracts` (class library): shared message contract and queue naming

Queues (Azure Service Bus):
- `poc-in`
- `poc-wait`
- `poc-process`
- `poc-error`

State store (for “blocked sessions”):
- Azure Table Storage table `BlockedSessions` (or Cosmos/Redis later; Table is simplest)

---

## Prerequisites
- .NET 8 SDK
- Azure subscription access with permission to create:
  - Service Bus namespace + queues
  - Storage account (Table)
- Azure CLI (for provisioning), or equivalent portal access

---

## Phase 0 — Decide POC conventions
Use these conventions throughout:
- **Session key**: `membershipId` (string) maps to Service Bus `SessionId`
- **Message type**: simple JSON (no SOAP/XML), keep it small
- **Ordering rule**:
  - If a session is “blocked” (has an outstanding error), new messages for that session go to **Wait**
  - Once unblocked, waiting messages can move to **Process**

---

## Phase 1 — Provision Azure resources

### Cursor Prompt 1: Create infra-as-code (Bicep) + deploy script
```
Create a new folder `BUPA/poc/async-servicebus-poc/infra`.

Add Bicep to provision:
1) An Azure Service Bus namespace (Standard) and 4 queues:
   - poc-in, poc-wait, poc-process, poc-error
2) Enable sessions on all 4 queues.
3) Set reasonable defaults for a POC:
   - lockDuration: PT1M
   - maxDeliveryCount: 10
4) Create a Storage Account (standard) and a Table named `BlockedSessions`.

Add a PowerShell script `infra/deploy.ps1` that:
- takes resourceGroupName, location, and a unique name suffix
- creates the resource group if missing
- deploys the bicep
- prints out:
  - Service Bus connection string (or instructions to fetch it)
  - Storage connection string (or instructions to fetch it)

Also add `infra/README.md` with exact commands to deploy and to fetch connection strings using Azure CLI.
```

### Expected output
- `infra/main.bicep`
- `infra/deploy.ps1`
- `infra/README.md`

---

## Phase 2 — Create the .NET solution layout

### Cursor Prompt 2: Scaffold a .NET 8 solution with services + shared contracts
```
In `BUPA/poc/async-servicebus-poc/` create a .NET 8 solution named `AsyncFlow.Poc.sln` with these projects:

src/Contracts (class library)
src/Producer.Api (ASP.NET Core Web API, minimal APIs ok)
src/Listener.Worker (Worker Service)
src/Processor.Worker (Worker Service)
src/Waiter.Worker (Worker Service)
src/Admin.Api (ASP.NET Core Web API)
src/BossMock.Api (ASP.NET Core Web API)

Add project references so the services reference Contracts.

Add NuGet packages where appropriate:
- Azure.Messaging.ServiceBus
- Azure.Data.Tables
- Microsoft.Extensions.Http.Polly (or Polly) for retries on HTTP calls
- Swashbuckle.AspNetCore (for the APIs)
- Serilog.AspNetCore + Serilog.Sinks.Console (or stick to built-in logging, but include structured logging)

Add a repo-level `README.md` in `async-servicebus-poc/` describing:
- the 4 queues and the flow
- how to configure connection strings via environment variables
- how to run all services locally (multiple terminals)
```

### Expected output
- `AsyncFlow.Poc.sln`
- `src/*` projects created and referenced
- `async-servicebus-poc/README.md`

---

## Phase 3 — Configuration model (shared)

### Cursor Prompt 3: Add shared settings + queue names
```
In `src/Contracts` add:
1) A `QueueNames` class with constants:
   InQueue = "poc-in"
   WaitQueue = "poc-wait"
   ProcessQueue = "poc-process"
   ErrorQueue = "poc-error"

2) A `ServiceBusSettings` class for:
   - FullyQualifiedNamespace (optional)
   - ConnectionString (for local POC)

3) A `StorageSettings` class for:
   - ConnectionString
   - BlockedSessionsTableName = "BlockedSessions"

4) A message contract `AsyncOperationMessage` with fields:
   - MessageId (string)
   - MembershipId (string)
   - Operation (string)
   - Payload (object or Dictionary<string, string>)
   - CreatedUtc (DateTimeOffset)

Also add a small helper to serialize/deserialize JSON consistently (System.Text.Json).
```

---

## Phase 4 — Producer API (enqueue to In Queue)

### Cursor Prompt 4: Implement Producer API enqueue endpoint
```
Implement `Producer.Api` with:
- POST `/enqueue` accepting a JSON body containing:
  - membershipId
  - operation
  - payload (free-form object)

Behavior:
1) Create an `AsyncOperationMessage`
2) Send to Service Bus `poc-in`
3) Set ServiceBusMessage:
   - MessageId = message.MessageId
   - SessionId = membershipId
   - CorrelationId = message.MessageId
   - ContentType = "application/json"
4) Return 202 Accepted with messageId

Configuration:
- Read `SERVICEBUS_CONNECTION_STRING` from environment variables
- Fail fast with a clear error if missing

Add Swagger and a sample request in the README.
```

---

## Phase 5 — Blocked-session store (Table Storage)

### Cursor Prompt 5: Implement BlockedSessionStore abstraction
```
Create a small component (in each service or in Contracts) called `BlockedSessionStore` backed by Azure Table Storage:

- Table: BlockedSessions
- PartitionKey: "blocked"
- RowKey: membershipId

Store fields:
- MembershipId
- BlockedUtc
- Reason (string)
- ErrorMessageId (string, optional)

API methods:
- IsBlockedAsync(membershipId)
- BlockAsync(membershipId, reason, errorMessageId)
- UnblockAsync(membershipId)
- ListBlockedAsync()

Add DI registration helpers and ensure the table is created if missing (POC convenience).

Use env var `STORAGE_CONNECTION_STRING`.
```

---

## Phase 6 — Listener Worker (In → Wait/Process)

### Cursor Prompt 6: Build Listener.Worker routing logic
```
Implement `Listener.Worker` to consume messages from `poc-in` using `ServiceBusSessionProcessor`.

For each received message:
1) Deserialize AsyncOperationMessage
2) Call BlockedSessionStore.IsBlockedAsync(message.MembershipId)
3) If blocked:
   - forward the same payload to `poc-wait` with the same SessionId
   - complete the original message from `poc-in`
4) Else:
   - forward to `poc-process` with the same SessionId
   - complete original

Notes:
- Include structured logs with messageId, membershipId, sourceQueue, targetQueue.
- Use a single `ServiceBusClient` via DI.
- Configure concurrency to something safe for a POC (e.g., 2 sessions).
```

---

## Phase 7 — Processor Worker (Process → success or Error)

### Cursor Prompt 7: Implement Processor.Worker with downstream call and error routing
```
Implement `BossMock.Api` with:
- POST `/boss/process` that sometimes fails based on input:
  - If operation == "FAIL" return 500
  - Else return 200 with a simple response

Implement `Processor.Worker` to consume from `poc-process` using `ServiceBusSessionProcessor`:
1) Deserialize AsyncOperationMessage
2) Call BossMock.Api `/boss/process` with retries for transient errors
3) On success:
   - complete the message
4) On failure after retries:
   - send the message to `poc-error` (same SessionId)
   - call BlockedSessionStore.BlockAsync(membershipId, reason, messageId)
   - complete the original message from `poc-process`

Add clear logs:
- success path
- failure path (include exception details)
```

---

## Phase 8 — Waiter Worker (timer-based Wait → Process)

### Cursor Prompt 8: Implement Waiter.Worker timer mover
```
Implement `Waiter.Worker` as a Worker Service that runs every 30 seconds:

Algorithm (POC-friendly):
1) Peek/receive a limited batch from `poc-wait` (e.g., up to 20)
2) For each message:
   - deserialize, check BlockedSessionStore.IsBlockedAsync(membershipId)
   - if not blocked:
       - forward to `poc-process` (same SessionId)
       - complete the message from `poc-wait`
   - else leave it in wait (abandon)

Keep it simple and safe:
- add a max loop time per tick
- log how many moved vs left waiting
```

---

## Phase 9 — Admin API (minimal “Sync Admin Tool”)

### Cursor Prompt 9: Implement Admin.Api for unblock and replay
```
Implement `Admin.Api` with endpoints:

GET `/admin/blocked`
- returns the list from BlockedSessionStore

POST `/admin/unblock/{membershipId}`
- removes the block entry for that membershipId

POST `/admin/replay-error/{membershipId}`
- reads messages for that session from `poc-error` (use session receiver)
- forwards them to `poc-process` (same SessionId)
- completes them from `poc-error`

Also add:
GET `/admin/health`

Add Swagger and document a typical support workflow:
1) see blocked sessions
2) unblock
3) replay error
4) wait timer moves wait → process automatically
```

---

## Phase 10 — Runbook and test plan

### Cursor Prompt 10: Add run scripts + a guided test plan
```
Add to `async-servicebus-poc/README.md`:
- a table of all environment variables needed for each service
- commands to run each service (dotnet run)
- a small PowerShell script `run-local.ps1` that starts all services in separate processes (best-effort)

Add a "Test plan" section that proves the flow:
1) Enqueue two messages for same membershipId:
   - first with operation=FAIL
   - second with operation=OK
2) Verify:
   - first ends up in poc-error
   - membershipId is blocked in storage
   - second ends up in poc-wait (not processed yet)
3) Unblock and replay via Admin.Api
4) Verify:
   - waiter moves message from wait to process
   - processor succeeds after replay/unblock

Include example curl commands for enqueue and admin calls.
```

---

## Notes / simplifications (for learning)
- This POC uses Azure Table Storage as the “blocked session registry” so **Listener** and **Waiter** can make consistent routing decisions.
- In a real implementation you’d likely add:
  - application-level idempotency (dedupe)
  - richer poison-message handling (DLQ)
  - more robust replay tooling
  - distributed tracing across Service Bus + HTTP

