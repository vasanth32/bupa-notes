## Claims Domain API – Design summary

### Purpose (from PDF)

Claims API is a secure REST interface that enables **submission, retrieval, and management of Medical & Ancillary claims**, including integration with external systems (example explicitly mentioned: **Medicare**).

---

## Key areas in the domain model (high level)

The PDF includes large models for:

- **Ancillary claims**
  - claim summary + line items
  - claim status and exceptions
  - claim payment history
  - estimates (provider scenarios, services, benefits/out-of-pocket)

- **Medical claims**
  - claim header/lines, rejections, transmission details
  - Medicare member details and Medicare status

- **Hospital claims**
  - hospital claim summary + lines, payment history
  - waiting periods and limits models

It also lists additional request/response types such as:

- claim eligibility
- psychiatric waiver
- offline claim payload
- referral maintenance requests (health aids)
- DRG / grouping output payload (very detailed)

---

## Upstream and downstream systems

### Upstream systems (callers)

- Cyclops
- Batch
- Facade Service
- MyBupa

### Downstream systems (dependencies)

- BOSS
- WDM workflow
- Medicare
- 3M GPC API
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via `HI.Library.Authorization` and `[AuthorizePermission]`
  - Examples shown: `ClaimsDomainAPI.Read/Create/Update/Delete`

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hi-api-claims`
- APIM API name: `hiclaimsdomain/{env}`
- Variable group: `hicbocoreservices-claims-api-{env}-vars`
- Key Vault pattern: `banz-hi-claims-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **1200 requests/IP/min**
- Latency: avg **200 ms**, p90 **900 ms**, worst **1.2 s**
- Throughput: avg **500 calls/min**, peak **1500 calls/min**
- HPA: min **4**, max **10**

