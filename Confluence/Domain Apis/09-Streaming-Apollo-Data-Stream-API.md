## Streaming Domain API (Apollo Data Stream API) – Design summary

### Purpose (from PDF)

The Apollo Data Stream API provides an endpoint that sends messages to requested **queues/topics in Azure Service Bus**. From there, **function apps forward messages to Pega**.

This API is described as an endpoint Apollo can invoke (externally hosted in the described setup).

---

## Core message model

The PDF shows a `streamMessage` payload including:

- `bupa_event_sequence`
- `bupa_eventsource`
- `bupa_eventtime`
- `bupa_eventtype`
- `bupa_primaryentityid`
- `bupa_queuename` / `bupa_topicname`
- `bupa_subscribername`
- `body` (object payload)

---

## Upstream and downstream systems

### Upstream systems (callers)

- Apollo (externally hosted endpoint Apollo can invoke)

### Downstream systems (dependencies)

- Azure Service Bus
- CRM (listed in PDF)
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via shared authorization package and `[AuthorizePermission]`
  - Example shown: `StreamingDomainAPI.Create` for POST (the PDF’s permission snippet is inconsistent in places; treat per-endpoint permissions as source-of-truth in the detailed design)

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hi-api-streaming`
- APIM API name: `histreamingdomain/{env}`
- Variable group: `hicbocoreservices-streaming-api-{env}-vars`
- Key Vault pattern: `banz-hi-streaming-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **1000 requests/IP/min**
- Latency: avg **200 ms**, p90 **1 s**, worst **1.5 s**
- Throughput: avg **600 calls/min**, peak **1500 calls/min**
- HPA: min **2**, max **10**

