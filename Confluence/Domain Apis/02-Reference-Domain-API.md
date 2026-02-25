## Reference Domain API – Design summary

### Purpose (from PDF)

The Reference Domain API provides centralised access to a wide range of **static and semi-static reference data** used across BUPA systems, so consumers can retrieve consistent reference information.

### Example reference areas listed

The PDF enumerates many categories, including (not exhaustive):

- Claims: claim types, rejection codes, modalities, hospital claim groups, health aids, etc.
- Customer & membership: family, membership, relation roles, customer info categories/groups
- Communication & preferences: topics, delivery methods, contact type delivery methods
- Documents & emails: document types, rendering templates, email templates
- Bank: banks and BSB details
- Geographic/regulatory: countries, locations
- Admin/config: application version, configuration data, logs, etc.

---

## Upstream and downstream systems

### Upstream systems (callers)

- Cyclops
- Apollo
- Batch Jobs
- Facade Services
- Digital (MyBupa)
- IVR

### Downstream systems (dependencies)

- Hugo and BOSS DB
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via shared authorization package and `[AuthorizePermission]`
  - Examples shown: `ReferenceDomainAPI.Read` etc.

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hidomain-reference`
- APIM API name: `hireferencedomain/{env}`
- Variable group: `hicbocoreservices-reference-api-{env}-vars`
- Key Vault pattern: `banz-hi-ref-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **1000 requests/IP/min**
- Latency: avg **100 ms**, p90 **600 ms**, worst **1.2 s**
- Throughput: avg **500 calls/min**, peak **1500 calls/min**
- HPA: min **4**, max **10**

