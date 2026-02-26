# PR Checklist POC — Learning Notes

This POC is intentionally small, but structured to make code-review checklist items **observable in code**.

## Structure

- `src/ChecklistPoc.Api`: ASP.NET Core Web API
- `tests/ChecklistPoc.UnitTests`: xUnit + Moq tests

## 3-tier boundaries (Controller → Service → Repository)

### Controller

- **Allowed**: HTTP concerns only (routing, model binding, calling a service, returning results)
- **Forbidden**: business rules, persistence decisions, exception→HTTP mapping logic
- **Examples**: `src/ChecklistPoc.Api/Controllers/OrdersController.cs`

### Service

- **Allowed**: business rules, orchestration across repositories, deciding which **domain exceptions** to throw
- **Forbidden**: HTTP concerns (status codes, `ActionResult`), direct persistence implementation
- **Examples**: `src/ChecklistPoc.Api/Services/OrderService.cs`, `src/ChecklistPoc.Api/Exceptions/*`

### Repository

- **Allowed**: data access only (store/retrieve domain objects), no business decisions
- **Forbidden**: validation, state-transition rules, throwing domain exceptions
- **Examples**: `src/ChecklistPoc.Api/Repositories/InMemoryOrderRepository.cs`

**Test that enforces this boundary**

- `CreateOrderAsync_WhenDraftInvalid_ThrowsAndDoesNotCallRepository` in `tests/ChecklistPoc.UnitTests/Services/OrderServiceTests.cs` proves that validation happens in the **service** (repo is never called). If someone moved validation into the repo, this test would start failing.

## “Async everywhere” + `Async` suffix

- Controller actions: `OrdersController.CreateAsync`, `GetByIdAsync`, `ListAsync`, `CancelAsync` in `src/ChecklistPoc.Api/Controllers/OrdersController.cs`
- Service API: `IOrderService` + `OrderService` methods in `src/ChecklistPoc.Api/Services/*`
- Repository API: `IOrderRepository` methods in `src/ChecklistPoc.Api/Repositories/IOrderRepository.cs`

## Cancellation token passed end-to-end (no optional defaults)

- `OrdersController` receives `CancellationToken cancellationToken` and passes it into `IOrderService`.
- `OrderService` passes the token into `IOrderRepository`.
- `IOrderRepository` requires the token on every method.

Files:

- `src/ChecklistPoc.Api/Controllers/OrdersController.cs`
- `src/ChecklistPoc.Api/Services/IOrderService.cs`
- `src/ChecklistPoc.Api/Repositories/IOrderRepository.cs`

## Repository return conventions (nullability rules)

- Single-get “may not exist”: `Task<Order?> GetByIdAsync(...)` in `src/ChecklistPoc.Api/Repositories/IOrderRepository.cs`
- Collections never null: `Task<IReadOnlyList<Order>> ListAsync(...)` returns `[]` on failure in `src/ChecklistPoc.Api/Repositories/InMemoryOrderRepository.cs`

## “Repositories never throw”

- `InMemoryOrderRepository` wraps operations in `try/catch` and returns safe values (`null`, `[]`, `false`) instead of throwing.

File:

- `src/ChecklistPoc.Api/Repositories/InMemoryOrderRepository.cs`

## “Services/controllers never return null” + “services throw meaningful custom exceptions”

- Service throws **custom exceptions**:
  - `ValidationException` for invalid input (`src/ChecklistPoc.Api/Exceptions/ValidationException.cs`)
  - `OrderNotFoundException` for missing orders (`src/ChecklistPoc.Api/Exceptions/OrderNotFoundException.cs`)
  - `OrderConflictException` for conflicts (`src/ChecklistPoc.Api/Exceptions/OrderConflictException.cs`)
- Controller always returns `ActionResult<T>` with concrete values (never `null`).

Files:

- `src/ChecklistPoc.Api/Services/OrderService.cs`
- `src/ChecklistPoc.Api/Controllers/OrdersController.cs`

## Exception → HTTP mapping via middleware (not controllers)

- `ExceptionMappingMiddleware` maps domain exceptions to HTTP status codes + `ProblemDetails` JSON.
- Unit tests validate the mapping behavior.

Files:

- `src/ChecklistPoc.Api/Middleware/ExceptionMappingMiddleware.cs`
- `tests/ChecklistPoc.UnitTests/Middleware/ExceptionMappingMiddlewareTests.cs`

## Domain objects only between layers (no DTO/entity leakage)

- Controller accepts and returns domain types (`OrderDraft`, `Order`) rather than separate API DTOs.
- Service and repository also use the same domain types.

Files:

- `src/ChecklistPoc.Api/Domain/Orders/*`
- `src/ChecklistPoc.Api/Controllers/OrdersController.cs`
- `src/ChecklistPoc.Api/Services/OrderService.cs`
- `src/ChecklistPoc.Api/Repositories/*`

