## Design review guidelines (API design checklist)

Source: Confluence PDF “Design Review Guidelines” exported on 2025-02-26.

### Goal

Use this checklist before development starts so that:

- upstream/downstream impacts are understood
- endpoint contracts are consistent and maintainable
- REST conventions are followed (or exceptions are explicitly justified)

---

## 1) Upstream & dependencies

- Verify upstream endpoints are correct and aligned with intended integration points.
- Confirm **endpoint ownership** and routing (examples from the guideline):
  - `hi/apollo/` → APIs directly hitting Apollo
  - `hi/hugo/` → APIs directly hitting Hugo
- **URI rule**: For this Hugo Cap Mod project, URIs **must not include version numbers** (e.g., avoid `/v1`, `/v2` in the path).

---

## 2) Endpoint design

### RESTful endpoints

- Use appropriate identifiers in URI (e.g., `membershipId`, `personId`).
- `404` is allowed only when a true **resource lookup** fails (example: `GET /customers/{id}`).

### Non‑RESTful endpoints

- Must justify why they cannot be RESTful.
- Should not contain identifiers in the path; move identifiers to:
  - query params (for `GET` / `DELETE`)
  - request body (for `POST` / `PUT`)

Example from the guideline:

- ❌ `/hi/memberships/services/{membershipId}/eclipse-eligibility-history`
- ✅ `/hi/memberships/services/eclipse-eligibility-history?membershipId=123`

---

## 3) Request/response & domain model

- For RESTful endpoints, request/response types must match the **domain model**.
- Simplify/refactor complex or bloated response objects.
- Response code usage:
  - Discuss any introduction of `404`, `422`, `429` with the broader group.
  - Prefer `200 OK` with empty body for “no data” scenarios (instead of `404`) when appropriate.
- For `POST`/`PUT` return created/updated resource or identifiers.

---

## 4) Request validation

- **Enum values** must be listed in the Field Info table (in the design doc).
- Provide example values or field descriptions.
- Path params are structural; do not add business validation rules for “missing path params”.
- Confirm correct handling of query/body parameters.

---

## 5) HTTP method expectations (response codes)

### GET

- Validate query/path params and URI consistency.
- Allowed response codes: `200`, `400`, `401`, `403`.
- No business validation rules expected (if needed, discuss first).

### POST

- Use for **create** (not disguised GETs).
- Requires session token.
- Return created object/identifier.
- Response codes: `201`, `400`, `401`, `403`, `409`.
- `200` only allowed for “read-style POSTs” (must be marked as **read scope**).

### PUT

- Use for update of an existing resource.
- Identifier must be in URI.
- Requires session token.
- Return updated object/identifier.
- Response codes: `200`, `400`, `401`, `403`.
- Idempotent requests may return `200` with request body echoed back (noted as “to be confirmed” in guideline).

### DELETE

- Use for true resource deletions.
- Identifier required in URI.
- Requires session token.
- Response codes: `204`, `400`, `401`, `403`.

---

## 6) Headers & auth

- Confirm the standard auth header keyword: **`SessionToken`** (as per guideline).
- Confirm whether `Ocp-Apim-Subscription-Key` is required.
- Capture authorization requirements explicitly.

---

## 7) Confluence hygiene (before dev starts)

- All comments must be resolved before development begins.
- Ensure sample requests and environment setup are usable.

---

## 8) Naming & conventions

- Rename `lanId` / `bossLanId` → `bossUserId`.
- Maintain consistent casing and parameter naming conventions.
- JSON request/response types should have “@” before types (as stated in guideline; apply consistently with the team’s API spec format).

