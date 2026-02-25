## Payment Domain API – Design summary

### Purpose (from PDF)

Payment Domain API manages payment operations and data, including:

- bank account and direct debit
- payment transactions (single and batch)
- refunds
- PayPal and Stripe related services

The PDF also notes that for some card flows (create/update/retrieve), an **Enterprise payment API** is used.

---

## Key entities / areas listed

- Payment card + card details and transaction
- Bank account (direct credit/direct debit)
- Single payment
- Batch payment / batch refund
- Direct debit details + next payment / next direct debit
- Corporate: corporate DD, adjustment payments, transfers
- PayPal: token, details, transactions
- Renewal payment info (frequency, method)

---

## Upstream and downstream systems

### Upstream systems (callers)

- Cyclops
- Apollo
- Batch Jobs
- Facade Services
- Digital (MyBupa)
- IVR
- SpeechStorm

### Downstream systems (dependencies)

- Enterprise Payment API
- BOSS
- Redis cache (caching)

---

## Security model (high level)

- **Auth**: OAuth2 + JWT bearer token
- **Token validation**: at APIM
- **Authorization**: permissions via shared authorization package and `[AuthorizePermission]`
  - Examples shown: `PaymentDomainAPI.Read/Create/Update/Delete`

---

## Deployment & runtime notes (from PDF)

- AKS namespace: `hicbocoreservices-{env}-ns`
- Container: `hi-api-payment`
- APIM API name: `hipaymentdomain/{env}`
- Variable group: `hicbocoreservices-payment-api-{env}-vars`
- Key Vault pattern: `banz-hi-payment-{env}-kv01`

Non-functional targets (as captured in the PDF):

- Rate limiting: **2100 requests/IP/min**
- Latency: avg **500 ms**, p90 **1 s**, worst **1.5 s**
- Throughput: avg **1000 calls/min**, peak **2100 calls/min**
- HPA: min **4**, max **10**

