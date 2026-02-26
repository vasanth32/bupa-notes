## Unit Test + Integration Test POC (Fast Track) — with Cursor Prompts

We’ll build a **FAST POC for Unit Test + Integration Test** using Cursor prompts.

**Goal**

- ⏱ Finish in **1.5–2 hours**
- 🧠 Fully understand the difference
- 🛠 Build a working demo
- 📝 Generate notes automatically

---

## POC Plan (very small but powerful)

We will build:

```plaintext
Demo.OrderApi
   ├── Controller
   ├── Service
   ├── Repository (Fake)
   ├── Unit Tests
   └── Integration Tests
```

You will clearly see:

- What is mocked
- What is real
- What is tested where

---

## Step 1 — Create Demo API (10–15 mins)

### Prompt 1 — Generate API

Paste in Cursor:

```text
Create a minimal .NET 8 Web API called Demo.OrderApi.

Requirements:
- OrderController
- GET /orders/{id}
- POST /orders
- OrderService with business logic
- IOrderRepository interface
- FakeOrderRepository implementation (in-memory list)
- Add ILogger in service
- Clean folder structure
- Keep simple and runnable
```

Run:

```text
dotnet run
```

Test endpoints.

### Prompt 2 — Ask Cursor to explain architecture

```text
Explain the flow of this project step by step.

Explain:
- Controller responsibility
- Service responsibility
- Repository responsibility
- Where business logic lives
- Why we separate them

Provide notes in markdown format for learning.
```

---

## Step 2 — Create Unit Tests (30 mins)

### Prompt 3 — Generate unit test project

```text
Create xUnit test project for this solution.

Requirements:
- Use Moq
- Create OrderServiceTests
- Mock IOrderRepository
- Write unit tests for:

1. GetOrderById returns correct order
2. GetOrderById throws exception if not found
3. CreateOrder adds new order
4. CreateOrder logs information
5. Repository throws exception and service handles it

Add proper Arrange-Act-Assert structure.
Add comments explaining why this is unit testing.
```

Run:

```text
dotnet test
```

### Prompt 4 — Ask Cursor to explain unit testing

```text
Explain what makes these tests unit tests.

Explain:
- Why repository is mocked
- Why controller is not tested here
- Why no HTTP call is made
- Why this is fast
- What is isolation

Relate to real enterprise project like Customer Domain API.
Generate short revision notes.
```

---

## Step 3 — Create Integration Tests (30–40 mins)

Now we test the full HTTP pipeline.

### Prompt 5 — Generate integration tests

```text
Create integration test project using WebApplicationFactory.

Requirements:
- Use Microsoft.AspNetCore.Mvc.Testing
- Create OrderApiIntegrationTests
- Test:

1. GET /orders/{id} returns 200
2. GET invalid id returns 404
3. POST /orders returns 201
4. Verify JSON response format

Use real pipeline (no mocking repository).
Explain in comments why this is integration testing.
```

Run:

```text
dotnet test
```

Now you are testing:

```plaintext
HTTP → Controller → Service → Repository
```

Real flow.

### Prompt 6 — Explain integration testing

```text
Explain what makes these integration tests.

Explain:
- Why WebApplicationFactory is used
- How HTTP client is created
- How full pipeline is executed
- Difference between this and unit tests
- What parts are real vs mocked

Generate comparison table in markdown.
```

---

## Step 4 — Visualize the difference

### Prompt 7 — Generate visual explanation

```text
Draw conceptual flow comparison:

Unit Test Flow:
??

Integration Test Flow:
??

Explain clearly using simple diagrams in markdown.
Relate to Kubernetes/AKS production environment.
```

---

## Step 5 — Simulate a real enterprise case

### Prompt 8 — Add validation & error middleware

```text
Add global exception middleware to API.
Return custom error JSON format.

Now:
- Update integration tests to validate error response.
- Update unit tests to validate service exception behavior.

Explain how enterprise APIs use this pattern.
```

---

## Step 6 — Generate final learning notes

### Prompt 9 — Generate personal notes

```text
Generate structured learning notes based on this POC.

Sections:
- What is Unit Testing
- What is Integration Testing
- When to use each
- What to mock
- What not to mock
- Common mistakes
- Enterprise best practices
- Mapping to Domain API architecture

Keep simple and interview ready.
```

---

## Fast execution timeline

| Task | Time |
| --- | --- |
| API | 15 min |
| Unit Tests | 30 min |
| Integration Tests | 40 min |
| Notes | 15 min |

Total ≈ **1.5–2 hours**.

---

## After this POC you will understand

- Mocking
- Moq
- xUnit
- WebApplicationFactory
- HTTP pipeline testing
- Isolation
- Test layering
- Enterprise patterns

And when someone in KT says:

> “This should be unit tested, not integration tested”

…you’ll instantly know why.

