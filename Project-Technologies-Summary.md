# BUPA Health Insurance Project - Technologies, Concepts & Tools Summary

## 📋 Table of Contents
1. [Core Concepts](#core-concepts)
2. [Architecture Patterns](#architecture-patterns)
3. [Technologies - Legacy Stack](#technologies---legacy-stack)
4. [Technologies - Modern Stack](#technologies---modern-stack)
5. [Azure Services & Cloud Tools](#azure-services--cloud-tools)
6. [Development Tools & Packages](#development-tools--packages)
7. [Business Domain Concepts](#business-domain-concepts)

---

## Core Concepts

### 1. **Domain-Driven Design (DDD)**
- System organized into **Domain APIs** by business capability
- Each domain owns its business logic and data access
- Domains: Customer, Payment, Sales, Membership, Interaction, Claim, Reference, Premium

### 2. **Microservices Architecture**
- Migration from monolithic WCF service to distributed Domain APIs
- Each Domain API is independently deployable
- Better scalability and maintainability

### 3. **Migration Strategy (3 Milestones)**
- **M1 (Foundation)**: Common infrastructure and cross-cutting concerns
- **M2 (MVP)**: Proof of concept with Customer & Reference Domain APIs (~20 endpoints)
- **M3 (Full Migration)**: Complete migration, decommission legacy systems

### 4. **BFF Pattern (Backend for Frontend)**
- Layer between frontend applications and Domain APIs
- Examples: Apollo BFF, Cyclops
- Aggregates data from multiple Domain APIs for frontend consumption

### 5. **Event-Driven Architecture**
- Asynchronous messaging patterns
- Migration from MSMQ to Azure Service Bus
- Queue-based processing with Azure Functions

---

## Architecture Patterns

### 1. **Layered Architecture**
```
Controllers → Services → Repositories → Data Access
```

### 2. **Repository Pattern**
- Abstraction layer for data access
- Separation between business logic and data persistence

### 3. **Service Layer Pattern**
- Business logic orchestration
- Domain services handle use cases

### 4. **API Gateway Pattern**
- Domain APIs act as gateways to business functionality
- Single entry point per domain

### 5. **Message Queue Pattern**
- Asynchronous processing via queues
- Session-enabled queues for ordering
- Dead-letter queues for error handling

---

## Technologies - Legacy Stack

### 1. **WCF (Windows Communication Foundation)**
- **Purpose**: Legacy service-oriented architecture framework
- **Status**: Being decommissioned
- **Replacement**: REST-based Domain APIs

### 2. **.NET Framework 4.8**
- **Purpose**: Legacy Hugo Core Services
- **Status**: Being migrated to modern .NET
- **Replacement**: .NET Core/.NET 6/8/9

### 3. **MSMQ (Microsoft Message Queuing)**
- **Purpose**: Legacy asynchronous messaging
- **Status**: Part of decommissioning scope
- **Replacement**: Azure Service Bus

### 4. **Windows Services**
- **Purpose**: Background processing and queue handling
- **Status**: Being replaced
- **Replacement**: Azure Functions

### 5. **SOAP Protocol**
- **Purpose**: WCF services use SOAP/XML
- **Status**: Legacy communication protocol
- **Replacement**: REST/JSON

---

## Technologies - Modern Stack

### 1. **.NET Core / .NET 6/8/9**
- **Purpose**: Modern Domain APIs development
- **Version Strategy**: 
  - Existing APIs: .NET Core/.NET 6
  - New APIs: .NET 8 or 9
- **Type**: Web API projects

### 2. **REST APIs**
- **Purpose**: Modern API communication
- **Protocol**: HTTP/HTTPS
- **Format**: JSON
- **Style**: Resource-based endpoints

### 3. **Entity Framework / Dapper**
- **Purpose**: Data access layer
- **Usage**: Database operations in Domain APIs
- **Alternative**: Stored procedures

### 4. **NuGet Packages**
- **Purpose**: Shared components and dependencies
- **M1 Packages**: Authentication, Authorization, Logging, Caching, Exception Handling
- **Distribution**: Internal NuGet feed

### 5. **Application Insights**
- **Purpose**: Telemetry and monitoring
- **Integration**: Distributed tracing
- **Features**: Performance metrics, logging, alerting

---

## Azure Services & Cloud Tools

### 1. **Azure Service Bus**
- **Purpose**: Modern messaging infrastructure
- **Features**:
  - Queues (FIFO messaging)
  - Topics (pub/sub scenarios)
  - Session-enabled queues (ordering/grouping)
  - Dead-letter queues (error handling)
- **Replaces**: MSMQ

### 2. **Azure Functions**
- **Purpose**: Serverless compute for queue processing
- **Types**:
  - **Queue-triggered**: Process messages from Service Bus
  - **Timer-triggered**: Scheduled tasks (e.g., Wait Timer Function)
- **Functions in M3**:
  - Listener Function
  - Processor Function
  - Wait Timer Function

### 3. **Azure Key Vault**
- **Purpose**: Secure storage of secrets
- **Usage**: Connection strings, API keys, certificates
- **Integration**: Referenced from app settings

### 4. **Azure App Service**
- **Purpose**: Hosting Domain APIs
- **Type**: Web App / API App
- **Configuration**: Environment variables, app settings

### 5. **Application Insights**
- **Purpose**: Application performance monitoring
- **Features**:
  - Distributed tracing
  - Log aggregation
  - Performance metrics
  - Alerting

### 6. **Azure Storage**
- **Purpose**: Document storage
- **Usage**: Reference Domain API for document management

---

## Development Tools & Packages

### 1. **Visual Studio / VS Code**
- **Purpose**: Primary development IDE
- **Usage**: .NET development, debugging

### 2. **NuGet Package Manager**
- **Purpose**: Package management
- **Packages**: M1 foundation packages, third-party libraries

### 3. **Git**
- **Purpose**: Version control
- **Repository**: Source code management

### 4. **Azure DevOps / Teams**
- **Purpose**: Collaboration and documentation
- **Features**: 
  - Teams "Foundation" folder
  - Discovery phase documents
  - Project documentation

### 5. **M1 Foundation Packages**
- **Bupa.HI.Authentication.Package**: Authentication framework
- **Bupa.HI.Authorization.Package**: Authorization and role-based access
- **Logging Package**: Centralized logging with Application Insights
- **Exception Handling Package**: Global exception management
- **Caching Package**: Performance optimization
- **Configuration Package**: Settings management

### 6. **Code Quality & Static Analysis Tools**
- **SonarQube** (if used)
  - Static code analysis tool
  - Code quality metrics and security vulnerability detection
  - Code smell detection and technical debt tracking
  - Supports .NET/C# analysis
  - Typically integrated into CI/CD pipelines
  - **Note**: Not explicitly mentioned in KT notes, but commonly used in enterprise .NET projects

### 7. **Local Development Tools**
- **local.settings.json**: Local Function App configuration
- **appsettings.json**: API configuration
- **Azure CLI / PowerShell**: Azure resource management

---

## Business Domain Concepts

### 1. **Health Insurance Domain**
- **Customer Types**: Regular customers, OSHC (Overseas Student Health Cover)
- **Insurance Packages**: Various coverage options
- **Premium Calculation**: Based on members, options, no-claim bonus

### 2. **No-Claim Bonus**
- **Concept**: Discount for customers with no claims in previous year
- **Similar to**: Vehicle insurance no-claim bonus
- **Impact**: Premium reduction for next year

### 3. **Digital Insurance Card**
- **Purpose**: Digital card with barcode for healthcare access
- **Generation**: After successful payment
- **Usage**: Accessing healthcare services

### 4. **Healthcare Providers**
- **Types**: Hospitals, clinics, medical service providers
- **Integration**: Claim processing involves providers

### 5. **Claim Processing**
- **Flow**: Customer uses services → Submits claim → Processing → Refund
- **Domain**: Claim Domain API handles this

### 6. **OSHC (Overseas Student Health Cover)**
- **Purpose**: Special health insurance for international students in Australia
- **Significance**: Important business term in the application

---

## Domain APIs Overview

### 1. **Customer Domain API** (M2 - Complete)
- Customer information management
- Customer types handling
- ~20 endpoints implemented

### 2. **Reference Domain API** (M2 - Complete)
- Document rendering
- Email functionality
- Document storage
- Utility functions

### 3. **Payment Domain API** (M3 - In Development)
- Payment processing
- Transaction management
- Integration with Stripe, PayPal

### 4. **Sales Domain API** (M3 - In Development)
- Sales process management
- Membership acquisition

### 5. **Membership Domain API** (M3 - In Development)
- Membership lifecycle management
- Policy management

### 6. **Interaction Domain API** (M3 - In Development)
- Customer interactions
- Communication tracking

### 7. **Claim Domain API** (M3 - In Development)
- Claim submission
- Claim processing
- Provider integration
- Refund handling

### 8. **Premium Domain API** (M3 - In Development)
- Premium calculation
- Discount management
- No-claim bonus application

---

## Integration Points

### 1. **Frontend Applications**
- Cyclops (Windows Forms desktop app)
- MyBupa (mobile & web)
- Digital Card Domain
- BCA (Business Customer Application)
- Corporate (CPC) Domain
- Optical BFF API
- Titanium APIs

### 2. **External Systems**
External systems outside BUPA that integrate with the health insurance platform:

- **IVR (Interactive Voice Response)**
  - Phone-based automated customer service system
  - Customers interact via voice/DTMF tones
  - Routes through DMZ layer (IVR Wrapper) for security
  - Handles customer inquiries and transactions via phone

- **Comparators**
  - Third-party systems for comparing insurance products/prices
  - Used for competitive analysis or partner integrations
  - May provide market comparison data

- **BizTalk**
  - Microsoft enterprise integration platform
  - Handles Enterprise Application Integration (EAI)
  - Manages message routing and transformation between systems
  - Bridges different protocols and data formats

- **HCA Claim Event**
  - Healthcare provider or claim processing system
  - Sends claim-related events to BUPA's system
  - Part of the claim processing workflow
  - Integrates with Claim Domain API

- **Hugo Batch**
  - Batch processing system for bulk operations
  - Handles scheduled jobs and batch updates
  - Connects to Hugo Service for processing
  - Processes large volumes of data asynchronously

### 3. **DMZ Layer (Demilitarized Zone)**
Security layer between external systems and internal BUPA systems. Acts as the first line of defense and protocol translation layer.

- **IVR Wrapper**
  - Security wrapper around IVR communications
  - Validates, sanitizes, and routes IVR requests
  - Acts as a security gateway for voice/phone interactions
  - Protects internal systems from direct IVR access

- **Relay Facade**
  - Facade pattern implementation for external system communications
  - Provides unified interface and security layer
  - Handles authentication, routing, and protocol translation
  - Protects internal systems from direct external access
  - Monitors and logs external system interactions

**DMZ Layer Purpose:**
- **Security**: First line of defense against external threats
- **Protocol Translation**: Converts external protocols to internal formats
- **Request Validation**: Ensures only valid requests reach internal systems
- **Load Balancing**: Distributes incoming requests
- **Monitoring**: Tracks external system interactions

### 4. **Backend Services**
- Boss Enterprise Service (WCF legacy)
- WDM (WebHandle & Database)
- Hugo Database
- Address Validation Service
- Payment Gateways (Stripe, PayPal)

---

## Key Technical Flows

### 1. **Authentication & Authorization Flow**
- Token-based authentication (OAuth 2.0 / OpenID Connect)
- Token validation (issuer, signature, audience, expiry)
- Role-based authorization (scopes/roles)
- Token refresh mechanism (for interactive apps like Cyclops)

### 2. **Distributed Tracing Flow**
- End-to-end request tracing
- Cross-service correlation
- Queue operation tracking
- API call monitoring

### 3. **Message Processing Flow (M3)**
```
Hugo → In Queue → Listener Function → Process Queue → 
Processor Function → Boss Service → Completion/Error Queue
```

### 4. **Error Handling Flow**
- Error Queue management
- Wait Queue for dependent messages
- Sync Admin Tool for manual intervention
- Dead-letter queue for failed messages

---

## Configuration & Deployment

### 1. **Configuration Management**
- **Local**: `local.settings.json` (Functions), `appsettings.json` (APIs)
- **Azure**: App Settings / Environment Variables
- **Secrets**: Azure Key Vault integration

### 2. **Retry & Resilience**
- Service Bus retry policies
- Function App retry configuration
- Timeout handling
- Exception routing

### 3. **Monitoring & Observability**
- Application Insights integration
- Distributed tracing
- Performance metrics
- Alerting mechanisms

---

## Migration Timeline

- **Discovery Phase**: July 2024
- **Foundation Phase (M1)**: November 2024
- **MVP Phase (M2)**: Completed (in testing)
- **Full Implementation (M3)**: Ongoing, expected until Feb 2026

---

## Summary

This project represents a **modernization initiative** moving from:
- **Legacy**: WCF monolith, MSMQ, Windows Services, .NET Framework 4.8
- **Modern**: Domain APIs, Azure Service Bus, Azure Functions, .NET Core/8/9

**Key Technologies**: .NET Core/8/9, Azure (Service Bus, Functions, Key Vault, App Insights), REST APIs, Entity Framework/Dapper

**Key Concepts**: Domain-Driven Design, Microservices, BFF Pattern, Event-Driven Architecture, Repository Pattern

**Key Tools**: Visual Studio, NuGet, Git, Azure DevOps, Application Insights

