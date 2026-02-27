# PR Code Review Checklist (for Devs)

Source: `DT-PR Code Review Checklist (for Devs)-260226-055250.pdf`

> Note: This Markdown file is a cleaned-up formatting of text extracted from the PDF.  
> The intent/meaning is preserved; only formatting/structure was improved.

## Checklist

### 1) Don’t copy/paste legacy code
- Don’t **COPY and PASTE** code from legacy codebases.
- Your job is to upgrade to new standards **without losing semantic meaning**.
- If you have copied code, make necessary changes **before raising a PR**.

**Preferred structure (3-tier):**
- **Controller → Service → Repository**
- A **Controller** can only talk to a **Service** (or multiple **Services**).
- A **Service** can talk to its own **Repository** or call other **Services**.
  - Avoid dependency on **multiple repositories** inside a single service.
  - Services can throw exceptions for invalid situations (and other services can leverage them).
- A **Repository** is a simple data store for retrieving/saving/deleting entities:
  - No business logic
  - No complicated data management
  - May depend on other repositories in some cases to avoid duplication

### 2) Avoid `null` (and unnecessary null checks)
- Avoid `null` throughout the codebase.
- **Only exception**: repository methods returning a **single value** that may not exist.
  - In that case, the signature should clearly indicate nullable return.
  - For multiple values: return an **empty collection** (always).

Example:

```csharp
Task<Customer?> FindCustomerAsync(long customerId, CancellationToken cancel);
```

Notes:
- Return type is `Customer?` (nullable) and **not** `Customer`.
- **Repositories never throw exceptions.**
- **Services and Controllers never return `null`**; they throw customised and meaningful exceptions instead.

### 3) Use Domain objects between tiers (not DTOs/entities)
- Controllers, Services, and Repositories communicate via **Domain objects** only (not DTOs or entities).
- Repository returns a fully constructed **Domain object** (if it exists in DB).
- Services and Controllers work with these objects and return them back to the client.
- If not all properties are required in the response, use **projections** (anonymous objects) with a subset of properties.

Projection example:

```csharp
var customers = await customerService.GetCustomersAsync(skip, count, cancel);
var firstAndLastNames = customers.Select(c => new
{
    c.FirstName,
    c.LastName
});
```

### 4) Service base + marker interface
- Service classes should inherit from **`ServiceBase`** and implement **`IService`**.

### 5) Meaningful naming + consistent style
- Use meaningful variable/method names.
  - Example: `dataReader` and not `rdr`.
- Use `var` wherever possible **or** `TypeName variableName = new { }`, but **not both together**.
- Prefer collection initialization expression:

```csharp
List<T> items = [];
```

### 6) Async conventions
- Async methods should end with the **`Async`** suffix.
- Methods returning multiple values should return **`IEnumerable<T>`**.
- Always pass a **`CancellationToken`** to async methods (don’t make it optional with default values).

Example:

```csharp
Task<IEnumerable<Customer>> GetCommunicationPreferencesAsync(long customerId, CancellationToken cancel);
```

Also:
- Interfaces and repositories should return `IEnumerable<T>` for Get/read-only methods to avoid accidental modifications.

### 7) File-scoped namespaces
- Use file-scoped namespaces to reduce indentation.

### 8) Use scoped `using` and avoid unnecessary try/catch
- Always use function-scoped `using` statements.
- Avoid unnecessary `try...catch` blocks to improve readability.

### 9) Clean code hygiene
- Remove extra lines and unused imports.

### 10) No typos
- Ensure there are no typos in method or variable names.

### 11) Initialise model properties
- Always initialise model properties with default values that aren’t nullable.

### 12) No helper methods
- Don’t create helper methods as they are unnecessary.

### 13) Extension methods: be judicious
- Be judicious when creating extension methods; discuss with leads prior.

### 14) Exception handling
- Implement proper exception handling.
- Don’t catch generic `Exception`.

### 15) Validation belongs in the domain
- Validate all domain objects; only domain objects should contain validation logic.
- Use .NET 8 attribute-based validation where possible.
- If that isn’t sufficient, implement `IValidatableObject`.
- If validation needs exceed that, implement Business or Inference rules.
- Don’t add validation methods into services/interfaces unless there is a specific use case (validation as a service to other services).

### 16) XML documentation
- Add proper XML documentation.
- Populate empty comments.
- Don’t raise PRs with empty comments.
- Use meaningful comments that match what methods actually do.

### 17) Reviews + mandatory evidence
- Get an internal peer review with your respective offshore lead if required.
- **[MANDATORY]** Attach SonarQube & CheckmarX reports with passed status (no medium or high issues).
- **[MANDATORY]** Attach Postman or Swagger screenshots showing successful local result.
- **[MANDATORY]** Code coverage should be **≥ 90%** (bare minimum).
- **[MANDATORY]** “Sample Request” & “Sample Response” sections must be populated for every endpoint on its confluence page:
  - Include headers and endpoint requirements in “Sample Request”
  - If not applicable, use “NA” where appropriate
  - PRs will not be approved unless this is to the right standard (tester can independently test)

### 18) Unit test regions and naming
- Add regions for **List**, **Create**, **Update**, **Delete** methods in unit test class files **only**
  - Not in services, interfaces, or other places
- Follow naming convention for test file names:
  - `MedicareCardUnitTests.cs`
  - `MedicareCardIntegrationTests.cs`
- Liaise with other team members if needed and rebase to improve code coverage.

### 19) Responsibility
- Ensure you follow all guidelines before raising a PR; your code is your responsibility.

### 20) Review periodically
- Review this document periodically for updates.

