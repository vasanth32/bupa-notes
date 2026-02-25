## Membership Domain API – Design summary

### Purpose (from PDF)

The Membership module provides REST APIs to manage membership data and related entities, covering:

- policy details and people covered
- alerts and notes
- fulfilment and digital/physical cards
- communications and tax statements
- financial information and organisational relationships

---

## Key entities listed (examples)

- PolicyDetails, PersonCovered, PersonCoverHistory
- Alert, Note
- PackageInfo, Fund/Badge/Brand
- Premium details, Rebate, LHC loading, ABD
- Clearance requests
- Communication preferences
- Fulfilment, DigitalCard
- Organisation, organisation sales messaging

---

## Upstream and downstream systems

### Upstream systems (callers)

Includes a wide set of consumers in the PDF:

- Apollo, Cyclops, Batch Jobs
- Digital Cards, Digital (MyBupa)
- Facade Services, IVR, SpeechStorm
- Titanium, Corporate CPC, Connexions CPC
- Access Verification, Blua, PACE ENE

### Downstream systems (dependencies)

- BOSS
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via shared authorization package and `[AuthorizePermission]`
  - Examples shown: `MembershipDomainAPI.Read/Create/Update/Delete`

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hidomain-membership`
- APIM API name: `himembershipdomain/{env}`
- Variable group: `hicbocoreservices-membership-api-{env}-vars`
- Key Vault pattern: `banz-hi-membership-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **2000 requests/IP/min**
- Latency: avg **300 ms**, p90 **1 s**, worst **1.5 s**
- Throughput: avg **4000 calls/min**, peak **6000 calls/min**
- HPA: min **6**, max **10**

