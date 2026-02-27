# Apollo.Customer.Mock — Progress (Beginner-friendly)

## 📍 Where the project lives
- **Solution root**: `d:\HexawareProjects\BUPA\Poc-Projects\Apollo.Customer.Mock`
- **Solution file**: `Apollo.Customer.Mock.sln`

## ✅ Current status (what is finished)
- ✅ .NET 8 solution created
- ✅ Clean Architecture layers created (**Domain / Application / Infrastructure / WebApi**)
- ✅ Endpoints implemented:
  - Create Customer
  - Get Customer By Id
  - Get Customers (pagination)
  - Update Customer
  - Update Marketing Preference
  - Delete Customer
- ✅ EF Core 8 + SQL Server wired (DbContext + DI)
- ✅ Migration created + applied (DB + tables created)
- ✅ Unit tests + Integration tests added
- ✅ Coverage threshold enforced (**≥ 90%**)

## 🧠 Big picture (how your API works)
When a request comes to your API (example: `GET /api/customers/{id}`), the flow is:

1. **Controller** (Web API layer) receives the HTTP request 🌐  
2. Controller calls a **Service** (Application layer) 🧩  
3. Service calls a **Repository** (Infrastructure layer) 🗄️  
4. Repository talks to **SQL Server using EF Core** 🗃️  
5. The result flows back up and the Controller returns an HTTP response ✅

This separation is useful because each layer has 1 job:
- **Controller**: HTTP only (routes, query/body, status codes)
- **Service**: application/business decisions + meaningful exceptions
- **Repository**: database access only (no business rules)

## 🧱 Layers and what you built inside each one

### 🟦 Domain (`Apollo.Customer.Mock.Domain`)
This is the **core business layer** (your “truth”).

**What’s inside:**
- ✅ `Customer` entity:
  - Fields: `Id`, `FirstName`, `LastName`, `Email`, `IsSubscribed`, `CreatedAt`
- ✅ **Validation lives here only**:
  - `Customer` constructor + `Customer.Update(...)` validate values
  - If invalid → throws `DomainValidationException`

**Domain input models (used by API):**
- `CustomerProfile`: create/update input
- `PaginationRequest`: `skip` + `count` paging input
- `MarketingPreference`: marketing preference input (`IsSubscribed`)

### 🟩 Application (`Apollo.Customer.Mock.Application`)
This is the **use-case layer** (what the app can do).

**What’s inside:**
- ✅ `IService` + `ServiceBase` (checklist requirement)
- ✅ `ICustomerService` + `CustomerService`
- ✅ `ICustomerRepository` abstraction

**Very important checklist rule implemented:**
- Repository may return `null` for “not found” (single entity only).
- Service must convert that `null` into a **meaningful exception**, using `?? throw`.

So the service throws:
- `CustomerNotFoundException` → becomes **404** in the API
- `CustomerPersistenceException` → becomes **500** in the API

### 🟨 Infrastructure (`Apollo.Customer.Mock.Infrastructure`)
This is the **EF Core + SQL Server** layer.

**What’s inside:**
- ✅ `ApolloCustomerMockDbContext`
- ✅ `CustomerRepository` (implements `ICustomerRepository`)

**Repository never throws (checklist):**
- It catches errors and returns safe defaults:
  - Lists → empty `IEnumerable<Customer>`
  - Single get → `null`
  - Update/Delete → `false`

### 🟥 Web API (`Apollo.Customer.Mock.WebApi`)
This is the **HTTP layer** (routes + responses).

**What’s inside:**
- ✅ `CustomersController` (the endpoints)
- ✅ `ExceptionHandlingMiddleware` (global error → HTTP mapping)
- ✅ DI wiring in `Program.cs`

**Global exception mapping:**
- `DomainValidationException` → **400 Bad Request**
- `CustomerNotFoundException` → **404 Not Found**
- `CustomerPersistenceException` → **500 Internal Server Error**

This is why controllers don’t have try/catch everywhere.

## 🌐 API endpoints (what you can call)

### ➕ Create Customer
- **POST** `/api/customers`
- Body: `CustomerProfile`

### 🔎 Get Customer By Id
- **GET** `/api/customers/{id}`

### 📄 Get Customers (pagination)
- **GET** `/api/customers?skip={skip}&count={count}`
- Example: `/api/customers?skip=0&count=10`

### ✏️ Update Customer
- **PUT** `/api/customers/{id}`
- Body: `CustomerProfile`

### 📣 Update Marketing Preference
- **PUT** `/api/customers/{id}/marketing-preference`
- Body: `MarketingPreference`

### 🗑️ Delete Customer
- **DELETE** `/api/customers/{id}`

## 🗄️ Database + Migrations (EF Core)
✅ Migration created and applied:
- Migration: `InitialCreate`
- Database: `ApolloCustomerMock`
- Table: `Customers`
- Unique index: `Email`

**Connection string:**
- File: `src/Apollo.Customer.Mock.WebApi/appsettings.json`
- Key: `ConnectionStrings:SqlServer`
- Target: SQL Express `VASANTH\SQLEXPRESS`

## 🧪 Testing (what and why)

### ✅ Unit tests (`Apollo.Customer.Mock.UnitTests`)
- Purpose: test service/domain behavior (success + failure cases)
- Required regions are present (to help reviewers/testing team):
  - `#region List`, `#region Create`, `#region Update`, `#region Delete`

### ✅ Integration tests (`Apollo.Customer.Mock.IntegrationTests`)
- Purpose: test the API like a real client (HTTP calls)
- Uses `WebApplicationFactory` and EF Core **InMemory** DB so tests are:
  - fast ⚡
  - isolated ✅
  - do not require SQL Server ✅

## 📊 Coverage (≥ 90%)
Coverage enforcement is configured in:
- `tests/coverlet.runsettings`

Run with enforced threshold:

```bash
dotnet test .\Apollo.Customer.Mock.sln -c Release --collect:"XPlat Code Coverage" --settings .\tests\coverlet.runsettings
```

## ▶️ How to run the API locally
From the solution root:

```bash
dotnet run --project .\src\Apollo.Customer.Mock.WebApi
```

Swagger is enabled in Development.

## ✅ Checklist compliance (why we are aligned)
- ✅ Controller → Service → Repository only
- ✅ Service inherits `ServiceBase` and implements `IService`
- ✅ Repository never throws
- ✅ Repository returns nullable only for single entity (`GetByIdAsync`)
- ✅ Service converts null to meaningful exception (**uses `?? throw`**)
- ✅ Controller never returns null
- ✅ `IEnumerable<T>` for list endpoints
- ✅ Async methods end with `Async`
- ✅ `CancellationToken` passed everywhere
- ✅ Model properties initialized with defaults (`string.Empty`, etc.)
- ✅ XML documentation present
- ✅ No helper methods added
- ✅ Domain contains validation logic
- ✅ SQL Server + EF Core 8 configured; migration applied
- ✅ Unit tests + Integration tests added
- ✅ Coverage threshold enforced (≥ 90%)

## 🧼 SonarQube (code quality) — how to start and use next time

### ✅ What we achieved
- ✅ SonarQube project created: `Apollo.Customer.Mock`
- ✅ First analysis uploaded successfully
- ✅ Quality Gate: **Passed**
- ℹ️ Sonar showed a few **warnings** (mainly because this SonarQube version is EOL / older)

### 🐳 Start SonarQube locally (Docker)

#### 1) Start Docker Desktop
- Make sure **Docker Desktop** is running (otherwise `docker` commands will fail).

#### 2) Start SonarQube container
On this machine, **port 9000 is reserved by Windows (PID 4 / System)**, so we use **9001** on the host.

- If container already exists:
