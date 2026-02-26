# Cursor POC Prompt Pack — PR Code Review Checklist (Devs)

Use this to **learn the full checklist by doing** in a small .NET 8 POC, fast.  
Copy/paste the prompts into Cursor chat / Composer. Keep the scope tiny (one feature, one endpoint group, tests).

## Suggested timebox (fast)

- 15 min: scaffold POC + baseline endpoints
- 45 min: implement “good” version (aligned with checklist)
- 30 min: intentionally add 3–5 “bad” violations, then fix them
- 20 min: produce PR evidence + write learning notes

## POC scope (recommended)

Build an `Order API` with 4 endpoints and a simple in-memory “repo” (or fake persistence):
- `GET /api/orders/{id}`
- `GET /api/orders?skip=&take=`
- `POST /api/orders`
- `PUT /api/orders/{id}`
- `DELETE /api/orders/{id}`

Layers must be: **Controller → Service → Repository**, with **Domain objects between layers**.

## Folder suggestion

Create it under:
- `BUPA/Poc-Projects/PR-Checklist-Poc/`

---

## Master “create the POC” prompt

Paste this first.

> **PROMPT**
>
> You are helping me learn a PR code review checklist by building a tiny POC in **.NET 8**.
>
> Create a new solution under `BUPA/Poc-Projects/PR-Checklist-Poc/` with:
> - `src/ChecklistPoc.Api` (ASP.NET Core Web API)
> - `tests/ChecklistPoc.UnitTests` (xUnit + Moq)
>
> Implement strict 3-tier boundaries:
> - Controllers only call Services.
> - Services call their Repository and/or other Services.
> - Repositories do data access only (no business logic).
>
> Use:
> - file-scoped namespaces
> - async everywhere with `Async` suffix
> - `CancellationToken` passed end-to-end (not optional defaults)
> - repositories return `T?` only for single-get “may not exist”; collections must be empty (never null)
> - repositories **never throw**
> - services/controllers never return null; services throw meaningful custom exceptions; controller maps exceptions to HTTP via middleware/filter
> - Domain objects only between layers (no DTO/entity leakage between controller/service/repo)
>
> Add Orders endpoints and all required code (domain, exceptions, validations, service, repo, controller, middleware, DI).
> Add unit tests to reach **>= 90%** coverage for service + middleware mapping.
>
> After creating the code, generate `LEARNING_NOTES.md` in the POC folder that explains *how each checklist rule is satisfied* with links to the relevant files/classes.

---

## Checklist item prompts (do + explain)

### 1) Don’t copy/paste legacy code “as-is”

> **PROMPT (do)**
>
> Add a new feature to the POC in two commits (simulate): first add a “legacy-ish” implementation (bad naming, missing cancellation token, nulls), then refactor it to match the checklist standards. Keep behavior identical. Show me a short diff-style summary of what changed and why.

> **PROMPT (explain in markdown)**
>
> Update `LEARNING_NOTES.md` with: “How I recognize legacy patterns” and a checklist of refactor steps I should do before raising PRs.

---

### 2) 3-tier structure: Controller → Service → Repository

> **PROMPT (do)**
>
> Audit the POC and ensure responsibilities are correct:
> - controller: request parsing + calling service + return result only
> - service: business rules, orchestration, exception decisions
> - repo: persistence only, no business decisions
>
> If anything violates this, refactor. Then add unit tests that would fail if business logic leaks into repository.

> **PROMPT (explain in markdown)**
>
> In `LEARNING_NOTES.md`, add a table: “Layer → allowed responsibilities → forbidden responsibilities” with 2–3 examples from this POC.

---

### 3) Avoid null + unnecessary null checks

> **PROMPT (do)**
>
> Ensure: repository can return `Order?` only for `FindByIdAsync`; collection reads return `IEnumerable<Order>` and never null; repository never throws; service never returns null and throws `NotFoundException` when repo returns null; controller never returns null.
>
> Add unit tests verifying these contracts explicitly.

> **PROMPT (explain in markdown)**
>
> Add “Null-handling rules” to `LEARNING_NOTES.md` including examples of correct signatures (`Task<T?>`, `Task<IEnumerable<T>>`) and where null is forbidden.

---

### 4) Use Domain objects between tiers (not DTOs/entities)

> **PROMPT (do)**
>
> Ensure controller/service/repo communicate using `Order` (domain). If the API response needs fewer fields, use projections (anonymous objects) only in the controller, without introducing “DTO sprawl”. Demonstrate one endpoint that returns a projection.

> **PROMPT (explain in markdown)**
>
> Add a section: “Domain vs DTO vs Entity (in this POC)”, including where each is allowed and why.

---

### 5) Service conventions (ServiceBase + IService)

> **PROMPT (do)**
>
> If the POC doesn’t have `ServiceBase`/`IService`, add them (minimal but realistic). Put cross-cutting concerns (like logging helpers) in `ServiceBase` only if it truly helps. Update DI registration accordingly.

> **PROMPT (explain in markdown)**
>
> Explain in `LEARNING_NOTES.md`: why a service base/interface pattern exists, what belongs there, and what does NOT.

---

### 6) Meaningful names; consistent typing style; collection init `[]`

> **PROMPT (do)**
>
> Enforce naming: no abbreviations like `rdr`, no typos. Use consistent local typing style: prefer `var` where possible; do not mix `var` and explicit types in the same statement. Use collection init expressions like `List<Order> orders = [];` where appropriate.
>
> Add a small refactor commit that improves naming + consistency without behavior changes.

> **PROMPT (explain in markdown)**
>
> Add “Naming + typing conventions” to `LEARNING_NOTES.md` with before/after examples from the refactor.

---

### 7) Async conventions (`Async` suffix, `IEnumerable<T>`, CancellationToken)

> **PROMPT (do)**
>
> Audit every async method: ensure it ends with `Async`. Ensure every async path accepts/passes `CancellationToken` (no optional defaults). Ensure collection returns use `IEnumerable<T>`.
>
> Add unit tests to ensure service passes cancellation token to repository.

> **PROMPT (explain in markdown)**
>
> Add a section: “Async + CancellationToken rules” with a short “review checklist” I can run mentally.

---

### 8) File-scoped namespaces

> **PROMPT (do)**
>
> Ensure all new files use file-scoped namespaces (`namespace X;`). If any block-scoped namespace exists, convert it.

> **PROMPT (explain in markdown)**
>
> Add a 3-bullet explanation: why file-scoped namespaces improve readability/diffs.

---

### 9) Function-scoped `using`; avoid unnecessary try/catch

> **PROMPT (do)**
>
> Refactor any disposable usage to function-scoped `using var` / `await using` where applicable. Remove any `try/catch` that adds no value. If catching exceptions, catch specific types and translate to meaningful domain/service exceptions.
>
> Add tests proving exception translation preserves the original exception as `InnerException` when appropriate.

> **PROMPT (explain in markdown)**
>
> Add a section: “When try/catch is justified vs harmful” with 2 concrete examples from this POC.

---

### 10) Remove extra lines + unused imports

> **PROMPT (do)**
>
> Clean formatting: remove unused usings, trailing whitespace, random blank lines. Run `dotnet format` if available; otherwise rely on analyzer/fixups. Keep diffs clean.

> **PROMPT (explain in markdown)**
>
> Add a 5-item “Clean diff” checklist I can run before PR.

---

### 11) No typos in method/variable names

> **PROMPT (do)**
>
> Scan for typos and ambiguous names. Use rename refactors (not manual edits) to fix them. Ensure tests still pass.

> **PROMPT (explain in markdown)**
>
> Add: “Why typos are expensive” + a rule: always prefer rename tooling.

---

### 12) Initialize model properties with non-null defaults

> **PROMPT (do)**
>
> Ensure all non-nullable properties in domain/models are initialized (constructor or defaults like `string.Empty`, `[]`). Remove any `null!` usage unless there’s a strong reason.
>
> Add a unit test that would have caught a null default bug.

> **PROMPT (explain in markdown)**
>
> Add a section explaining how non-null defaults reduce runtime bugs and simplify code.

---

### 13) Don’t create “Helper” methods

> **PROMPT (do)**
>
> If any method/class is named `Helper`, remove/rename it into a responsibility-focused type (e.g., `OrderNumberParser`). If not present, intentionally add a bad `Helper` then refactor it into a proper cohesive type.

> **PROMPT (explain in markdown)**
>
> Add: “How to name utility code without ‘Helper’” with 5 naming examples.

---

### 14) Be judicious with extension methods

> **PROMPT (do)**
>
> Propose an extension method that might be tempting (e.g., `OrderExtensions.IsLarge()`), then decide whether it’s justified in this POC. If not, implement the same behavior in a clearer place (domain method/service). Document trade-offs.

> **PROMPT (explain in markdown)**
>
> Add a short decision framework: when extensions are OK vs when they become “global magic”.

---

### 15) Proper exception handling (don’t catch generic `Exception`)

> **PROMPT (do)**
>
> Ensure there is no `catch (Exception)` unless it rethrows immediately with added context AND preserves the original exception. Prefer catching specific exceptions. Ensure repository never throws; service translates repo outcomes into meaningful exceptions; middleware maps them to HTTP.
>
> Add tests for 404 (not found) and 400 (validation) and 500 (unexpected).

> **PROMPT (explain in markdown)**
>
> Add: “Exception taxonomy for APIs” (validation vs not found vs conflict vs unexpected) and how each maps to HTTP.

---

### 16) Validation belongs in Domain objects (.NET 8 attributes, then IValidatableObject)

> **PROMPT (do)**
>
> Implement domain validation for `Order` and/or `CreateOrderRequest`:
> - use .NET attributes where possible
> - if needed, use `IValidatableObject` for cross-field rules
>
> Ensure validation errors are surfaced as meaningful 400 responses, with unit tests.

> **PROMPT (explain in markdown)**
>
> Add: “Validation ladder” (attributes → IValidatableObject → business rules) and examples from the POC.

---

### 17) XML documentation (no empty comments)

> **PROMPT (do)**
>
> Add XML docs to the public surface area that matters (public services/exceptions/controllers). No empty stubs. If you generate docs, verify accuracy. Ensure docs describe contract (inputs/outputs/exceptions) not implementation trivia.

> **PROMPT (explain in markdown)**
>
> Add a section: “What good XML docs contain” + 1 example from this POC.

---

### 18) Mandatory PR evidence (Sonar/CheckmarX, Swagger/Postman, coverage >= 90, Confluence samples)

> **PROMPT (do)**
>
> Create a `PR_EVIDENCE.md` in the POC folder with placeholders and instructions to fill:
> - SonarQube status (no medium/high)
> - CheckmarX status (no medium/high)
> - Swagger/Postman “local run” evidence
> - Coverage command + resulting percentage (>= 90)
> - Confluence sample request/response template per endpoint (include required headers/prereqs/dependencies; use “NA” if not applicable)
>
> Also tell me the exact commands to run locally to generate coverage (cross-platform dotnet commands).

---

### 19) Unit test regions + naming conventions (List/Create/Update/Delete)

> **PROMPT (do)**
>
> Organize unit tests using `#region` blocks for List/Create/Update/Delete **in test files only**. Ensure test file naming matches `SomethingUnitTests.cs`.
>
> Also review this existing test file and point out misalignments with the checklist (especially “repository never throws” rule):
> `BUPA/Poc-Projects/Unit Test/tests/Demo.OrderApi.UnitTests/UnitTest1.cs`
>
> Provide a refactor plan for that file (don’t change production code unless necessary).

---

### 20) “Your code is your responsibility” — pre-PR routine

> **PROMPT (explain in markdown)**
>
> Append to `LEARNING_NOTES.md` a “Senior pre-PR routine” that is a single page: architecture, contracts, async, validation/exceptions, clean diffs, docs, tests, evidence.

---

## Fast “self-review this PR” prompt (use on any PR)

> **PROMPT**
>
> Review my changes like a strict BUPA PR reviewer using the checklist rules:
> - tier boundaries
> - domain objects between layers
> - null rules
> - async/cancellation conventions
> - exception handling rules
> - validation in domain
> - naming/typos/formatting
> - XML docs completeness
> - tests: >= 90% coverage + required regions in test files
> - PR evidence completeness (Sonar/CheckmarX/Swagger/Confluence)
>
> Output:
> 1) “Must fix before approval”
> 2) “Should fix”
> 3) “Nice to have”
> 4) A short suggested PR description with evidence checklist.

