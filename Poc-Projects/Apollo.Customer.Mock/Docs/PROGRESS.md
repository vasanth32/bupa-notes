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

##### Option A (recommended): create a persistent SonarQube instance (keeps your projects/tokens)
Run these once (creates volumes) and reuse forever:

```bash
docker run -d --name sonarqube -p 9001:9000 -v sonarqube_data:/opt/sonarqube/data -v sonarqube_extensions:/opt/sonarqube/extensions -v sonarqube_logs:/opt/sonarqube/logs sonarqube:lts
```

Then open:
- `http://localhost:9001`

##### Option B (quick/temporary): no volumes (data is lost if you delete the container)

```bash
docker run -d --name sonarqube -p 9001:9000 sonarqube:lts
```

### 🔁 Start / Stop / Restart SonarQube next time

#### ▶️ Start (if it already exists)

```bash
docker start sonarqube
```

#### ⏹️ Stop

```bash
docker stop sonarqube
```

#### 🔄 Restart (stop + start)

```bash
docker restart sonarqube
```

#### ✅ Confirm it’s running + which port

```bash
docker ps --filter "name=sonarqube"
```

### 🔐 Login + tokens (so scans can authenticate)

#### Default login (fresh install)
- **Username**: `admin`
- **Password**: `admin`

You will be forced to change the password after first login.

#### If you forgot the password 😅
SonarQube Community (Docker) doesn’t have a simple “forgot password” flow by default. The easiest reset is to **delete the container + volumes** and start fresh.

⚠️ This deletes all SonarQube data (projects, tokens, history).

```bash
docker rm -f sonarqube
docker volume rm sonarqube_data sonarqube_extensions sonarqube_logs
```

Then run the “Option A” command again and login with `admin/admin`.

#### Create a token (recommended for scanning)
In the SonarQube UI:
- Click your avatar (top-right) → **My Account**
- Go to **Security**
- Under **Tokens**:
  - Name: `apollo-local`
  - Token type: **Project Analysis Token** (best for scanning this one project)
  - Generate → copy the `sqp_...` token (save it somewhere secure)

### 🧪 Run analysis for this solution (SonarScanner for .NET)
Run these commands from solution root:
`d:\HexawareProjects\BUPA\Poc-Projects\Apollo.Customer.Mock`

> Replace `<TOKEN>` with your `sqp_...` token.

```bash
dotnet tool install --global dotnet-sonarscanner

dotnet sonarscanner begin /k:"Apollo.Customer.Mock" /d:sonar.host.url="http://localhost:9001" /d:sonar.token="<TOKEN>"
dotnet build .\Apollo.Customer.Mock.sln -c Release
dotnet sonarscanner end /d:sonar.token="<TOKEN>"
```

Then refresh the project page in SonarQube to see results.

### 🧯 Troubleshooting (common issues)

#### ❌ “Bind for 0.0.0.0:9000 … forbidden” / “ports are not available”
- ✅ Use **9001:9000** instead of 9000:9000 (because PID 4 uses 9000 on this machine).

#### ❌ Browser says “This page can’t be found” / “ERR_EMPTY_RESPONSE”
- Check container is running: `docker ps --filter "name=sonarqube"`
- Wait 1–2 minutes after starting; SonarQube takes time to boot.
- Check logs: `docker logs -f sonarqube`

#### ❌ SonarScanner says “Not authorized”
This usually means one of these:
- token is wrong / expired / copied incorrectly
- using the wrong token type (use **Project Analysis Token** or **User Token**)
- you forgot to pass the token in **both** `begin` and `end`
- wrong host URL (ensure it is `http://localhost:9001`)

### ⚠️ “You’re running a version of SonarQube that is no longer active”
This is a **support/EOL warning** from SonarSource. It does not always block scanning, but you should upgrade when possible.

**Simple upgrade approach (local Docker):**
- Stop + remove container, then run again using `sonarqube:lts` (or a newer tag your team approves).
- If you used persistent volumes, keep them if the upgrade path supports it; otherwise recreate clean.

---

## 🔒 Checkmarx (security scanning) — how to use with this project
Checkmarx is a **security scanner** (SAST) that looks for vulnerabilities in source code (SQL injection, insecure deserialization, weak crypto, etc.). Think of it as:
- **SonarQube** = code quality + maintainability + some security rules 🧼
- **Checkmarx** = security-first scanning 🔐

### ✅ What you should scan in this repo
- ✅ `src/` (main code — most important)
- ✅ `tests/` (optional; scan if your org policy requires)
- ❌ Don’t scan generated folders: `bin/`, `obj/`, `.sonarqube/`, `TestResults/` (already in `.gitignore`)

### 🧭 Two ways Checkmarx is usually used (choose based on your company setup)

#### Option 1: CI/CD scan (most common in enterprises) 🏗️✅
This means **you don’t run it manually**. The pipeline scans when you push code.

Typical flow:
1. You push PR/branch to repo
2. Pipeline runs Checkmarx step
3. If findings are high severity, pipeline may fail (“quality gate”)
4. You fix issues and re-run pipeline

What you need to do for this project:
- **Ask your DevSecOps/team** which Checkmarx product you use:
  - **Checkmarx One (CxOne)** (modern SaaS) or
  - **CxSAST (on-prem)** (older style)
- Get the required pipeline values (usually provided as secret variables):
  - server URL / tenant
  - API key / client id-secret / token
  - project name (use `Apollo.Customer.Mock`)
- Add/verify pipeline step in your CI (Azure DevOps / Jenkins / GitHub Actions)

#### Option 2: Local scan (only if your org allows it) 💻
You run a CLI tool that sends code to Checkmarx and downloads results.

**Important:** The exact commands depend on which product your org uses.
- For **Checkmarx One** you usually use the **Cx CLI** (`cx ...`)
- For **CxSAST** you may use different tooling/CLI

Generic example pattern (CxOne-style):

```bash
# 1) authenticate (method depends on your org)
# 2) scan this folder (solution root)
cx scan create --project-name "Apollo.Customer.Mock" --source "."
```

### 🧯 What to do after a scan (how to work with findings)
1. **Open results** (web portal or exported report)
2. Sort by **Severity** (Critical/High first)
3. For each finding:
   - Understand the vulnerable code path
   - Fix it (or justify false positive with a comment/ticket if policy allows)
4. Re-scan until the project passes your org’s policy

### 🧩 Where this fits in our checklist mindset
- **Repository layer**: make sure DB calls are parameterized and safe (EF Core helps here) 🗄️✅
- **Web API layer**: validate input + don’t leak internal errors (we already map exceptions globally) 🌐✅
- **Secrets**: never commit tokens/connection strings; keep secrets in env/user-secrets 🔑✅
