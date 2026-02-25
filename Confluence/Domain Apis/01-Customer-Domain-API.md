## Customer Domain API – Design summary

### Purpose (from PDF)

The Customer Service Domain API manages core customer operations and data:

- personal details
- contact information + addresses
- Medicare card details
- communication preferences
- external cards linked to a person
- overseas + student details
- alerts and additional details (ABD/LHC info)

### Key capabilities listed

- **Customer information management**: create/update/retrieve customer details
- **Advanced people search**
- **External card management**: add/delete/retrieve external cards
- **Address/contact management**: add/update/retrieve/delete
- **Medicare card management**: add/update/delete
- **Communication preferences**: create/update with contact info
- **Overseas/student details**: add/update/delete/retrieve
- **Alerts + additional details**: person alerts and ABD/LHC info
- **Bupa contact management**: create/update Bupa contact details

---

## Upstream and downstream systems

### Upstream systems (callers)

- Cyclops
- Apollo
- Batch Jobs
- Facade Services
- Digital (MyBupa)
- IVR
- Access Verification

### Downstream systems (dependencies)

- BOSS
- Address Validation Service
- Apollo
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token (`Authorization: Bearer <token>`)
- **Token validation**: performed at APIM (issuer/audience/signature/lifetime)
- **Authorization**: permission-based via `BUPA.HI.Authorization` package and `[AuthorizePermission]`
  - Examples shown: `CustomerDomainAPI.Read` (GET), `CustomerDomainAPI.Create` (POST), `CustomerDomainAPI.Update` (PUT), `CustomerDomainAPI.Delete` (DELETE)
- **Mutual TLS**: PDF states client applications must use an SSL client certificate; validated at APIM

---

## Observability & error handling (as described)

- Application Insights logging via reusable logging package (OpenTelemetry exporter mentioned)
- Global exception handling via exception-handling package
- Custom correlation handling (header name varies across docs; see `00-Common-Standards.md`)
- Error schema shown in PDF uses `data` + `errors[]`

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hi-api-customer`
- APIM API name: `hicustomerdomain/{env}`
- Variable group: `hicbocoreservices-customer-api-{env}-vars`
- Key Vault pattern mentioned: `banz-hi-cust-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **1000 requests/IP/min**
- Latency: avg **500 ms**, p90 **1 s**, worst **1.5 s**
- Throughput: avg **2000 calls/min**, peak **5000 calls/min**
- HPA: min **6**, max **10**

