# PR Code Review Checklist (for Devs) — Explained Guide

Source: `DT-PR Code Review Checklist (for Devs)-260226-055250.pdf`

This is a practical “how to not get your PR bounced” guide. For each checklist item, you’ll see:
- **What it means**: the expectation
- **Why it matters**: what reviewers are protecting
- **How to self-check**: quick verification steps you can do before raising the PR

---

## 1) Don’t copy/paste legacy code “as-is”

- **What it means**: If you reuse logic from an older codebase, you must upgrade it to current standards (structure, naming, async/cancel, nullability, validation, exceptions, etc.) while preserving behavior.
- **Why it matters**: Copy/paste drags old bugs, old patterns, and security/quality debt into the new codebase.
- **How to self-check**:
  - Compare the new code against the checklist below (tiers, nullability, naming, async, validation, docs).
  - If you copied something: rewrite it to match current architecture, don’t “wrap it and ship it”.

---

## 2) Use 3-tier structure: Controller → Service → Repository

- **What it means**:
  - **Controller** talks only to **Service(s)**.
  - **Service** talks to **its repository** and/or calls other **Service(s)**.
  - **Repository** is a data-access layer only (retrieve/save/delete) with **no business logic**.
  - Avoid a service depending on many repositories (business rules/validation shouldn’t be duplicated).
- **Why it matters**: Clear boundaries prevent “business logic in data access”, duplicated rules, and untestable code.
- **How to self-check**:
  - Controllers contain: request parsing, auth/headers, calling service, returning result.
  - Services contain: business rules, orchestration, validation triggers, exception decisions.
  - Repositories contain: queries + mapping to domain objects + persistence only.
  - If you see business decisions inside repository (e.g., “if X then throw/validate”), move them to the service/domain.

---

## 3) Avoid `null` and unnecessary null checks

- **What it means**:
  - **Repository** may return `null` only when “single item may not exist”, and the signature must show it (e.g., `Customer?`).
  - For multi-item reads: **return empty collections, always** (never `null`).
  - **Repositories never throw**.
  - **Services and Controllers never return `null`**; they should throw **custom, meaningful exceptions** instead.
- **Why it matters**: `null` spreads defensive programming and causes runtime bugs; clear contracts make code safer and simpler.
- **How to self-check**:
  - Repository methods:
    - Single-get: `Task<T?>` is OK.
    - Multi-get: `Task<IEnumerable<T>>` and return `Enumerable.Empty<T>()` (or `[]` with collection expressions where applicable).
    - No `throw` in repositories.
  - Service/controller methods:
    - Never return `null` for domain/response.
    - Convert “not found/invalid” into domain/service exceptions that your API layer maps to correct HTTP results.

---

## 4) Use Domain objects between tiers (not DTOs/entities)

- **What it means**:
  - Controller/Service/Repository communicate using **Domain objects**.
  - Repository returns a **fully constructed Domain object** (if found).
  - API responses can return domain objects; if you only need a subset, use **projections** (anonymous types) for response shaping.
- **Why it matters**: DTO/entity leakage couples layers and makes changes risky; domain objects keep business meaning stable.
- **How to self-check**:
  - Ensure repository doesn’t return EF entities (or persistence models) to service/controller.
  - Ensure controller/service don’t accept persistence entities as parameters.
  - If response needs subset fields, project in controller/service without inventing “DTO sprawl” unless the project standards require DTOs.

Example projection:

```csharp
var customers = await customerService.GetCustomersAsync(skip, count, cancel);
var firstAndLastNames = customers.Select(c => new
{
    c.FirstName,
    c.LastName
});
```

---

## 5) Service class conventions: inherit `ServiceBase` and implement `IService`

- **What it means**: Service classes follow a consistent base class/interface pattern mandated by the project.
- **Why it matters**: Consistency enables shared concerns (logging, correlation, telemetry, common helpers) and predictable DI registration.
- **How to self-check**:
  - New services: `class XService : ServiceBase, IService` (and any project-specific interface like `IXService` if used).
  - Ensure DI registration matches the pattern used in this codebase.

---

## 6) Use meaningful names; consistent typing style

- **What it means**:
  - Prefer descriptive names (`dataReader` not `rdr`).
  - Use `var` **wherever possible**, or use explicit typing — but **don’t mix both in the same statement**.
  - Use collection initialization expression for readability: `List<T> items = [];`
- **Why it matters**: Reviewers optimize for maintainability; “clever abbreviations” and inconsistent style slow everyone down.
- **How to self-check**:
  - Scan for abbreviations, 1–2 letter names, ambiguous method names.
  - Ensure local variable declarations follow one consistent style per team guidance.
  - Prefer `List<T> items = [];` and similar modern C# patterns where allowed by the project.

---

## 7) Async conventions: `Async` suffix, return `IEnumerable<T>`, pass `CancellationToken`

- **What it means**:
  - Async methods must end with **`Async`**.
  - Multi-return methods should return **`IEnumerable<T>`**.
  - Async calls should accept and pass **`CancellationToken`** (don’t make it optional with a default).
  - Interfaces and repositories should return `IEnumerable<T>` for read-only to avoid accidental modifications.
- **Why it matters**: Consistent contracts, scalable cancellation, reduced coupling (callers shouldn’t rely on `List<T>` mutability).
- **How to self-check**:
  - Method names: `GetXAsync`, `FindXAsync`, etc.
  - Signatures: `Task<IEnumerable<T>>` for collections; `Task<T?>` for optional single.
  - Ensure every `await SomethingAsync(...)` passes `cancel` down to dependencies.

---

## 8) Use file-scoped namespaces

- **What it means**: Prefer `namespace My.App;` (file-scoped) instead of block-scoped namespaces.
- **Why it matters**: Less indentation, cleaner diffs, more consistent formatting.
- **How to self-check**:
  - New files follow file-scoped namespace style unless the repo is still on old style everywhere.

---

## 9) Prefer function-scoped `using`; avoid unnecessary `try/catch`

- **What it means**:
  - Use `using var x = ...;` (or `await using`) in the narrowest scope.
  - Don’t wrap code in `try/catch` unless you add value (translate to meaningful exception, add context, handle expected condition).
- **Why it matters**: “Catch-all” blocks hide bugs, swallow stack traces, and add noise.
- **How to self-check**:
  - If you added `try/catch`, answer: *what specific exception am I handling and why?*
  - Ensure you never catch `Exception` (see item 15).

---

## 10) Remove extra lines and unused imports

- **What it means**: Keep diffs clean: no unused `using`, no trailing whitespace, no random blank lines.
- **Why it matters**: Noisy diffs waste reviewer time and cause merge conflicts.
- **How to self-check**:
  - Run your formatter/analyzers.
  - Confirm there are no greyed-out usings / IDE hints about unused imports.

---

## 11) No typos in method/variable names

- **What it means**: Names must be spelled correctly, consistent, and searchable.
- **Why it matters**: Typos live forever in APIs; they reduce credibility and slow team navigation.
- **How to self-check**:
  - Use IDE rename/refactor tools (avoid manual rename that misses call sites).
  - Read names out loud once; you’ll catch more mistakes than you think.

---

## 12) Initialize model properties with non-null default values

- **What it means**: If a property is not meant to be nullable, give it a safe default (e.g., `string.Empty`, `[]`, new object where appropriate).
- **Why it matters**: Prevents `NullReferenceException` and communicates invariants to readers and serializers.
- **How to self-check**:
  - For non-nullable reference properties, ensure they are initialized or set via constructor.
  - For collections, initialize to empty rather than `null`.

---

## 13) Don’t create “Helper” methods

- **What it means**: Avoid dumping unrelated utilities into a generic “Helper” class/method.
- **Why it matters**: “Helper” becomes a junk drawer, breaks discoverability, and creates hidden dependencies.
- **How to self-check**:
  - If you’re about to name something `Helper`, stop and rename based on responsibility (e.g., `CustomerNameFormatter`, `OrderNumberParser`).
  - Prefer cohesive services, domain methods, or well-named small components.

---

## 14) Be judicious with extension methods (discuss with leads)

- **What it means**: Don’t introduce extension methods casually; they can make code harder to trace and cause naming collisions.
- **Why it matters**: Extensions affect discoverability and can become “global magic”.
- **How to self-check**:
  - Only add an extension if it will be reused broadly and is unambiguously correct.
  - If it changes important semantics, it likely shouldn’t be an extension.

---

## 15) Proper exception handling (don’t catch generic `Exception`)

- **What it means**: Catch specific exceptions you can actually handle/translate; otherwise let them bubble to global handling.
- **Why it matters**: Catching `Exception` usually hides defects and loses useful debugging signals.
- **How to self-check**:
  - Replace `catch (Exception)` with specific types (e.g., `SqlException`, `HttpRequestException`) only when you add meaningful action.
  - Ensure thrown exceptions are domain/service meaningful (not “something went wrong”).

---

## 16) Validation belongs in Domain objects

- **What it means**:
  - Domain objects contain validation logic.
  - Prefer **.NET 8 attribute-based** validation.
  - If attributes aren’t enough, implement `IValidatableObject`.
  - Only if needed beyond that, implement business/inference rules.
  - Don’t add validation methods in services/interfaces unless validation is explicitly offered “as a service” for a specific use case.
- **Why it matters**: Keeps invariants close to the data/behavior they constrain; avoids duplicated, inconsistent rules.
- **How to self-check**:
  - If you added `if (...) throw ...` checks in service for domain invariants, consider moving them into the domain object.
  - Ensure validations are test-covered and produce meaningful error messages.

---

## 17) Add proper XML documentation (no empty comments)

- **What it means**:
  - Public APIs (and project-required areas) should have XML docs.
  - No empty doc stubs.
  - You may use Copilot to generate docs but you must verify they match reality.
- **Why it matters**: Documentation is part of the deliverable; wrong docs are worse than no docs.
- **How to self-check**:
  - Search for `/// <summary>` blocks with no meaningful content.
  - Ensure docs describe intent/contract (inputs/outputs/exceptions), not implementation trivia.

---

## 18) Peer review with offshore lead (if required) + mandatory PR evidence

This section is explicitly marked mandatory in the checklist.

- **What it means**: Before PR approval, provide proof the code is safe, tested, and verifiable.
- **Why it matters**: Reviewers cannot (and will not) run everything locally for you; evidence prevents back-and-forth.
- **How to self-check (attach/ensure)**:
  - **SonarQube & CheckmarX** reports: passed, **no medium/high** issues.
  - **Postman/Swagger** screenshots: successful local call(s).
  - **Code coverage**: **>= 90% minimum**.
  - **Confluence** for every endpoint:
    - “Sample Request” and “Sample Response” filled properly.
    - Include required headers/prereqs/dependencies in “Sample Request”.
    - Use “NA” where truly not applicable.
    - Must be detailed enough that a tester can independently test the endpoint.

---

## 19) Unit test file regions + naming conventions

- **What it means**:
  - Add `#region` blocks for **List/Create/Update/Delete** in **unit test classes only** (not services/interfaces/other code).
  - Follow naming: `SomethingUnitTests.cs` or `SomethingIntegrationTests.cs`.
  - Coordinate/rebase with team members if needed to improve coverage.
- **Why it matters**: Test readability, easier verification by QA/testing team, consistent navigation.
- **How to self-check**:
  - Open your test file: do you see clear sections and consistent naming?
  - Coverage meets the minimum bar and includes meaningful cases (success, invalid, not found, edge cases).

---

## 20) “Your code is your responsibility” + review this document periodically

- **What it means**: Don’t rely on reviewers to catch basics; treat the checklist as your pre-flight.
- **Why it matters**: Senior developers reduce team load by preventing avoidable review churn.
- **How to self-check**:
  - Run your personal pre-PR routine (below).
  - Re-check the checklist monthly (or when you notice new rejections).

---

## A senior-level pre-PR routine (use every time)

- **Architecture**: Controller → Service → Repository boundaries respected; domain objects across tiers.
- **Contracts**: No `null` leaking from services/controllers; repository nullability explicit; collections non-null.
- **Async**: `Async` suffix; `CancellationToken` passed end-to-end.
- **Validation/Exceptions**: Domain validations; no `catch (Exception)`; meaningful custom exceptions.
- **Clean diff**: No unused usings, no formatting noise, no typos.
- **Docs**: XML docs present and accurate (no empty stubs).
- **Tests**: Coverage ≥ 90%; tests organized with required regions and naming.
- **Evidence**: SonarQube + CheckmarX passed; Swagger/Postman success screenshots; Confluence sample request/response filled.

