# TaxGenPst Batch Job - Beginner's Guide

Welcome to the TaxGenPst Batch Job project! This guide will help you understand what this project does, how it's structured, and how to work with it.

---

## ?? Table of Contents
- [What is This Project?](#what-is-this-project)
- [High-Level Architecture](#high-level-architecture)
- [Project Structure](#project-structure)
- [Key Components Explained](#key-components-explained)
- [How Does It Work?](#how-does-it-work)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Development Workflow](#development-workflow)
- [Testing](#testing)
- [Common Scenarios](#common-scenarios)
- [Key Technologies Used](#key-technologies-used)
- [Troubleshooting](#troubleshooting)

---

## ?? What is This Project?

This is an **Azure Functions Batch Job** that handles **Tax Generation Post-processing (TaxGenPst)** for a health insurance system. 

**In simple terms:** This is a cloud-based automated job that runs tax-related calculations and processes. It can be triggered via HTTP requests (GET or POST) and performs different types of tax generation tasks.

**Why does it exist?**
- To automate tax statement generation and adjustments
- To process tax-related data for members
- To handle annual tax finalization and various adjustment scenarios

---

## ??? High-Level Architecture

```
???????????????????????????????????????????????????????????????
?                     Azure Function App                       ?
?  (HI.FunctionApp.Batch.TaxGenPst)                           ?
?                                                               ?
?  ?????????????????????????????????????????????????????????? ?
?  ?  HTTP Trigger (GET/POST)                               ? ?
?  ?  BatchJobTaxGenPst.cs                                  ? ?
?  ?????????????????????????????????????????????????????????? ?
?                   ?                                          ?
?                   ?                                          ?
?  ?????????????????????????????????????????????????????????? ?
?  ?  Business Logic Layer                                  ? ?
?  ?  TaxGenPstProcessor                                    ? ?
?  ?  (HI.Batch.TaxGenPst)                                  ? ?
?  ?????????????????????????????????????????????????????????? ?
?                   ?                                          ?
?                   ?                                          ?
?  ?????????????????????????????????????????????????????????? ?
?  ?  Data Layer                                            ? ?
?  ?  IDataStore ? DomainDataService / MockDataService     ? ?
?  ?????????????????????????????????????????????????????????? ?
????????????????????????????????????????????????????????????????
                    ?
                    ?
         ????????????????????????
         ?  External APIs        ?
         ?  (Domain/Membership)  ?
         ????????????????????????
```

---

## ?? Project Structure

The solution contains **3 projects**:

### 1. **HI.FunctionApp.Batch.TaxGenPst** (Main Azure Function)
**Location:** `src/HI.FunctionApp.Batch.TaxGenPst/`

**Purpose:** This is the **entry point** - the Azure Function that gets triggered via HTTP

**Key Files:**
- `Program.cs` - Application startup and dependency injection configuration
- `BatchJobTaxGenPst.cs` - The Azure Function endpoint (the actual function that gets called)
- `Config/ApiAuthOptions.cs` - Configuration for API authentication
- `Http/DomainApiAuthenticator.cs` - Handles authentication for downstream API calls
- `Instrumentation/` - Monitoring, logging, and telemetry components

### 2. **HI.Batch.TaxGenPst** (Business Logic Library)
**Location:** `src/HI.Batch.TaxGenPst/`

**Purpose:** Contains the **core business logic** separate from Azure Functions infrastructure

**Key Files:**
- `Processing/TaxGenPstProcessor.cs` - Main business logic processor
- `Data/IDataStore.cs` - Interface defining data operations
- `Data/DomainDataService.cs` - Real implementation that calls external APIs
- `Data/MockDataService.cs` - Mock implementation for testing/development

### 3. **Batch.TaxGenPst.UnitTests** (Test Project)
**Location:** `tests/Batch.TaxGenPst.UnitTests/`

**Purpose:** Unit tests for the business logic

**Key Files:**
- `BasicTests.cs` - Test cases (currently has placeholder tests)
- `MSTestSettings.cs` - Test configuration

---

## ?? Key Components Explained

### Azure Function (`BatchJobTaxGenPst.cs`)

This is the **HTTP-triggered function** - think of it as the "front door" to your application.

**What it does:**
1. Receives an HTTP request (GET or POST)
2. Generates a unique session ID for tracking
3. Calls the processor to do the actual work
4. Logs everything for monitoring
5. Returns an HTTP response with success/failure status

**Important concepts:**
- **SessionId**: A unique identifier for each job run (used for tracking in logs)
- **CorrelationId**: Tracks a request across multiple systems
- **InvocationId**: Azure Functions' own tracking ID

### Processor (`TaxGenPstProcessor.cs`)

This is the **brain** of the application - it contains all the business logic.

**What it does:**
1. Reads configuration (Mode, Year, etc.)
2. Validates the inputs
3. Decides which operation to perform based on the mode
4. Calls the data store to interact with external systems
5. Returns a result summary

### Data Store (`IDataStore` interface)

This is an **abstraction** over external data sources. It has two implementations:

**DomainDataService** (Real):
- Makes actual HTTP calls to external APIs
- Not yet fully implemented (marked with `NotImplementedException`)

**MockDataService** (Mock):
- Returns fake data for testing
- Currently active in the code (see `Program.cs` line with comment "remove this line")

---

## ?? How Does It Work?

### The Flow (Step by Step)

1. **HTTP Request arrives** ? Function triggers
2. **Function creates context** ? SessionId, CorrelationId, logging setup
3. **Processor starts** ? Reads configuration
4. **Validation** ? Checks if mode and parameters are valid
5. **Business Logic Execution:**
   - If **Annual mode** ? Calls `FinaliseGenerationAsync()` 
   - If **Other modes** ? Calls `RunPostGenerationAsync()`
6. **Data Store Call** ? Makes HTTP call to external API (or returns mock data)
7. **Result Processing** ? Processor interprets the response
8. **Response** ? Function returns HTTP 200 (success) or 500 (error)
9. **Logging & Telemetry** ? Everything is logged to Application Insights and/or SQL database

### Tax Generation Modes

The job supports different modes (defined in `IDataStore.cs`):

| Mode | Value | Description |
|------|-------|-------------|
| **Annual** | 1 | Annual tax finalization (no year parameter needed) |
| **AdjustmentForMailHouse** | 2 | Adjustments for mail house processing |
| **AdjustmentForATO** | 3 | Adjustments for Australian Tax Office |
| **ManualReissue** | 4 | Manual reissue (not allowed via this job) |
| **SystemFix** | 5 | System fixes |
| **None** | 6 | Invalid/default (will cause validation error) |

---

## ?? Getting Started

### Prerequisites

1. **.NET 8 SDK** - The project targets .NET 8
2. **Visual Studio 2022** (or VS Code with Azure Functions extension)
3. **Azure Functions Core Tools** (for local development)
4. **Azure subscription** (for deployment)

### Running Locally

1. **Open the solution:**
   ```
   HI.FunctionApp.Batch.TaxGenPst.sln
   ```

2. **Set startup project:**
   - Right-click `HI.FunctionApp.Batch.TaxGenPst` ? "Set as Startup Project"

3. **Configure local settings:**
   - Edit `src/HI.FunctionApp.Batch.TaxGenPst/local.settings.json`
   - Add your configuration values (see Configuration section)

4. **Run the function:**
   - Press F5 in Visual Studio
   - The function will start on a local port (e.g., http://localhost:7071)

5. **Trigger the function:**
   - Open a browser or Postman
   - Navigate to: `http://localhost:7071/api/BatchJobTaxGenPst`
   - You should see "TaxGenPst batch job completed" if using mock data

---

## ?? Configuration

### Application Settings Structure

The application uses `appsettings.json` and environment variables:

```json
{
  "BatchJobSettings": {
    "Mode": "Annual",           // Tax generation mode
    "Year": 2024               // Tax year (required for non-annual modes)
  },
  "MembershipApi": {
    "BaseUrl": "https://api.example.com",
    "TimeoutSeconds": 30,
    "Auth": {
      // Authentication settings
    }
  },
  "AzureMonitor": {
    "ConnectionString": "InstrumentationKey=..."  // Application Insights
  },
  "ConnectionStrings": {
    "LoggingDb": "Server=...;Database=..."       // SQL logging (optional)
  }
}
```

### Configuration Files

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides
- `local.settings.json` - Local development secrets (not in source control)

---

## ??? Development Workflow

### Making Changes

1. **Business Logic Changes** ? Edit files in `HI.Batch.TaxGenPst` project
2. **Function/Infrastructure Changes** ? Edit files in `HI.FunctionApp.Batch.TaxGenPst` project
3. **Always build and test** after changes

### Adding New Functionality

**Example: Adding a new validation rule**

1. Open `TaxGenPstProcessor.cs`
2. Add your validation in the `RunAsync` method (before data store calls)
3. Return appropriate `BatchProcessingResult` if validation fails
4. Add unit tests in `BasicTests.cs`

### Switching from Mock to Real Data

Currently, the application uses **MockDataService** for testing.

**To enable real API calls:**

1. Open `Program.cs`
2. Find this line:
   ```csharp
   builder.Services.AddSingleton<IDataStore, MockDataService>(); 
   ```
3. **Remove** or comment out that line
4. The `DomainDataService` will be used instead (registered earlier in the file)
5. **Note:** You'll need to implement the actual endpoints in `DomainDataService.cs`

---

## ?? Testing

### Running Unit Tests

1. Open **Test Explorer** in Visual Studio (Test ? Test Explorer)
2. Click "Run All Tests"
3. Currently, there's only a placeholder test

### Writing Tests

Tests use **MSTest** framework:

```csharp
[TestClass]
public class MyTests
{
    [TestMethod]
    public void MyTest_Should_Pass()
    {
        // Arrange
        // Act
        // Assert
        Assert.IsTrue(true);
    }
}
```

### Manual Testing

1. Start the function locally (F5)
2. Use **Postman** or **curl** to send requests:
   ```bash
   curl http://localhost:7071/api/BatchJobTaxGenPst
   ```
3. Check the console logs for output
4. Verify the response

---

## ?? Common Scenarios

### Scenario 1: Running Annual Tax Finalization

**Configuration:**
```json
{
  "BatchJobSettings": {
    "Mode": "Annual"
  }
}
```

**What happens:**
- No year parameter needed
- Calls `FinaliseGenerationAsync()` with Annual mode
- Finalizes all tax generation for the year

### Scenario 2: Running Post-Generation Adjustments

**Configuration:**
```json
{
  "BatchJobSettings": {
    "Mode": "AdjustmentForATO",
    "Year": 2024
  }
}
```

**What happens:**
- Requires a year parameter
- Calls `RunPostGenerationAsync()` with specified mode and year
- Processes adjustments for ATO submission

### Scenario 3: Validation Error

**Configuration:**
```json
{
  "BatchJobSettings": {
    "Mode": "None"
  }
}
```

**What happens:**
- Validation fails immediately
- Returns HTTP 500 with error message
- Logs a warning (not an error, since it's expected validation)

---

## ?? Key Technologies Used

### Core Technologies

| Technology | Purpose | Where Used |
|------------|---------|------------|
| **.NET 8** | Runtime platform | All projects |
| **Azure Functions v4** | Serverless hosting | Function App project |
| **Serilog** | Structured logging | Throughout the app |
| **Application Insights** | Monitoring & telemetry | Instrumentation layer |
| **MSTest** | Unit testing framework | Test project |

### Key NuGet Packages

- `Microsoft.Azure.Functions.Worker` - Azure Functions runtime
- `Azure.Identity` - Azure authentication
- `Serilog.Sinks.*` - Logging to various destinations
- `Microsoft.ApplicationInsights.*` - Telemetry and monitoring

### Design Patterns Used

1. **Dependency Injection** - All components are registered and injected via DI container
2. **Repository Pattern** - `IDataStore` abstracts data access
3. **Strategy Pattern** - Different processors based on mode
4. **Factory Pattern** - HTTP client factory for creating configured HTTP clients

---

## ?? Key Components Explained (In-Depth)

### 1. Program.cs - Application Startup

**What it does:**
- Configures the Azure Functions host
- Sets up Dependency Injection (DI)
- Configures logging (Serilog)
- Registers services (HTTP clients, processors, data stores)
- Configures Application Insights monitoring

**Key sections:**
```csharp
// Configuration
builder.Configuration.AddJsonFile("appsettings.json", ...)

// Application Insights setup
builder.Services.AddApplicationInsightsTelemetryWorkerService(...)

// Register your services
builder.Services.AddSingleton<ITaxGenPstProcessor, TaxGenPstProcessor>();

// HTTP Client with authentication
builder.Services.AddHttpClient<IDataStore, DomainDataService>(...)
    .AddHttpMessageHandler<DomainApiAuthenticator>();
```

### 2. BatchJobTaxGenPst.cs - The Function

**Key methods/concepts:**

- **Constructor Injection:** Dependencies are injected via constructor
  ```csharp
  public BatchJobTaxGenPst(ILogger, ITaxGenPstProcessor, TelemetryClient, IConfiguration)
  ```

- **Function Attribute:** Defines this as an Azure Function
  ```csharp
  [Function("BatchJobTaxGenPst")]
  ```

- **HTTP Trigger:** Defines how to trigger the function
  ```csharp
  [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
  ```

- **Context objects:**
  - `FunctionContext` - Information about the function execution
  - `HttpRequestData` - The incoming HTTP request
  - `HttpResponseData` - The outgoing HTTP response

### 3. TaxGenPstProcessor.cs - Business Logic

**Responsibilities:**
1. **Input Validation** - Checks if mode and year are valid
2. **Business Rules** - Enforces rules (e.g., ManualReissue not allowed)
3. **Orchestration** - Decides which data store method to call
4. **Result Mapping** - Converts API responses to `BatchProcessingResult`

**Status Codes:**
- `Success` - Everything worked
- `BusinessError` - Validation or business rule violation
- `Failed` - Technical error
- `Cancelled` - Job was cancelled

### 4. IDataStore & Implementations - Data Layer

**Purpose:** Abstraction for external API calls

**MockDataService:**
- Returns fake successful responses
- Good for development and testing
- No external dependencies needed

**DomainDataService:**
- Makes real HTTP calls to external APIs
- Uses authenticated HTTP client
- Not yet fully implemented (placeholders exist)

### 5. Instrumentation Components

**DependencyTrackingProcessor:**
- Customizes how dependencies are tracked in Application Insights

**CommonTelemetryInitializer:**
- Adds common properties to all telemetry (app name, environment, version)

**FunctionActivitySource:**
- Implements distributed tracing using OpenTelemetry

**DomainApiAuthenticator:**
- HTTP message handler that adds authentication to outgoing API requests
- Uses Azure DefaultAzureCredential (Managed Identity, Service Principal, etc.)

---

## ?? How Does It Work? (Detailed Flow)

### Request Flow Example

Let's walk through what happens when you call the function:

1. **HTTP Request:** `POST http://localhost:7071/api/BatchJobTaxGenPst`

2. **Azure Function Runtime** receives the request and calls `Run()` method

3. **Logging Context Setup:**
   ```
   InvocationId: abc-123
   CorrelationId: abc-123
   SessionId: 1234567890 (timestamp-based)
   ```

4. **Activity Tracking starts** - For distributed tracing

5. **Processor invoked:**
   ```csharp
   jobProcessingResult = await _processor.RunAsync(sessionId, ct);
   ```

6. **Processor reads config:**
   ```
   Mode: Annual
   Year: (not needed for Annual)
   ```

7. **Validation passes** (Mode is valid, not ManualReissue)

8. **Data Store call:**
   ```csharp
   apiResponse = await _dataStore.FinaliseGenerationAsync(Annual, null, ct);
   ```

9. **Mock returns success:**
   ```csharp
   return new ApiResponse("Success", correlationId);
   ```

10. **Processor maps to result:**
    ```csharp
    return new BatchProcessingResult(TaxGenPstStatus.Success, null, correlationId, null, null);
    ```

11. **Function logs success and returns HTTP 200:**
    ```
    "TaxGenPst batch job completed. Status=Success"
    ```

### Error Handling Flow

If something goes wrong:

- **Validation Error** ? Returns `BusinessError` status ? HTTP 500 ? Warning logged
- **Exception** ? Caught in try-catch ? `Failed` status ? HTTP 500 ? Error logged
- **Cancellation** ? Returns `Cancelled` status ? HTTP 500 ? Warning logged

---

## ?? Configuration (Detailed)

### BatchJobSettings

Controls the job behavior:

```json
{
  "BatchJobSettings": {
    "Mode": "Annual",  // or "AdjustmentForATO", "AdjustmentForMailHouse", "SystemFix"
    "Year": 2024       // Required for non-Annual modes
  }
}
```

**Rules:**
- `Mode` is **required** and must not be "None" or "ManualReissue"
- `Year` is **required** for all modes except "Annual"

### MembershipApi Settings

Configures external API calls:

```json
{
  "MembershipApi": {
    "BaseUrl": "https://api.membership.example.com",
    "TimeoutSeconds": 30,
    "Auth": {
      // Scope, ClientId, etc. (defined in ApiAuthOptions.cs)
    }
  }
}
```

### Monitoring Settings

```json
{
  "AzureMonitor": {
    "ConnectionString": "InstrumentationKey=xxx;IngestionEndpoint=https://..."
  },
  "ConnectionStrings": {
    "LoggingDb": "Server=tcp:xxx;Database=logs;..."
  }
}
```

**Where logs go:**
1. **Console** - Always (visible during local development)
2. **Application Insights** - If connection string is configured
3. **SQL Database** - If LoggingDb connection string is configured

---

## ?? Development Workflow

### Step 1: Set Up Your Environment

1. Clone the repository
2. Open `HI.FunctionApp.Batch.TaxGenPst.sln` in Visual Studio
3. Restore NuGet packages (should happen automatically)

### Step 2: Configure Local Settings

1. Create/edit `local.settings.json` in the function project:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
     }
   }
   ```

### Step 3: Build the Solution

- In Visual Studio: **Build ? Build Solution** (or Ctrl+Shift+B)
- Watch for errors in the Error List window

### Step 4: Run and Debug

- Press **F5** to start debugging
- Function will start and show the endpoint URL in console
- Set breakpoints in code to step through execution

### Step 5: Make Changes

**Example: Adding a new validation rule**

1. Open `TaxGenPstProcessor.cs`
2. Find the `RunAsync` method
3. Add your validation after existing checks:
   ```csharp
   if (year.HasValue && year.Value < 2000)
   {
       return new BatchProcessingResult(
           TaxGenPstStatus.BusinessError, 
           "INVALID_YEAR", 
           null, null, 
           "Year must be 2000 or later");
   }
   ```

### Step 6: Test Your Changes

1. Write a unit test
2. Run the function locally
3. Verify logs and responses

---

## ?? Testing

### Unit Testing Structure

Currently uses **MSTest** framework.

**Basic test structure:**
```csharp
[TestClass]
public class ProcessorTests
{
    [TestMethod]
    public async Task RunAsync_WithInvalidMode_ReturnsBusinessError()
    {
        // Arrange - Set up test data and mocks
        
        // Act - Call the method
        
        // Assert - Verify the result
    }
}
```

### Testing with Mocks

The project already uses `MockDataService` for development. You can:

1. Create mock implementations of interfaces
2. Use mocking frameworks like Moq or NSubstitute
3. Test business logic in isolation

---

## ?? Common Scenarios

### Scenario: Adding a New Tax Generation Mode

1. Add enum value to `TaxGenerationMode` in `IDataStore.cs`
2. Add handling in `TaxGenPstProcessor.RunAsync()`
3. Add corresponding method to `IDataStore` if needed
4. Implement in both Mock and Domain data services
5. Add configuration documentation
6. Write tests

### Scenario: Adding Authentication to API Calls

The project already has `DomainApiAuthenticator` configured:

1. Configure `ApiAuthOptions` in appsettings
2. The authenticator automatically adds bearer tokens to requests
3. Uses Azure DefaultAzureCredential (supports Managed Identity)

### Scenario: Adding New Logging

The project uses **Serilog** with structured logging:

```csharp
_logger.LogInformation("Processing started {SessionId} Mode={Mode}", sessionId, mode);
_logger.LogWarning("Validation failed: {ErrorCode}", errorCode);
_logger.LogError(ex, "Unexpected error occurred");
```

**Log properties are automatically captured:**
- SessionId
- InvocationId
- CorrelationId
- Application name
- Environment
- Assembly version

---

## ?? Troubleshooting

### Issue: Function won't start

**Possible causes:**
- Missing `local.settings.json`
- Invalid configuration
- Port already in use

**Solution:**
- Check Output window for errors
- Verify configuration files exist
- Check if another process is using port 7071

### Issue: "NotImplementedException" error

**Cause:** Real API endpoints not implemented yet

**Solution:**
- Use MockDataService (check `Program.cs`)
- Or implement the methods in `DomainDataService.cs`

### Issue: Authentication failures

**Cause:** Missing or incorrect API authentication configuration

**Solution:**
- Verify `MembershipApi:Auth` configuration
- Ensure Azure credentials are available (when using DomainDataService)
- Check if using correct authentication method for your environment

### Issue: Logs not appearing in Application Insights

**Possible causes:**
- Connection string not configured
- Connection string invalid
- Sampling too aggressive

**Solution:**
- Verify `AzureMonitor:ConnectionString` in config
- Check `host.json` sampling settings
- Wait a few minutes for telemetry to appear

---

## ?? Key Concepts for Beginners

### What is Azure Functions?

Think of it as a **"function as a service"** - you write a function, upload it to Azure, and Azure runs it when triggered. You don't manage servers, scaling, or infrastructure.

**Benefits:**
- Pay only when function runs
- Automatic scaling
- Built-in monitoring
- Easy to deploy

### What is Dependency Injection?

Instead of creating objects yourself:
```csharp
var processor = new TaxGenPstProcessor(new MockDataService(), logger, config);
```

You **declare what you need** and the framework provides it:
```csharp
public BatchJobTaxGenPst(ITaxGenPstProcessor processor)  // Automatically provided
{
    _processor = processor;
}
```

### What is Structured Logging?

Instead of:
```csharp
_logger.LogInformation("Processing session 12345 with mode Annual");
```

You use:
```csharp
_logger.LogInformation("Processing {SessionId} {Mode}", sessionId, mode);
```

**Benefits:**
- Searchable in log systems
- Easier to query and analyze
- Better for monitoring

### What is Telemetry?

**Telemetry** = Automatic collection of data about how your application runs:
- How long functions take
- How many succeed/fail
- What exceptions occur
- API call durations

All sent to **Application Insights** for dashboards and alerts.

---

## ?? Learning Resources

### To Learn More About:

**Azure Functions:**
- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [Isolated Worker Model](https://docs.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide)

**.NET Dependency Injection:**
- [.NET Dependency Injection](https://docs.microsoft.com/dotnet/core/extensions/dependency-injection)

**Serilog:**
- [Serilog Documentation](https://serilog.net/)
- [Structured Logging Best Practices](https://stackify.com/what-is-structured-logging-and-why-developers-need-it/)

**Application Insights:**
- [Application Insights Overview](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)

---

## ?? Next Steps

As a new developer on this project, here's what you should do:

### Week 1: Understanding
1. ? Read this guide
2. ? Clone and build the solution
3. ? Run the function locally
4. ? Read through `Program.cs`, `BatchJobTaxGenPst.cs`, and `TaxGenPstProcessor.cs`
5. ? Experiment with different configuration modes

### Week 2: Hands-On
1. ? Add a new unit test
2. ? Make a small change to logging
3. ? Add a new validation rule
4. ? Review the instrumentation code

### Week 3: Deep Dive
1. ? Implement one of the `DomainDataService` methods
2. ? Add comprehensive error handling
3. ? Write integration tests
4. ? Review the authentication flow

---

## ?? Getting Help

### Who to Ask:

- **Business Logic Questions** ? Product Owner / Business Analyst
- **Technical Questions** ? Tech Lead / Senior Developers
- **Azure/Infrastructure** ? DevOps Team
- **API Integration** ? API Team

### Useful Commands:

**Build:**
```bash
dotnet build
```

**Run tests:**
```bash
dotnet test
```

**Start function locally:**
```bash
cd src/HI.FunctionApp.Batch.TaxGenPst
func start
```

**View logs:**
- Check Visual Studio Output window
- Check Azure Portal ? Function App ? Log Stream
- Check Application Insights ? Logs

---

## ?? Important Notes

### Current State of the Project

?? **Note:** This appears to be a **template or early-stage project**:
- `DomainDataService` methods are not yet implemented
- Tests are placeholder tests
- Mock data service is active by default
- README.md still has TODO sections

### Before Going to Production

1. ? Implement actual API endpoints in `DomainDataService`
2. ? Remove or disable `MockDataService`
3. ? Add comprehensive unit and integration tests
4. ? Configure production Application Insights
5. ? Set up proper authentication
6. ? Configure environment-specific settings
7. ? Add proper error handling and retry logic
8. ? Document API contracts and dependencies

---

## ?? Quick Reference

### File ? Purpose Mapping

| File | Purpose |
|------|---------|
| `Program.cs` | App startup, DI configuration |
| `BatchJobTaxGenPst.cs` | HTTP endpoint, orchestration |
| `TaxGenPstProcessor.cs` | Business logic, validation |
| `IDataStore.cs` | Data access contract, enums |
| `MockDataService.cs` | Fake data for testing |
| `DomainDataService.cs` | Real API calls (not implemented) |
| `DomainApiAuthenticator.cs` | API authentication handler |
| `appsettings.json` | Configuration settings |
| `host.json` | Azure Functions host config |

### Common Namespaces

- `HI.FunctionApp.BatchJobHost` - Azure Function components
- `HI.TaxGenPst.Processing` - Business logic
- `HI.TaxGenPst.Data` - Data access layer
- `HI.TaxGenPst.Mocks` - Mock implementations

---

## ?? Support

If you're stuck:
1. Check this guide
2. Review the code comments
3. Check Azure Functions documentation
4. Ask your team lead
5. Review Application Insights logs for runtime issues

---

**Good luck with your development! ??**

---

*Last Updated: [Auto-generated]*
*Project: HI.FunctionApp.Batch.TaxGenPstJob*
*Target Framework: .NET 8*
*Azure Functions Version: v4*
