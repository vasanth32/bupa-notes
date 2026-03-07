# Bupa.Api.Apollo.Customer — Backend Onboarding (for .NET 8 / ASP.NET Core devs)

This repo contains the **Apollo Customer Domain API** (ASP.NET Core, **.NET 8**). The codebase is organized in a layered style:

- **Presentation**: HTTP API (controllers, middleware, Swagger, DI extension methods)
- **Application**: business services + interfaces
- **Infrastructure**: data access (Oracle), external calls (CRM), health checks
- **Domain**: models, config types, business rules, exceptions

> Note: The root `README.md` is still the default template. This document is the real “how to work in this repo”.

---

## What this API does (business view)

Right now the exposed API surface is small and focused:

- **Unsubscribe marketing communication preferences** (via CRM/Dynamics integration).

The main endpoint lives in:

- `src/Bupa.Api.Apollo.Customer/Presentation/Controllers/ServicesController.cs`

Route constants live in:

- `src/Bupa.Api.Apollo.Customer/Presentation/Constants/AppConstants.cs`

Current base route:

- `hi/apollo/customers`

---

## Solution layout

Solution file:

- `Bupa.Api.Apollo.Customer.sln`

Projects:

- `src/Bupa.Api.Apollo.Customer/Bupa.Api.Apollo.Customer.csproj` (**net8.0** web app)
- `tests/Bupa.Api.Apollo.Customer.UnitTests/Bupa.Api.Apollo.Customer.UnitTests.csproj`
- `tests/Bupa.Api.Apollo.Customer.IntegrationTests/Bupa.Api.Apollo.Customer.IntegrationTests.csproj`

Key top-level folders:

- `src/`
  - `Bupa.Api.Apollo.Customer/`
    - `Program.cs` (entrypoint; minimal hosting)
    - `Presentation/` (controllers, filters, extensions)
    - `Application/` (interfaces + services)
    - `Infrastructure/` (repositories, health checks, Oracle wallet init)
    - `Domain/` (models/config/exceptions/business rules)
- `tests/` (unit + integration)
- `config/` (Oracle wallet `.ora` files used in container)
- `devops/` (Azure pipeline + helm chart for AKS deployment)
- `Nuget.config` (private Azure DevOps feeds + nuget.org)

---

## How the app starts (runtime wiring)

Entry point: `src/Bupa.Api.Apollo.Customer/Program.cs`.

Almost all configuration/DI/middleware is composed via extension methods in:

- `src/Bupa.Api.Apollo.Customer/Presentation/Extensions/ApiBehaviourExtensions.cs`
- `src/Bupa.Api.Apollo.Customer/Presentation/Extensions/ServicesExtensions.cs`
- `src/Bupa.Api.Apollo.Customer/Presentation/Extensions/RepositoryExtensions.cs`
- `src/Bupa.Api.Apollo.Customer/Presentation/Extensions/HealthCheckExtensions.cs`

### Startup sequence (high level)

In `Program.cs` the app does, in order:

- **Configuration + Key Vault** (`ConfigureAppSettings`)
- **Kestrel settings** (`ConfigureCustomKestrel`)
- **Controllers + JSON** (`ConfigureController`)
- **Output formatter** (Bupa custom media type formatter)
- **Swagger** (`ConfigureSwagger`)
- **Oracle Wallet init** (`ConfigureOracleWalletAsync`)
- **Redis cache registration** (`ConfigureCache`)
- **AuthN/AuthZ** (`ConfigureAuthorization`)
- **Logging + exception handling** (Bupa packages)
- **App Insights** (`ConfigureAppInsights`)
- **HSTS** (`ConfigureHsts`)
- **Repositories + Services DI** (`ConfigureRepositories`, `ConfigureServices`)
- **Health checks** (`ConfigureHealthChecks`)
- **API versioning** (`ConfigureVersioning`)

Then in the middleware pipeline:

- Optional path base (`PATH_BASE`)
- Custom middlewares:
  - `LoggingEnrichmentMiddleware` (from `Bupa.HI.Logging.Package`)
  - `GlobalExceptionMiddleware` (from `Bupa.HI.ExceptionHandling.Package`)
- Routing
- `UseAuthentication()` then `UseAuthorization()`
- HSTS/HTTPS redirect (non-development)
- Swagger UI
- `MapControllers()`
- Health endpoints

---

## Request flow (for the main endpoint)

Controller:

- `Presentation/Controllers/ServicesController.cs`

Service:

- `Application/Services/ServicesService.cs`

Repository (CRM/Dynamics integration):

- `Infrastructure/Repositories/ServicesRepository.cs`

Typical flow for unsubscribe:

1. **Controller** validates model + enforces auth attributes:
   - `[AuthorizePermission(Roles.Delete)]`
   - `[AuthorizeSession]`
   - Requires `SessionToken` header (`AppConstants.ClaimKey.SessionToken`)
2. **Service** performs business validation (app id/email/topic ids) and checks cache.
3. **Repository**:
   - Calls CRM “contacts” using FetchXML (`fetchXml=...`)
   - If contact(s) found, constructs a CRM `$batch` request to create/update communication preferences

---

## AuthN/AuthZ (what you need to know)

Auth config happens in:

- `Presentation/Extensions/ApiBehaviourExtensions.cs` → `ConfigureAuthorization(...)`

Configuration comes from `appsettings*.json`:

- `AuthenticationSettings`:
  - `Issuers` (array)
  - `Audiences` (semicolon-separated string)
  - `IssuerSigningKeys` (semicolon-separated string)
  - `Authority`
- `InteractionSettings`:
  - `SessionTokenIssuer`
  - `SessionTokenIssuerSigningKey`
  - `ClosedSessionIdCacheKey`
- `Caching`:
  - used for “closed session id” validation and general caching

Authorization uses role strings defined in:

- `Presentation/Constants/AppConstants.cs` (`CustomerDomainAPI.Read/Update/Delete/Create`)

**Where to look when an endpoint returns 401/403**:

- Endpoint attributes in the controller (`AuthorizePermission`, `AuthorizeSession`)
- `ConfigureAuthorization(...)` wiring
- Your local `appsettings.*` values (audience/issuer/signing keys/session token values)

---

## External integrations

### CRM / Dynamics (PnP)

Outbound HTTP client is configured in:

- `Presentation/Extensions/ServicesExtensions.cs`

It binds `CrmApiSettingsPnP` and registers an `HttpClient` with custom handlers:

- `Presentation/Extensions/ClientHandler/TimeoutDelegatingHandler.cs`
- `Presentation/Extensions/ClientHandler/AdAppTokenDelegatingHandler.cs`

Token handler behavior:

- Uses MSAL confidential client (`AcquireTokenForClient`)
- Scope is derived from the configured client `BaseAddress` as `${BaseAddress}/.default`

Actual CRM calls are in:

- `Infrastructure/Repositories/ServicesRepository.cs`

### Redis (Azure Cache for Redis)

There are two related pieces:

- `ServicesExtensions.ConfigureCache(...)` registers `ICachingService` using a Redis connection string and `DefaultAzureCredential`
- `Application/Services/CachingServiceWrapper.cs` wraps Redis get/decrypt logic and generates **hashed** cache keys using:
  - `Caching:CacheKeySalt`
  - `Caching:CachePrefix`

The unsubscribe flow uses the wrapper (`ICachingServiceWrapper`) to read cached results.

### Oracle DB

Data access uses `Oracle.ManagedDataAccess.Client` with a lightweight “DbContext”:

- `Infrastructure/Repositories/DbContext/CustomerDbContext.cs`

Repositories that touch Oracle:

- `Infrastructure/Repositories/SessionAuditRepository.cs` (stored proc: `SP_INSERT_AUDIT_ENTRY`)
- `Infrastructure/Repositories/ServicesRepository.cs` (constructor receives db context + oracle helper, though most shown logic is CRM-based)

### Oracle Wallet (TLS/connection)

Wallet init runs at startup:

- `Infrastructure/OracleWallet/OracleWalletInitializer.cs`

It expects base64 wallet secrets split across multiple config values:

- `DatabaseConnectionStrings:CWallet1/2/3` → concatenated → `cwallet.sso`
- `DatabaseConnectionStrings:EWallet1/2/3` → concatenated → `ewallet.p12`
- `DatabaseConnectionStrings:WalletFolder` → target directory for the wallet files

The container path is set in `Dockerfile`:

- `ENV TNS_ADMIN=/app/wallet`
- Wallet `.ora` files are copied from `config/db/wallet` into the image publish output and then referenced at runtime.

---

## Health checks

Configured in:

- `Presentation/Extensions/HealthCheckExtensions.cs`

Endpoints:

- `GET /api/health` (full dependency check; UI response writer)
- `GET /api/health/isalive` (liveness)
- `GET /api/health/isAliveAndWell` (readiness-style response)

Checks included:

- **Oracle DB** (only if `DatabaseConnectionStrings:OracleHugoConnectionString` is set)
- **Memory** (threshold from `HealthChecks:Memory:Threshold`, default 1GB)

Swagger includes health endpoints via:

- `Presentation/Filters/HealthChecksDocumentFilter.cs`

---

## Configuration & environments

Main config files:

- `src/Bupa.Api.Apollo.Customer/appsettings.json` (baseline; many values are “keyvault-override” placeholders)
- `src/Bupa.Api.Apollo.Customer/appsettings.Development.json` (local overrides)
- `src/Bupa.Api.Apollo.Customer/appsettings.NoKV.json` (useful for “no key vault”/swagger export scenarios)
- `tests/.../appsettings.IntegrationTest.json` (integration test settings)

Key config sections you’ll touch most:

- `KeyVault:BaseUrl`
- `DatabaseConnectionStrings:*`
- `Caching:*`
- `AuthenticationSettings:*`
- `InteractionSettings:*`
- `CrmApiSettingsPNP` + `PnPConfigurationValues`
- `AzureMonitor:ConnectionString` (AppInsights/Azure Monitor)
- `ServiceBus:*` (present in config; usage may be added/extended later)

### Important security note

If any `appsettings.*.json` contains real secrets/keys, treat that as a security incident and move secrets to Key Vault / pipeline variables. This repo is already designed to pull secrets from Key Vault (`Azure.Extensions.AspNetCore.Configuration.Secrets`).

---

## How to run locally (Windows / VS / dotnet CLI)

### Visual Studio / IIS Express

Use the `HI.Customer.Api` profile from:

- `src/Bupa.Api.Apollo.Customer/Properties/launchSettings.json`

Default URLs:

- `http://localhost:5009`
- `https://localhost:7055`

It launches Swagger at `/swagger`.

### dotnet CLI

From repo root:

```bash
dotnet restore .\Bupa.Api.Apollo.Customer.sln --configfile .\Nuget.config
dotnet build .\Bupa.Api.Apollo.Customer.sln
dotnet run --project .\src\Bupa.Api.Apollo.Customer\Bupa.Api.Apollo.Customer.csproj
```

If you need a specific environment:

- Set `ASPNETCORE_ENVIRONMENT=Development` (default for VS profile)
- Or `ASPNETCORE_ENVIRONMENT=NoKV` if you want KeyVault to be skipped (depends on config)

---

## Tests

### Unit tests

Project:

- `tests/Bupa.Api.Apollo.Customer.UnitTests`

Run:

```bash
dotnet test .\tests\Bupa.Api.Apollo.Customer.UnitTests\Bupa.Api.Apollo.Customer.UnitTests.csproj
```

### Integration tests

Project:

- `tests/Bupa.Api.Apollo.Customer.IntegrationTests`

Harness:

- `CustomWebApplicationFactory` uses `WebApplicationFactory<Program>`
- `IntegrationTestFixture` chooses:
  - **in-memory TestServer** if `APIM:BaseUrl` is empty/invalid
  - **real external APIM** if `APIM:BaseUrl` is a valid URL
- Token generation is handled by `Common/TokenService.cs` using client credentials (`BFFJwtToken:*`)

Run:

```bash
dotnet test .\tests\Bupa.Api.Apollo.Customer.IntegrationTests\Bupa.Api.Apollo.Customer.IntegrationTests.csproj
```

---

## Deployment (Docker + Helm + Azure Pipelines)

### Docker

Dockerfile:

- `src/Bupa.Api.Apollo.Customer/Dockerfile`

Highlights:

- Builds/publishes on .NET 8
- Copies `config/db/wallet` into the image publish output
- Exposes port `8080` and sets `ASPNETCORE_URLS=http://+:8080`
- Includes an `exportswagger` stage which runs `Swashbuckle.AspNetCore.Cli` and writes `swagger.json`

### Helm (AKS)

Chart:

- `devops/deploy/helm/bupa-api-apollo-customer`

Defaults:

- `devops/deploy/helm/bupa-api-apollo-customer/values.yaml`

### Azure DevOps pipeline

Main pipeline:

- `devops/pipeline.yaml`

Stages include:

- Build
- Security scan
- Deploy per environment parameter
- Post-deploy integration test stage (non-prod)

---

## “Where do I change X?” quick map

- **Add/modify an endpoint**: `Presentation/Controllers/*Controller.cs`
- **Request/response DTOs**: `Domain/Models/*`
- **Business logic**: `Application/Services/*`
- **External calls (CRM/APIs)**: `Infrastructure/Repositories/*` + `Presentation/Extensions/ClientHandler/*`
- **Database access**: `Infrastructure/Repositories/*` + `Infrastructure/Repositories/DbContext/*`
- **Auth rules / roles**: `Presentation/Extensions/ApiBehaviourExtensions.cs` + `Presentation/Constants/AppConstants.cs`
- **Swagger**: `Presentation/Extensions/ApiBehaviourExtensions.cs` (SwaggerGen + UI)
- **Health checks**: `Presentation/Extensions/HealthCheckExtensions.cs`
- **Cache behavior/keys/encryption**: `Application/Services/CachingServiceWrapper.cs` + `Caching:*` config
- **Key Vault / secrets**: `Presentation/Extensions/ApiBehaviourExtensions.cs` → `ConfigureAppSettings`

---

## Known “gotchas” to be aware of (practical)

- **API versioning** is configured (`UrlSegmentApiVersionReader`) but current routes don’t obviously include a version segment. In practice, the **custom media type output formatter** and/or accept headers may matter more than URL segments—verify via Swagger/UI and existing consumers before changing versioning behavior.
- **Oracle wallet init** writes wallet files only if `DatabaseConnectionStrings:WalletFolder` exists on disk. In containers this is handled; on local machines you may need to create the folder or adjust config.
- **Private NuGet feeds**: builds need access to the Azure DevOps feeds listed in `Nuget.config`.

---

## Next steps for you (recommended first 60–90 minutes)

- Run the API locally and open Swagger.
- Read `ServicesController` → `ServicesService` → `ServicesRepository` top-to-bottom once.
- Skim `Presentation/Extensions/ApiBehaviourExtensions.cs` (it explains 80% of startup + auth/logging/telemetry behavior).
- Run unit tests, then integration tests in **in-memory** mode (leave `APIM:BaseUrl` empty).


