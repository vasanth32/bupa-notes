## Provider Domain API – Design summary

### Purpose (from PDF)

Provider Domain API manages and retrieves healthcare provider information (medical and ancillary), including:

- searching providers
- retrieving provider summary/detail
- provider relationships
- provider status (active/valid/suspended/ceased)
- health aids and pseudo provider data

Key operations explicitly listed:

- Get provider by ID
- Search provider

---

## Upstream and downstream systems

### Upstream systems (callers)

- Cyclops
- Batch Jobs
- Facade Services
- Digital (MyBupa)

### Downstream systems (dependencies)

- PMP API
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via shared authorization package and `[AuthorizePermission]`
  - Examples shown: `ProviderDomainAPI.Read/Create/Update/Delete`

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hi-api-provider`
- APIM API name: `hiproviderdomain/{env}`
- Variable group: `hicbocoreservices-provider-api-{env}-vars`
- Key Vault pattern: `banz-hi-prov-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **350 requests/IP/min**
- Latency: avg **300 ms**, p90 **500 ms**, worst **1 s**
- Throughput: avg **150 calls/min**, peak **350 calls/min**
- HPA: min **2**, max **10**

