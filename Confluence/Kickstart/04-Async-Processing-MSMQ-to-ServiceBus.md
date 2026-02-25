## Async processing: MSMQ → Azure Service Bus (+ Azure Functions)

### Why this matters

The programme includes a “plumbing modernisation” stream where legacy async processing is moved from:

- **MSMQ + Windows services** (on-prem)
to
- **Azure Service Bus + Azure Functions** (cloud)

The intent is to keep business behaviour the same while modernising transport, reliability, and observability.

---

## Building blocks

### Queues (as described in KT)

Four queues are commonly referenced in the flow:

- **In Queue**: entry point for new messages
- **Wait Queue**: holds messages that must wait because earlier related messages are still processing or in error
- **Process Queue**: active work queue
- **Error Queue**: failed messages requiring investigation/manual action

### Functions (as described in KT)

- **Listener Function** (queue-triggered on **In Queue**)
- **Processor Function** (queue-triggered on **Process Queue**)
- **Wait Timer Function** (timer-triggered; scans **Wait Queue**)

### Sessions / ordering

Queues are described as **session-enabled** so that messages for the same logical unit (e.g., membership/session id) can be:

- grouped together
- processed in order
- prevented from “overtaking” each other after an error

---

## End-to-end flow (typical)

### 1) Message creation

An upstream system calls Hugo (WCF) and Hugo creates a message (often SOAP/XML payload) representing a downstream operation (e.g., a BOSS operation). That message is enqueued to **In Queue** on Service Bus.

### 2) Listener Function routing logic

When a message arrives in **In Queue**, the Listener Function decides where it should go next:

- If there is already a related message (same membership/session) in **Process Queue** or **Error Queue**
  - route to **Wait Queue**
- Otherwise
  - route to **Process Queue**

Purpose: preserve ordering and avoid processing “newer” messages while an older message is still running or has failed.

### 3) Processor Function execution

When a message arrives in **Process Queue**, the Processor Function:

- Reads the message
- Resolves the correct **processor agent** (KT mentions ~12–14 agents)
- Converts the payload into the **BOSS request** format (converter step)
- Calls **BOSS service** (still WCF/SOAP in the described design)

Outcomes:

- **Success** → complete the message (removed from Process Queue)
- **Failure/exception** → move to **Error Queue**

### 4) Error handling + Sync Admin Tool

Support teams use a **Sync Admin Tool** (Windows UI) to:

- view messages in In/Wait/Process/Error queues
- investigate failures
- re-queue / move messages after issues are fixed

### 5) Wait Timer Function

Periodically (example from KT: every ~30 seconds), the Wait Timer Function:

- reads messages in **Wait Queue**
- checks if related errors still exist in **Error Queue**
- if no related error remains, moves the message to **Process Queue**

---

## Configuration / deployment notes (patterns)

- Queue names and connection strings are taken from:
  - local development: `local.settings.json` (Function Apps)
  - cloud: app settings / environment variables
- Secrets should be stored in **Key Vault** and referenced from settings.
- Use common **logging / tracing** packages so queue operations show up in Application Insights end-to-end.

