## Interaction Domain API – Design summary

### Purpose (from PDF)

The Interaction API exposes endpoints enabling upstream systems to:

- **Create / retrieve / update / terminate Hugo sessions**
- **Initiate Apollo sessions**
- **Retrieve audit information**

This API is a key part of “session” flows: the domain model includes a **`SessionToken`** and session expiry details.

---

## Key data areas in the domain model (from PDF)

- **HugoSession**
  - personId, user/team, client details (machine, IP), sessionId
  - session timing + `SessionToken` + expiry + session end indicator
  - audits with Before/After payloads (dynamic based on audit type)

- **ApolloSession**
  - userId/personId, identity provider data, source channel, CRM activity id

- **Printer** (explicitly marked as non-RESTful in the PDF)

- **Document** and **Email** models also appear in the extracted domain model section (documents, HTML, comm hub, email template payloads).

---

## Upstream and downstream systems

### Upstream systems (callers)

- Apollo BFF
- Batch Jobs
- Cyclops
- Customer Domain API
- Digital (MyBupa)

### Downstream systems (dependencies)

- Apollo CRM
- Hugo DB
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via shared authorization package and `[AuthorizePermission]`
  - Examples shown: `InteractionDomainAPI.Read/Create/Update/Delete`

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hidomain-interaction`
- APIM API name: `hiinteractiondomain/{env}`
- Variable group: `hicbocoreservices-interaction-api-{env}-vars`
- Key Vault pattern: `banz-hi-inter-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **1200 requests/IP/min**
- Latency: avg **300 ms**, p90 **750 ms**, worst **1 s**
- Throughput: avg **500 calls/min**, peak **1500 calls/min**
- HPA: min **3**, max **10**

