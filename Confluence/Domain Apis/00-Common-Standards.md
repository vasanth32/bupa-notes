## Common standards across Domain APIs (from the design PDFs)

This page captures patterns repeated in the Domain API design PDFs: **security**, **authorization**, **logging/observability**, **error handling**, and **deployment basics**.

---

## Authentication (OAuth2 + JWT)

All Domain APIs describe **Bearer Token Authentication**:

- Clients obtain access tokens from **Microsoft Entra** via:
  - **User credentials** (public clients; user-interactive apps like Cyclops)
  - **Managed identity** (confidential clients; service-to-service)
- Clients send:
  - `Authorization: Bearer <access_token>`

### Token validation responsibility

The PDFs state token validation occurs at **APIM** for each request:

- issuer, audience, signature, lifetime
- invalid → `401 Unauthorized`

Many PDFs also mention token validation exists in the authorization package but is **bypassed** because APIM validates tokens.

---

## Authorization (permissions/scopes/roles)

Authorization is implemented via a shared library and a custom attribute:

- Endpoint is annotated with something like:
  - `[AuthorizePermission("CustomerDomainAPI.Read")]`
- Handler checks that required permission exists in token `scope` or `roles` claim.
- Missing/invalid claims → request rejected (PDFs typically say `401 Unauthorized`).

Typical permission mapping shown:

- `*.Read` → `GET`
- `*.Create` → `POST`
- `*.Update` → `PUT`
- `*.Delete` → `DELETE`

### Public client group mapping (Cyclops example)

For Cyclops (public client), the PDFs mention adding users to an Entra group:

- `RES-CLD-Cyclops-Hugo-dev-access`

That group is associated with the app registration as part of user onboarding.

---

## Mutual TLS (client certificate)

At least the Customer design explicitly mentions:

- Clients must use an **SSL client certificate** when calling the API.
- Certificate validation is performed at **APIM**.

Other APIs list key vault entries like `ClientCertificateBase64` indicating mTLS is used widely.

---

## Common dependencies: caching

Most APIs list caching via **Redis** and reference a caching integration guide:

- “Caching Component – Redis”
- “How to integrate HI.Library.Cache component with domain API”

This is treated as a shared cross-cutting capability.

---

## Logging & observability

The PDFs describe a reusable logging package (names vary slightly):

- Example package names seen:
  - `Bupa.HI.Logging.Package`
  - `HI.Library.Logging`

Core traits:

- Logs/telemetry sent to **Azure Application Insights**
- Uses `ILogger` and OpenTelemetry exporter (noted: `Azure.Monitor.OpenTelemetry.AspNetCore`)

Common logged/correlation fields mentioned:

- Operation ID / Parent ID
- Path + HTTP method
- Trace ID
- Session ID (where relevant)
- Customer ID / Membership ID (domain specific)
- Correlation ID (custom)

---

## Exception handling + error schema

The PDFs describe:

- Global exception middleware from an exception-handling package (names vary, e.g. `HI.Library.ExceptionHandling`, `BUPA.HI.ExceptionHandling`)
- Mapping exception types → HTTP status codes (common set: 400/401/403/404/408/409/412/422/500/501/502/503/504)

### Custom error payload shape

Two variants appear in the PDFs:

**Variant A**

- `data: {}`
- `errors: [{ code, message, severity, classification }]`

**Variant B**

- `data: {}`
- `items: [{ code, message, severity, classification }]`

Where:

- `severity` is an enum like `NotSet | Information | Warning | Error`
- `classification` is `Business | System`

### Request payload storage on exceptions

The PDFs state:

- On exceptions, request payload can be **encrypted** and stored in Application Insights custom dimensions.
- Encryption keys/salts are stored in **Key Vault**.

### Correlation header

The PDFs mention a custom correlation header name, but spelling varies between documents:

- `X-Correlation-Request`
- `X-Correlation-Id`

Behaviour described:

- if missing, middleware generates one and returns it in the response header
- if present, it’s reused for logging

---

## Deployment & runtime (AKS + APIM)

Most APIs describe a similar deployment pattern:

- Hosted in **AKS**
  - Namespace format: `hicbocoreservices-{env}-ns`
- Exposed via **APIM**
  - API name pattern: `hi<domain>domain/{env}` (examples: `hicustomerdomain/{env}`, `hireferencedomain/{env}`, `hiinteractiondomain/{env}`)
- Configuration via ConfigMaps/Secrets and an ADO variable group:
  - Example format: `hicbocoreservices-<domain>-api-{env}-vars`
- Key Vault per domain/environment:
  - Example patterns from PDFs: `banz-hi-<domain>-{env}-kv01`

### Environment naming (as used across these docs)

Repeated strategy in the PDFs:

- DEV: `d24`
- SIT/TEST: `d34`
- PERF: `t33`
- UAT: `euat`
- PROD: `prod`

---

## Rate limiting & NFRs

Each API defines:

- target latency, throughput, and error rate
- IP-based rate limits at APIM (values differ by API)
- HPA (min/max replicas) and resource requests/limits

Use the per-API pages for the exact figures.

