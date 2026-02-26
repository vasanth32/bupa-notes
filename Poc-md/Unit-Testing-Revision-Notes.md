## Why these tests are *unit tests* (OrderServiceTests)

### Why the repository is mocked
- The repository is an **external dependency** from the service’s point of view (in real life: DB, external API, cache).
- In unit tests we replace it with a mock (`IOrderRepository`) so the test focuses only on **OrderService logic**:
  - validation and branching (found vs not found)
  - exception behavior (wrap/handle failures)
  - expected calls (e.g., `AddAsync` invoked once)

### Why the controller is not tested here
- Controllers are mostly **HTTP plumbing** (routing, model binding, status codes).
- In unit tests we want **business rules** and decisions, which live in `OrderService`.
- Testing controllers here would force you to deal with ASP.NET Core concerns that aren’t the “unit” under test.

### Why no HTTP call is made
- Unit tests call `OrderService` methods directly (no `HttpClient`, no server, no `WebApplicationFactory`).
- This keeps tests focused and avoids failures caused by routing/config/serialization that are unrelated to service logic.

### Why this is fast
- No web host startup.
- No network stack / HTTP pipeline.
- No real IO (DB/files/network).
- Mocks return immediately, so tests typically run in **milliseconds**.

### What “isolation” means (practically)
- Each test controls the service’s dependencies and inputs completely.
- If the test fails, the cause is very likely **inside the service logic**, not the environment.
- Tests are deterministic: same inputs → same outputs.

---

## Enterprise mapping (Customer Domain API style)

In a typical enterprise Domain API (e.g., Customer Domain API), you often see:

- **Controller**: request/response + status codes
- **Service / Use-case layer**: business rules, orchestration, logging
- **Repository / Client layer**: DB access or downstream calls

Unit tests usually target the **service/use-case layer** and:

- Mock repositories/clients (DB adapters, HTTP clients, message bus publishers)
- Assert business rules (validation, branching, error handling)
- Verify logging and important side effects (where appropriate)

Integration tests then cover:

- HTTP pipeline + routing + serialization
- real DI wiring
- middleware behavior (exception handling, auth if enabled)
- optionally real infrastructure (DB/Testcontainers) or in-memory substitutes

---

## Why these tests are *integration tests* (WebApplicationFactory)

### Why `WebApplicationFactory` is used
- It spins up your ASP.NET Core app **in-memory for tests** (no manual `dotnet run`).
- It uses your real `Program` startup and **real DI container**, so your test is close to production wiring.
- It lets you test HTTP endpoints without needing a real network port (fast, repeatable).

### How the HTTP client is created
- `factory.CreateClient()` gives you an `HttpClient` that sends requests into the in-memory test server.
- You call real endpoints like `GET /orders/1` and `POST /orders` exactly like a consumer would.

### How the full pipeline is executed
Your integration tests execute the real chain:

- HTTP request
- routing + model binding
- controller action
- service call
- repository call (real fake repo registered in DI)
- serialization back to JSON + HTTP status code

### Difference between this and unit tests
- Unit tests call `OrderService` directly and mock dependencies.
- Integration tests call the API via HTTP and validate the **system behaves correctly end-to-end**.

### What parts are real vs mocked (in this POC)
- **Real**: routing, controllers, DI wiring, filters (if any), JSON serialization, service logic
- **Not mocked**: the repository (we use the real `FakeOrderRepository` implementation)
- **No external infra**: there is no real database; that’s why this is a “lightweight integration test”

### Comparison table (unit vs integration)

| Aspect | Unit tests (this POC) | Integration tests (this POC) |
| --- | --- | --- |
| Entry point | `OrderService` method call | HTTP request (`HttpClient`) |
| Host | none | in-memory ASP.NET Core host |
| Dependencies | mocked (`IOrderRepository`) | real app DI wiring |
| What you validate | business rules + branching | routing + serialization + HTTP codes + end-to-end behavior |
| Speed | fastest | slower than unit, still fast |
| Typical failures | logic bug in unit | wiring/config + endpoint behavior |

---

## Visual flow comparison (simple diagrams)

### Unit test flow (service in isolation)

```text
Test
  ↓
OrderService
  ↓
Mock IOrderRepository (Moq)
```

What this means:

- You’re testing **business logic** (validation + branching + error handling)
- Dependencies are **controlled** (mocked)
- No ASP.NET Core host, no routing, no serialization

### Integration test flow (real HTTP pipeline in-memory)

```text
Test (HttpClient from WebApplicationFactory)
  ↓
ASP.NET Core pipeline (routing + model binding + DI)
  ↓
OrderController
  ↓
OrderService
  ↓
FakeOrderRepository (real implementation)
  ↓
JSON serialization + HTTP status code
```

What this means:

- You’re validating the **end-to-end behavior** of the API
- Most of the system is “real” (startup/DI/controller/service/serialization)
- Still fast because it’s not a real network deployment

---

## How this relates to Kubernetes / AKS (production mental model)

### Unit tests vs AKS

Unit tests **do not care** about AKS or Kubernetes. They are pure code checks:

- They run the same on your laptop, CI agent, or anywhere else.
- They won’t catch issues like wrong ports, missing env vars, broken ingress paths, readiness probes, etc.

### Integration tests vs AKS

Your `WebApplicationFactory` integration tests are “**AKS-like**” in the sense that they test the **HTTP behavior**, but they still run locally:

- They catch API contract issues early (routes, status codes, JSON shape).
- They still won’t cover the full AKS runtime environment.

### Full “AKS reality” (system/e2e tests)

In a real AKS environment, the request path typically looks like:

```text
Client
  ↓
Ingress / API Gateway
  ↓
Kubernetes Service
  ↓
Pod (OrderApi container)
  ↓
Downstream dependencies (DB / Service Bus / other APIs)
```

So:

- **Unit tests** protect business logic and edge cases quickly.
- **Integration tests** protect the API surface (HTTP pipeline) and wiring.
- **E2E/system tests** protect the real deployment path (ingress/service/pods + real dependencies).

---

## Short revision notes (interview-ready)

- **Unit tests** = test one class/component in isolation by mocking dependencies.
- Mock the **repository** because it represents IO and would slow/flakify tests.
- Don’t unit test the **controller** unless it has non-trivial logic; keep it thin.
- No HTTP in unit tests; HTTP belongs to **integration tests**.
- Unit tests should be **fast, deterministic, focused**.

