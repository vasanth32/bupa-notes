# 🚀 Apollo.Customer.Mock – Enterprise Acceleration Blueprint

## 🎯 Final Goal

Build a production-style .NET 8 microservice using:

* Clean Architecture
* SQL Server
* EF Core 8
* 90%+ Code Coverage
* SonarQube Clean
* Security Clean (Checkmarx-style)
* Full PR Checklist Compliance

This project simulates a real enterprise backend like **Api.Apollo.Customer**.

---

# 🧱 Phase 1 – Create Enterprise Skeleton (Day 1)

## 🏗 Architecture Structure

```
Apollo.Customer.Api              → Presentation Layer
Apollo.Customer.Application      → Services Layer
Apollo.Customer.Domain           → Domain Models + Rules
Apollo.Customer.Infrastructure   → EF Core + SQL Server
Apollo.Customer.UnitTests
Apollo.Customer.IntegrationTests
```

## Architecture Rules

* Controller → Service → Repository only
* Service inherits from `ServiceBase` and implements `IService`
* Repository NEVER throws exceptions
* Repository returns nullable only for single entity
* Return `IEnumerable<T>` for collections
* Use file-scoped namespaces
* Async methods must have `Async` suffix
* Pass `CancellationToken` everywhere (mandatory)
* Initialise model properties with non-null defaults
* Add XML documentation
* No helper methods
* Domain contains validation logic
* Use BusinessRuleException only if attribute validation insufficient

---

# 🧠 Cursor Prompt – Generate Full Solution

```
Create a .NET 8 Clean Architecture Web API solution named Apollo.Customer.Mock.

Architecture rules:
- Controller → Service → Repository only
- Service inherits from ServiceBase and implements IService
- Repository never throws exceptions
- Repository returns nullable only for single entity
- Return IEnumerable<T> for collections
- Use file-scoped namespaces
- Async methods must have Async suffix
- Pass CancellationToken everywhere (mandatory)
- Initialise model properties with default values
- Add XML documentation
- No helper methods
- Domain contains validation logic
- Use SQL Server with EF Core 8

Customer fields:
Id, FirstName, LastName, Email, IsSubscribed, CreatedAt

Generate complete code + explain checklist compliance.
```

---

# 🧠 Phase 2 – Implement Endpoints (Day 1–2)

## Required Endpoints

* POST /customers
* GET /customers/{id}
* GET /customers (pagination)
* PUT /customers/{id}/marketing-preference

## Cursor Prompt

```
Add the following endpoints:

1) Create Customer
2) Get Customer By Id
3) Get Customers (pagination)
4) Update Marketing Preference

Ensure:
- Domain validation only in Domain
- Repository never throws
- Service converts null to meaningful exception
- Controller never returns null
- Use ?? throw syntax
- IEnumerable for list
- Add unit tests (success + failure)
- Add integration tests
- Coverage >= 90%

Explain checklist compliance after implementation.
```

---

# 🧪 Phase 3 – Testing Discipline (Day 2)

Checklist requires:

* Regions only in unit test files
* Proper naming conventions
* ≥ 90% coverage
* Success + failure path coverage
* No generic exceptions

## Cursor Prompt – Validate Tests

```
Review all unit tests and ensure:

- Regions used only in unit test files
- Success path covered
- Failure path covered
- SaveAsync not called when exception occurs
- No generic Exception used
- Proper naming convention
- Coverage >= 90%

Fix anything missing.
```

---

# 📊 Phase 4 – SonarQube Practice (Day 3)

## Install SonarQube

```bash
docker run -d --name sonarqube -p 9000:9000 sonarqube:lts
```

Access:

```
http://localhost:9000
admin / admin
```

## Cursor Prompt – Configure Sonar

```
Explain step-by-step:

1) Install SonarQube locally using Docker
2) Use dotnet sonarscanner for .NET 8
3) Integrate coverage reports
4) Exclude test projects
5) Fail build if coverage < 90%
6) Fix common Sonar smells:
   - Missing XML docs
   - Cognitive complexity
   - Async without cancellation token
   - Duplicate code
   - Unused imports
```

---

# 🔐 Phase 5 – Security Simulation (Day 4)

Simulate Checkmarx findings.

## Cursor Prompt – Inject Vulnerabilities

```
Intentionally introduce:

1) Hardcoded connection string
2) SQL injection using FromSqlRaw
3) Returning stack trace to client
4) Missing input validation
5) Generic catch(Exception)

Then:
- Explain why this is dangerous
- Explain how Checkmarx would detect it
- Fix vulnerabilities
- Show secure version
```

---

# 📚 Phase 6 – PR Checklist Deep Understanding (Day 4–5)

You must internalise the PR checklist rules.

## Cursor Prompt – Rule-by-Rule Breakdown

```
For each PR checklist rule:

1) Explain why it exists
2) Show bad example
3) Show correct example
4) Explain architectural reasoning
5) Explain how Sonar enforces it
6) Explain how security tools enforce it

Format as senior interview notes.
```

---

# 📝 Phase 7 – Generate Master Learning Notes (Final Step)

## Cursor Prompt – Generate Study Notes

```
Generate structured study notes including:

- Clean Architecture explained simply
- Why repository never throws
- Why null only allowed at repository boundary
- Why service converts null to exception
- Why domain owns validation
- Why cancellation token is mandatory
- How to design proper unit tests
- How to design integration tests
- How to reach 90% coverage
- How Sonar enforces discipline
- How Checkmarx enforces security
- How to prepare PR with checklist compliance
- Common reviewer rejection reasons

Make it suitable for senior backend interview preparation.
```

---

# ⚡ Fast Execution Plan (Optimised)

### Day 1

* Generate solution
* Implement endpoints

### Day 2

* Write all tests
* Reach 90% coverage

### Day 3

* Run Sonar
* Fix smells

### Day 4

* Inject security flaws
* Fix them

### Day 5

* Deep checklist review
* Generate notes
* Mock PR review yourself

---

# 🗄 SQL Server Setup (Recommended)

Use Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Pass1" \
-p 1433:1433 --name sqlserver \
-d mcr.microsoft.com/mssql/server:2022-latest
```

This matches enterprise deployment style.

---

# 🎯 Final Outcome

After completing this POC, you will confidently say:

* I implemented Clean Architecture in .NET 8
* I followed enterprise PR checklist standards
* I ensured ≥90% coverage
* I ran SonarQube and resolved all issues
* I simulated Checkmarx findings and fixed vulnerabilities
* I understand architectural reasoning behind every rule

---

**End of Document** 🚀
