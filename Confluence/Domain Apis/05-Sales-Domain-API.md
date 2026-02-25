## Sales Domain API – Design summary

### Purpose (from PDF)

Sales Domain API handles sales functionality that helps complete sale of a policy.

Key operations listed:

- Get a sales transaction by ID
- Get a person’s sales transaction list
- Create a new sales transaction
- Update a sales transaction
- Get sales transactions for a membership by type(s)

---

## Domain model (high level)

The PDF contains a large domain model covering sales leads/quotes/orders, including:

- lead admin + contact info
- lead/product details (including OSHC/OVC-like product types)
- person info and family details
- quote order (people, visa/overseas fields, rebate details, clearance requests, fulfilments)
- sales transaction and status
- calculators for ABD/LHC/rates/premium

---

## Upstream and downstream systems

### Upstream systems (callers)

- BFF
- BupaComAu
- Batch Jobs
- Facade Services
- Cyclops
- Titanium
- Apollo

### Downstream systems (dependencies)

- BOSS
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via `BUPA.HI.Authorization` and `[AuthorizePermission]`
  - Examples shown: `SalesDomainAPI.Read/Create/Update/Delete`

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hi-api-sales`
- APIM API name: `hisalesdomain/{env}`
- Variable group: `hicbocoreservices-sales-api-{env}-vars`
- Key Vault pattern: `banz-hi-sales-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **2000 requests/IP/min**
- Latency: avg **900 ms**, p90 **1.5 s**, worst **2.5 s**
- Throughput: avg **600 calls/min**, peak **2700 calls/min**
- HPA: min **4**, max **10**

