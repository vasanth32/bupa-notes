## Sync Domain API (Hugo → Apollo Sync API) – Design summary

### Purpose (from PDF)

The Sync Domain API provides endpoints that send messages published by **HUGO/BOSS** to **Azure Service Bus**, which are then synced to **Apollo (CRM)**.

---

## Core batch models

### `syncBatch`

Described as sending sync messages as a batch to a requested queue. Each message includes fields like:

- `messageId`, `transactionId`
- `syncMethod` (Create/Update/Delete)
- `entityType` (large enum list in the PDF: Person, Membership, Contacts, Cards, Communication prefs, Flags, Orders, SalesTransaction, etc.)
- `streamId`, `entityId`, `timestamp`
- `body` (`JToken` JSON payload)

### `crmSyncBatch`

Described as a CRM sync batch message model (also supports a “syncFailedMessages” flag in the PDF).

---

## Upstream and downstream systems

### Upstream systems (callers)

- Titan Datastream Service (named in PDF)

### Downstream systems (dependencies)

- Azure Service Bus
- CRM
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via shared authorization package and `[AuthorizePermission]`
  - Example shown: `SyncDomainAPI.Create` for POST (per-endpoint permissions are expected in detailed design)

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hi-api-sync`
- APIM API name: `hisyncdomain/{env}`
- Variable group: `hicbocoreservices-sync-api-{env}-vars`
- Key Vault pattern: `banz-hi-sync-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **2100 requests/IP/min**
- Latency: avg **1 s**, p90 **1.3 s**, worst **1.5 s**
- Throughput: avg **800 calls/min**, peak **2100 calls/min**
- HPA: min **2**, max **10**

