# Health Insurance Project - Technical Architecture & Migration Strategy

## Quick Overview (Plain English)

- **What this project is doing**: Moving from a **single big legacy WCF system (Hugo Service + MSMQ)** to a set of **modern Domain APIs + Azure Service Bus + Azure Functions**.
- **Why**: The legacy stack (WCF, MSMQ, Windows services) is **hard to change, scale, and observe**. Domain APIs are **cleaner, more maintainable, and cloud-friendly**.
- **Three milestones**:
  - **M1 – Foundation**: Build **common building blocks** (auth, logging, exceptions, caching, monitoring) used by all Domain APIs.
  - **M2 – MVP**: Implement **Customer** and **Reference** Domain APIs and wire a **few real flows** (≈20 endpoints) to prove the approach.
  - **M3 – Full Migration**: **Move remaining business logic** out of Hugo into Domain APIs, **switch consumers** (Cyclops, BFFs, etc.) to those APIs, and **turn off Hugo + MSMQ + Hugo Sync Service**.
- **High‑level before/after**:
  - **Before**: All frontends and external systems → **Hugo Service (WCF)** → **Hugo DB, Boss, WDM**, with **MSMQ** + Windows services for async flows.
  - **After**: All frontends and external systems → **Domain APIs** → **Hugo DB, Boss, external services**, and **Azure Service Bus + Functions** replace MSMQ + Windows services.

## How to Use This Document

- **If you want a business‑level picture** (what is changing and why):
  - Read: `System Architecture Diagram`, `Current State Overview`, `Migration Strategy – Three Milestones`, and `Target Architecture – Domain APIs`.
- **If you are working on authentication / security**:
  - Focus on: `Milestone 1 – Auth, Token & Authorization Flow (Detailed KT Notes)`.
- **If you are working on logging / troubleshooting live flows**:
  - Focus on: `Distributed Tracing & Logging (Foundation Capability)`.
- **If you are working on M3 queues and Boss sync**:
  - Focus on: `Milestone 3 – MSMQ to Azure Service Bus & Boss Sync (Detailed Flow)`.
- **If you are new to some terms (WCF, BFF, MSMQ, Service Bus, etc.)**:
  - Read: `Key Technical Concepts` and then come back to the earlier sections.

---

## System Architecture Diagram (From KT Session)

### Complete Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                           BUPA HEALTH INSURANCE SYSTEM ARCHITECTURE                          │
└─────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────┐         ┌──────────────────────┐         ┌──────────────────────┐
│   EXTERNAL SYSTEMS   │         │      DMZ LAYER       │         │   BUPA SYSTEMS       │
│                      │         │                      │         │                      │
│  ┌──────────────┐   │         │  ┌──────────────┐   │         │  ┌──────────────┐   │
│  │     IVR      │───┼────────▶│  │ IVR Wrapper  │   │         │  │   Cyclops    │───┼─┐
│  └──────────────┘   │         │  └──────┬───────┘   │         │  └──────────────┘   │ │
│                     │         │         │           │         │                     │ │
│  ┌──────────────┐   │         │         ▼           │         │  ┌──────────────┐   │ │
│  │ Comparators  │───┼────────▶│  ┌──────────────┐  │         │  │ MyBupa       │───┼─┼─┐
│  └──────────────┘   │         │  │ Relay Facade │  │         │  │ (mobile)     │   │ │ │
│                     │         │  └──────┬───────┘  │         │  └──────────────┘   │ │ │
│  ┌──────────────┐   │         │         │          │         │                     │ │ │
│  │   BizTalk    │───┼─────────┼─────────┼──────────┼────────▶│  ┌──────────────┐   │ │ │
│  └──────────────┘   │         │         │          │         │  │ MyBupa       │───┼─┼─┼─┐
│                     │         │         │          │         │  │ (web)        │   │ │ │ │
│  ┌──────────────┐   │         │         │          │         │  └──────────────┘   │ │ │ │
│  │ HCA Claim    │───┼─────────┼─────────┼──────────┼────────▶│                     │ │ │ │
│  │ Event        │   │         │         │          │         │  ┌──────────────┐   │ │ │ │
│  └──────────────┘   │         │         │          │         │  │ Digital card │───┼─┼─┼─┼─┐
│                     │         │         │          │         │  │ Domain       │   │ │ │ │ │
│  ┌──────────────┐   │         │         │          │         │  └──────────────┘   │ │ │ │ │
│  │ Hugo Batch   │───┼─────────┼─────────┼──────────┼────────▶│                     │ │ │ │ │
│  └──────────────┘   │         │         │          │         │  ┌──────────────┐   │ │ │ │ │
└──────────────────────┘         └─────────┼──────────┘         │  │     BCA      │───┼─┼─┼─┼─┼─┐
                                          │                    │  └──────────────┘   │ │ │ │ │ │
                                          │                    │                     │ │ │ │ │ │
                                          │                    │  ┌──────────────┐   │ │ │ │ │ │
                                          │                    │  │ Corporate    │───┼─┼─┼─┼─┼─┼─┐
                                          │                    │  │ (CPC) Domain │   │ │ │ │ │ │ │
                                          │                    │  └──────────────┘   │ │ │ │ │ │ │
                                          │                    │                     │ │ │ │ │ │ │
                                          │                    │  ┌──────────────┐   │ │ │ │ │ │ │
                                          │                    │  │ Optical BFF   │───┼─┼─┼─┼─┼─┼─┼─┐
                                          │                    │  │ API          │   │ │ │ │ │ │ │ │
                                          │                    │  └──────────────┘   │ │ │ │ │ │ │ │
                                          │                    │                     │ │ │ │ │ │ │ │
                                          │                    │  ┌──────────────┐   │ │ │ │ │ │ │ │
                                          │                    │  │ Titanium     │───┼─┼─┼─┼─┼─┼─┼─┼─┐
                                          │                    │  │ APIs         │   │ │ │ │ │ │ │ │ │
                                          │                    │  └──────────────┘   │ │ │ │ │ │ │ │ │
                                          │                    └──────────────────────┘ │ │ │ │ │ │ │
                                          │                                                 │ │ │ │ │ │ │
                                          │                    ┌───────────────────────────┼─┼─┼─┼─┼─┘
                                          │                    │                           │ │ │ │ │
                                          │                    │                           │ │ │ │ │
                                          ▼                    ▼                           │ │ │ │ │
                          ╔═══════════════════════════════════════════════════════════════════╗
                          ║   SCOPE OF THIS INITIATIVE - DECOMMISSION (Red Dashed Box)        ║
                          ║                                                                   ║
                          ║                    ┌──────────────────┐                          ║
                          ║                    │  HUGO SERVICE    │                          ║
                          ║                    │  (WCF Legacy)    │                          ║
                          ║                    └────────┬─────────┘                          ║
                          ╚═════════════════════════════╪═══════════════════════════════════╝
                                                        │
                                                        │
                                                        ▼
                          ╔═══════════════════════════════════════════════════════════════════╗
                          ║   SCOPE OF THIS INITIATIVE - MODIFICATION (Red Dashed Box)       ║
                          ║                                                                   ║
                          ║                    ┌──────────────────┐                          ║
                          ║                    │   DOMAIN APIs    │                          ║
                          ║                    └────────┬─────────┘                          ║
                          ║                             │                                    ║
                          ║         ┌───────────────────┼───────────────────┐              ║
                          ║         │                   │                   │              ║
                          ║         ▼                   ▼                   ▼              ║
                          ║  ┌──────────┐      ┌──────────┐      ┌──────────┐            ║
                          ║  │ Payment  │      │  Sales   │      │ Customer │            ║
                          ║  │ Domain   │      │ Domain   │      │ Domain   │            ║
                          ║  │ API      │      │ API      │      │ API      │            ║
                          ║  └──────────┘      └──────────┘      └──────────┘            ║
                          ║         │                   │                   │              ║
                          ║         └───────────────────┼───────────────────┘              ║
                          ║                             │                                    ║
                          ║         ┌───────────────────┼───────────────────┐              ║
                          ║         │                   │                   │              ║
                          ║         ▼                   ▼                   ▼              ║
                          ║  ┌──────────┐      ┌──────────┐      ┌──────────┐            ║
                          ║  │Membership│      │Interaction│      │  Claim   │            ║
                          ║  │ Domain   │      │ Domain    │      │ Domain   │            ║
                          ║  │ API      │      │ API       │      │ API      │            ║
                          ║  └──────────┘      └──────────┘      └──────────┘            ║
                          ║                             │                                    ║
                          ║                             ▼                                    ║
                          ║                      ┌──────────┐                               ║
                          ║                      │Reference │                               ║
                          ║                      │Domain API│                               ║
                          ║                      └──────────┘                               ║
                          ╚═══════════════════════════════════════════════════════════════════╝
                                                        │
                                                        │
                                                        ▼
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                                    DATABASES & SERVICES                                       │
└─────────────────────────────────────────────────────────────────────────────────────────────┘

        ┌──────────────────────┐         ┌──────────────────────┐         ┌──────────────────────┐
        │                      │         │                      │         │                      │
        │   ┌──────────────┐   │         │   ┌──────────────┐   │         │   ┌──────────────┐   │
        │   │    HUGO     │   │         │   │     BOSS     │   │         │   │     WDM      │   │
        │   │  (Database) │   │         │   │  Enterprise  │   │         │   │  WebHandle   │   │
        │   │             │   │         │   │   Service    │   │         │   └──────┬───────┘   │
        │   └──────┬──────┘   │         │   └──────┬──────┘   │         │          │           │
        │          │           │         │          │           │         │          ▼           │
        │          │           │         │          │           │         │   ┌──────────────┐   │
        │          │           │         │          │           │         │   │     WDM      │   │
        │          │           │         │          │           │         │   │  (Database)   │   │
        │          │           │         │          │           │         │   └──────────────┘   │
        └──────────┼───────────┘         └──────────┼───────────┘         └──────────────────────┘
                   │                                 │
                   │                                 │
                   └─────────────────┬───────────────┘
                                     │
                                     │
        ╔═══════════════════════════════════════════════════════════════════╗
        ║   SCOPE OF THIS INITIATIVE - DECOMMISSION (Red Dashed Box)        ║
        ║                                                                   ║
        ║         ┌──────────────┐              ┌──────────────┐           ║
        ║         │    MSMQ     │◄────────────▶│ Hugo Sync    │           ║
        ║         │  (Message   │              │ Service      │           ║
        ║         │  Queue)     │              └──────┬───────┘           ║
        ║         └──────────────┘                     │                   ║
        ╚═══════════════════════════════════════════════╪═══════════════════╝
                                                        │
                                                        │
                                                        ▼
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                              EXTERNAL INTEGRATIONS                                            │
└─────────────────────────────────────────────────────────────────────────────────────────────┘

        ┌──────────────────────┐         ┌──────────────────────┐
        │                      │         │                      │
        │ Address Validation   │         │  External to Bupa    │
        │                      │         │                      │
        └──────┬───────────────┘         │  ┌──────────────┐   │
               │                         │  │   Stripe     │   │
               │                         │  └──────────────┘   │
               │                         │                     │
               └─────────────────────────▶│  ┌──────────────┐   │
                                         │  │   PayPal     │   │
                                         │  └──────────────┘   │
                                         └──────────────────────┘
```

### Diagram Explanation

#### **Top Layer - Input Sources:**
1. **External Systems**: IVR, Comparators, BizTalk, HCA Claim Event, Hugo Batch
2. **DMZ Layer**: Security layer with IVR Wrapper and Relay Facade
3. **Bupa Systems**: Frontend applications (Cyclops, MyBupa mobile/web, Digital card, BCA, Corporate CPC, Optical BFF, Titanium APIs)

#### **Center - Core Processing:**
1. **Hugo Service (Red Box - Decommission)**:
   - Legacy WCF service receiving all inputs
   - Currently handles all core business logic
   - **Status**: To be decommissioned

2. **Domain APIs (Red Box - Modification)**:
   - **Payment Domain API**: Payment processing
   - **Sales Domain API**: Sales and membership acquisition
   - **Customer Domain API**: Customer management (M2 - Complete)
   - **Membership Domain API**: Membership management
   - **Interaction Domain API**: Customer interactions
   - **Claim Domain API**: Claim processing
   - **Reference Domain API**: Document rendering, email, storage (M2 - Complete)

#### **Bottom Layer - Data & Services:**
1. **Databases**:
   - **Hugo Database**: Main data store
   - **WDM Database**: Data warehouse

2. **Services**:
   - **Boss Enterprise Service**: Enterprise integration
   - **WDM WebHandle**: Data warehouse interface

3. **Decommission Scope (Red Box)**:
   - **MSMQ**: Legacy message queue (to be removed)
   - **Hugo Sync Service**: Legacy sync service (to be removed)

4. **External Integrations**:
   - **Address Validation**: Address verification service
   - **External to Bupa**: Payment gateways (Stripe, PayPal)

### Key Points:
- **Red Dashed Boxes** indicate initiative scope (Decommission vs Modification)
- **Arrows** show data flow and dependencies
- **Hugo Service** is the central legacy component being replaced
- **Domain APIs** are the modern replacement organized by business domain
- All systems currently connect to Hugo Service, but will migrate to Domain APIs

## Current State Overview

### Hugo Service - The Centerpiece
- **Technology**: Built using **WCF (Windows Communication Foundation)** - a legacy framework
- **Role**: Currently performs the core business logic and operations
- **Status**: This is the system being **decommissioned** and replaced by modern Domain APIs

### System Architecture - Current View

```
┌─────────────────────────────────────────────────────────────┐
│                    Current System (Legacy)                  │
└─────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│   External    │    │   Bupa        │    │   DMZ         │
│   Systems     │    │   Systems     │    │   Layer       │
│               │    │               │    │               │
│ - IVR         │    │ - Cyclops     │    │ - IVR Wrapper │
│ - Comparators │    │ - MyBupa      │    │ - Relay       │
│ - BizTalk     │    │   (mobile)    │    │   Facade      │
│ - HCA Claim   │    │ - MyBupa      │    │               │
│   Event       │    │   (web)       │    │               │
│ - Hugo Batch  │    │ - Digital     │    │               │
│               │    │   card Domain │    │               │
└───────┬───────┘    └───────┬───────┘    └───────┬───────┘
        │                    │                    │
        └────────────────────┼────────────────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │  Hugo Service   │ ◄─── TO BE DECOMMISSIONED
                    │  (WCF Legacy)   │
                    └────────┬────────┘
                             │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│   Hugo DB     │    │   Boss        │    │   WDM         │
│   (Database)  │    │   Enterprise  │    │   WebHandle   │
│               │    │   Service     │    │               │
└───────────────┘    └───────────────┘    └───────────────┘
```

## Migration Strategy - Three Milestones

### Milestone 1: Foundation ⚙️

**Purpose**: Establish the core infrastructure and cross-cutting concerns

**Components Developed**:
1. **Authentication & Authorization**
   - Security framework
   - User authentication mechanisms
   - Role-based access control

2. **Logging Components**
   - Centralized logging infrastructure
   - Log aggregation and monitoring

3. **Exception Handling**
   - Global exception handling framework
   - Error management and reporting

4. **Caching**
   - Caching mechanisms
   - Performance optimization layer

5. **API Specifications Baseline**
   - API documentation standards
   - Contract definitions
   - API versioning strategy

6. **Monitoring & Observability**
   - System health monitoring
   - Performance metrics
   - Alerting mechanisms

**Deliverables**:
- NuGet packages (reusable components)
- Shared libraries and frameworks
- Infrastructure components

**Key Point**: These are **cross-cutting concerns** that will be used by all Domain APIs

---

### Milestone 2: MVP (Minimum Viable Product) 🚀

**Purpose**: Extract and transform specific functionality from Hugo Service into modern Domain APIs

#### Domain APIs Developed in M2:

##### 1. **Customer Domain API**
- **Endpoints**: ~20 endpoints extracted from Hugo Service
- **Functionality**: Customer information management
- **Status**: Development complete, currently in **testing phase**
- **Integration**: Connected with Cyclops and Apollo BFF

##### 2. **Reference Domain API**
- **Purpose**: Different from core business functionality
- **Features**:
  - Document rendering
  - Email functionality
  - Document storage
  - Additional utility functions

#### Integration Points (M2):
- **Cyclops**: Integrated with Customer Domain API endpoints
- **Apollo BFF**: Backend for Frontend layer integrated with Customer Domain API

#### M2 Status:
- ✅ Development: Complete
- 🔄 Testing: In progress
- 📦 Deployment: Will be deployed to **production**
- ⚠️ **Limitation**: Not fully usable in production (only 20 endpoints transformed)
- 🎯 **Purpose**: Serves as **MVP/Proof of Concept** for stakeholders

**Key Point**: M2 demonstrates the migration approach and validates the architecture

---

### Milestone 3: Full Implementation 🎯

**High-Level Purpose**: Complete the migration by:
- Extracting real business logic from **Hugo Service** into the **right Domain APIs**
- Changing **consumers** (like Cyclops and BFFs) so they start calling the new APIs
- Finally **switching off** legacy pieces (Hugo, MSMQ, Hugo Sync Service)

#### 1. Business Logic Extraction from Hugo
- Today: Hugo Service is the **single core service** for upstream systems (Cyclops, Apollo BFF etc.)
- For example, **Cyclops** (Windows Forms desktop app for business users):
  - Calls Hugo for all **server-side operations**
  - For customer operations, it calls a **customer service inside Hugo**
  - Example: *GetCustomer* – Cyclops sends a request, Hugo does the search, and returns a response in a **Hugo-specific format**
- Problem: Existing Domain APIs (like Customer) are mostly **pass-through**:
  - Take request from consumer → forward to some other system → return response
  - They do **not own customer logic** yet
- Milestone 3 goal:
  - **Move real logic** (search, update, claim operations, etc.) out of Hugo into the **correct Domain APIs**
  - So **Customer Domain API** really owns *customer* behaviour, **Claim Domain API** owns *claim* behaviour, etc.

#### 2. Proper Domain API Design (Not 1:1 Copy from Hugo)
- You **cannot just copy** Hugo endpoints “as is”:
  - Hugo’s *GetCustomer* response shape is tailored to Cyclops
  - Request/response models in Hugo are not ideal for clean domain boundaries
- In M3, for each domain (Customer, Claim, etc.) the team must decide:
  - **What endpoints** should exist (e.g. `GET /customers/{id}`, `GET /customers?search=...`)
  - **What request model** each takes
  - **What response model** they return
- Key design rule:
  - Avoid **duplicate / overloaded endpoints** with slightly different parameter sets (like we often do with methods in classes)
  - Instead, **model resources and use clear contracts** so each operation/end-point has a clear responsibility

#### 3. Consumer Impact – Especially Cyclops and BFF
- Cyclops currently:
  - Expects Hugo-style responses for calls like *GetCustomer*
  - Knows Hugo-specific request and response shapes
- When moving to Domain APIs:
  - Cyclops (and BFFs like Apollo) must be **changed to call the new endpoints**
  - They must also be updated to **handle the new response shapes**
  - This work is called **integration tasks** in the KT
- So in M3 there are always **two parallel activities**:
  1. **Endpoint development** in Domain APIs (implementing business logic previously inside Hugo)
  2. **Consumer integration changes** (Cyclops, BFF) to use these new endpoints

#### 4. Scope of Milestone 3
- **Remaining / Enhanced Domain APIs**:
  - Payment Domain API
  - Sales Domain API
  - Membership Domain API
  - Interaction Domain API
  - Claim Domain API
  - Premium Domain API (if not already covered)
  - Completion of functionality for:
    - Customer Domain API (beyond the ~20 endpoints in M2)
    - Reference Domain API
- **Complete Integration**:
  - All Bupa systems (Cyclops, MyBupa, BFFs, etc.) wired to Domain APIs
  - All external systems integrated through the new architecture
- **Decommissioning**:
  - Hugo Service (WCF legacy) no longer used
  - MSMQ and Hugo Sync Service decommissioned

**Status**: This is the milestone the current team (including you) is mainly working on.

---

## Milestone 1 – Auth, Token & Authorization Flow (Detailed KT Notes)

### Token Refresh Strategy (Cyclops – User-Interactive App)
- **Scenario**: Cyclops is a **user-interaction desktop app** (Windows Forms). Users are actively logged in and using the UI.
- **Problem**: Access tokens expire; we must **refresh without showing login popup again**.
- **Solution in M2/M3 (for Cyclops)**:
  - Uses a library (mentioned as *Pentra* in KT) that can:
    - Maintain an **application token cache**
    - Automatically **refresh tokens** when needed
  - Token cache is linked to the **app registration**:
    - Either Cyclops app registration
    - Or more commonly the **Domain API app registration**
  - If a 401 (Unauthorized) is received:
    - That means the token is invalid/expired → app must **acquire a new token**
  - For safety, Cyclops implements **two mechanisms**:
    1. **On-demand refresh** – manual trigger when needed
    2. **Automatic scheduled refresh** – based on token expiry time:
       - Reads token expiry
       - A few minutes before expiry, silently calls refresh
       - Ensures **no login box** is shown again to the user
  - Exact buffer time (e.g. how many minutes before expiry) was tuned by the M2 team (Amit).

### Token Handling for Non-UI / Service-to-Service Calls
- For **non-user interaction systems** (no direct user, pure backend/API to API):
  - Same **“get token → call API → refresh as needed”** concept applies
  - But refresh is handled **purely by the backend** using application credentials + token cache
  - Another separate flow (not fully detailed here) exists for these **service-to-service** calls.

### Token Validation vs Authorization (Inside Domain APIs)

Once a request reaches a Domain API (e.g. **Customer Domain API**), an M1 package handles:

1. **Token Validation** (Step #11 in KT diagram):
   - Check **authenticity** of the token:
     - Issuer is correct (trusted identity provider)
     - Signature is valid
   - Check **audience**:
     - The token is *intended for this API* (audience/`aud` claim matches)
   - Check token is **not expired** and structurally valid

2. **Authorization** (Step #12 in KT diagram):
   - Verify presence of **required roles/scopes** for the requested endpoint
   - Example:
     - `GET /customers/{id}` (read operation) → token must have **read scope/role**
   - If scopes/roles are missing:
     - Request is rejected (authorization failure)

Both **token validation** and **authorization** are handled by a **shared M1 foundation package** (OPA R3 referenced in KT), so all Domain APIs behave consistently.

### Important Clarifications from KT
- **Authorization inside Cyclops**:
  - There is an **internal authorization concept within Cyclops** itself.
  - This is **separate** from the M1 foundation authorization in Domain APIs.
  - It does **not depend on** M1 components; it’s a client-side concern (to be detailed separately later).
- **Customer Domain API flow**:
  - Cyclops gets an access token → calls **Customer Domain API**
  - M1 package:
    - Validates token (issuer, signature, audience, expiry)
    - Checks required roles/scopes for the specific endpoint

---

## Distributed Tracing & Logging (Foundation Capability)

### What Distributed Tracing Shows
- The M1 **logging package** provides **end-to-end tracing** of a request across:
  - Domain APIs
  - Service Bus queues
  - Downstream APIs (for example, **Boss API**)
  - Processing queues / worker services
- Example flow from the KT demo:
  1. A **message is created and put into a Service Bus queue**.
  2. A **Boss API call** is made (shown as “Boss API Call” in the trace).
  3. The message is **moved to another queue**.
  4. Additional checks/operations run on each queue.
  5. Finally, the message **reaches a processing queue** and is completed.

### How It Appears in Traces
- Each step is logged as a **separate trace entry/span**:
  - Queue send / receive
  - HTTP API calls
  - Domain API operations
- For a Domain API request you would see:
  - A **top-level entry** for the incoming Domain API call.
  - Child entries when it **puts messages on queues**.
  - Additional entries for **outbound API calls** (e.g., to Boss Enterprise Service).
- For each entry you can inspect:
  - Operation name (e.g., “Boss API Call”, “Message enqueued to queue X”)
  - Status (success/failure, HTTP status code)
  - Metadata (queue name, correlation IDs, etc.).

### Why This Matters
- Lets you **trace a business operation end-to-end**:
  - From Domain API → queues → downstream services → final processing
- Helps answer:
  - *Where did this message go?*
  - *Which step failed or was slow?* (queue, API, or handler)
- This distributed tracing comes from the **logging foundation built in M1**, and will be key for debugging M2/M3 flows.

---

## Milestone 3 – MSMQ to Azure Service Bus & Boss Sync (Detailed Flow)

### High-Level Idea
- **Current legacy flow**:
  - Upstream apps (Cyclops, Apollo CRM, Bupa.com, other frontends/tools) call **Hugo WCF services**.
  - Hugo / Windows services push **asynchronous messages** to **MSMQ**.
  - Windows services read from MSMQ and call **Boss Enterprise Service** (WCF/legacy).
  - Responses (success/failure) flow back; for each business operation (e.g. **new member enrolment**) a separate message is created.
- **Target flow (M3)**:
  - Keep business functionality the **same**, but **replace MSMQ with Azure Service Bus** and **Windows services with Azure Functions**.
  - No meaningful functional change, only **transport and hosting technology** changes.

### Simple Target Architecture Diagram (Based on Screenshot)

```
          ┌───────────┐
          │  Apollo   │
          └────┬──────┘
               │
               ▼
        ┌──────────────┐
        │  Domain APIs │
        └────┬─────────┘
               │
               ▼
          ┌───────────┐
          │ Hugo      │  ◄─ Also called “Hugo Services” (WCF)
          │ Services  │
          └────┬──────┘
     ┌─────────┼───────────┐
     │         │           │
     ▼         ▼           ▼
 ┌────────┐ ┌────────┐ ┌──────────────┐
 │Cyclops│ │Batch   │ │ Sync Admin   │
 │       │ │ Jobs   │ │ Tool         │
 └───┬───┘ └───┬────┘ └─────┬────────┘
     │         │            │
     └─────────┼────────────┘
               ▼
      ┌───────────────────┐
      │ Service Bus       │
      │ Queues            │
      └────────┬──────────┘
               │
               ▼
        ┌──────────────┐
        │ Function     │
        │ Apps (Azure) │
        └────┬─────────┘
               │
               ▼
        ┌──────────────┐
        │ Boss Service │
        └──────────────┘
```

### MSMQ → Service Bus Migration (What Changes)
- **Before (Legacy)**:
  - Hugo services / Windows services push messages to **MSMQ** queues.
  - Windows services read from MSMQ and call **Boss Enterprise Service**.
- **After (M3)**:
  - Hugo still creates messages, but now pushes them to **Azure Service Bus queues** instead of MSMQ.
  - **Azure Functions** (Listener, Processor, Wait Timer) replace Windows services:
    - All are **queue-triggered** or **timer-triggered** functions.
  - Messages still ultimately go to **Boss Service**, which remains WCF.
  - Core business behaviour is preserved; only **transport and plumbing** are modernized.

### Azure Service Bus Queues & Functions (Detailed Flow)

**Queues involved (in both MSMQ and Service Bus designs):**
- `In Queue`
- `Wait Queue`
- `Process Queue`
- `Error Queue`

**Functions:**
- **Listener Function** (Queue trigger on `In Queue`)
- **Processor Function** (Queue trigger on `Process Queue`)
- **Wait Timer Function** (Timer trigger, checks `Wait Queue`)

#### 1. From Hugo to In Queue
- Cyclops / Apollo / other upstream apps call **Hugo WCF services**.
- Hugo builds a **SOAP request body** (XML) and wraps it as a **message**.
- The message is sent to **`In Queue`** on Azure Service Bus.

#### 2. Listener Function (Queue-Triggered)
- Trigger: Message appears in `In Queue`.
- Steps:
  1. **Check Process Queue**:
     - Looks in **`Process Queue`** for an existing message with the **same membership/session ID**.
     - Queues are **session-enabled**, so ordering and grouping by membership/session is preserved.
  2. **Check Error Queue**:
     - If not in Process Queue, checks **`Error Queue`** for a message with the **same membership/session ID**.
     - Scenario: A previous call for this membership failed and was moved to `Error Queue`.
       - In that case, **subsequent messages should NOT be processed** yet.
  3. Routing decision:
     - If a related message exists in `Process Queue` or `Error Queue`:
       - New message is sent to **`Wait Queue`** (to wait until issues are resolved).
     - If **no related messages** exist in any of the three queues:
       - Message is sent to **`Process Queue`**.

#### 3. Processor Function (Queue-Triggered)
- Trigger: Message appears in **`Process Queue`**.
- Steps:
  1. Reads the message (`ServiceBusReceivedMessage`).
  2. Extracts **message type** (one of ~12–14 **processor agents**).
  3. Resolves the correct **agent** at runtime:
     - Each agent corresponds to a specific **Boss operation** (e.g., **new enrolment**, etc.).
  4. Uses a **converter**:
     - Input: XML SOAP-style request from Hugo.
     - Output: Boss-specific request format.
  5. Calls **Boss Service** (WCF/SOAP).
  6. On **success**:
     - Message is **completed/removed** from `Process Queue`.
  7. On **failure/exception**:
     - Message is moved to **`Error Queue`**.

#### 4. Sync Admin Tool & Error Handling
- **Sync Admin Tool**:
  - Windows UI used by support team.
  - Can **view and manage messages** in all four queues:
    - `In Queue`, `Wait Queue`, `Process Queue`, `Error Queue`.
  - For error cases:
    - Support team manually **inspect error messages** in `Error Queue`.
    - Fix data/configuration issues.
    - Manually move / re-queue messages (e.g., from `Error Queue` to `Wait Queue` or `Process Queue`).
- Goal:
  - Ensure **subsequent messages** for the same membership aren’t processed until **older error messages** are handled.

#### 5. Wait Timer Function (Timer-Triggered)
- Trigger: **Timer**, e.g., every **30 seconds**.
- Purpose:
  - Ensure **`Wait Queue`** is not permanently stuck with messages.
- Steps:
  1. Periodically **polls `Wait Queue`** for messages.
  2. For each message:
     - Checks whether a **related message still exists in `Error Queue`** (for same membership/session).
     - If an error still exists:
       - Message stays in **`Wait Queue`**.
     - If **no error message** exists:
       - Message is moved to **`Process Queue`**.
  3. Processor Function then handles it as usual and calls **Boss Service**.

### Service Bus Concepts Mentioned
- **Queue**:
  - Standard asynchronous messaging pattern (FIFO).
- **Topic**:
  - For scenarios with **multiple subscribers** (not used here).
- **Relay**:
  - For **on-premise integration scenarios** (not used here).
- **Sessions**:
  - Used to group messages (e.g., by membership ID) and enforce strict **ordering**.
  - Implementation/handling is slightly different for **session-enabled queues**, especially when reading messages.

### Configuration & Deployment Notes
- Queue names and connection strings are **configurable**:
  - Locally via `local.settings.json` in Function Apps.
  - In Azure via **environment variables / app settings** (never hard-coded).
- Processor Function signature:
  - Uses **Service Bus connection** and **queue name** from config.
  - Trigger attribute wires the function to **`Process Queue`**.

---

## Implementation Standards & Recommended Prep

### Implementation Standards (From KT)
- **Logging**:
  - All Function Apps (Listener, Processor, Wait Timer, etc.) use the **common logging package**.
  - Integrated with **Application Insights** for telemetry and distributed tracing.
  - Processor completion and failure paths both log using standard log helpers.
- **Configuration & Secrets**:
  - Queue names, connection strings, and other settings are **not hard-coded**.
  - Values are taken from:
    - `local.settings.json` in local development.
    - **App settings / environment variables** in Azure.
  - **Secrets** (e.g., connection strings, keys) are stored in **Azure Key Vault**, referenced from app settings.
- **Retry & Resilience**:
  - `ServiceBusClient` is configured with **retry options** (mode, max retries, delay, timeout).
  - Processor logic differentiates **Completed** vs **Exception** cases and routes messages accordingly (processed vs error queue).

### What You Should Learn / Practice
- For **M3 work** your main Azure focus areas are:
  - **Azure Service Bus**:
    - Queues (including **session-enabled** queues).
    - Dead-letter queues (DLQ).
  - **Azure Functions**:
    - Queue triggers.
    - Timer triggers.
    - Basic configuration (bindings, app settings).
- For **M1/M2** background:
  - Domain-driven design basics.
  - REST vs SOAP (Domain APIs vs WCF services).
- Suggested approach from KT:
  - Don’t just read docs – **build small POCs**:
    - Create a simple Service Bus queue + Function App with queue trigger and timer trigger.
    - Experiment with sending, processing, and dead-lettering messages.
  - Use the **project codebase** later to see the “real” implementation patterns.

---

## Session Wrap-Up (From KT Conversation)

- The overall architecture and milestones look **simple at high level**, but:
  - M2 and M3 involve **many moving parts** and **non-trivial integration work**.
- **M1, M2, M3 detailed sessions**:
  - M2 session planned on **Wednesday**
  - M3 session planned on **Thursday**
- **M2 key people** (as per KT):
  - **Praveen** – leads **Reference Domain API** and related work
  - **Amit** – leads **integration work** (token refresh, Cyclops changes, etc.)
  - **Raja** – works on **Cyclops** integration
- **M3 key people**:
  - “Superman” **Sivam** (name sounded like *Sivam/Sivan* in KT) – handles **Function Apps** and M3 work
- **Documentation / Resources**:
  - There is a **Teams “Foundation” folder** and **Discovery phase documents**
  - Discovery started around **July 2024**
  - Foundation phase from **November 2024**
  - Implementation phase expected to continue into **Feb 2026**
  - Some documents may be **partially relevant** and need curation before sharing
- **How to get help**:
  - You can **reach out to Gouri** (and others like Nitin Xavier, Praveen, Amit, Raja, Sivam) for clarifications
  - Additional docs will be shared to all attendees after internal review

---

## Target Architecture - Domain APIs

```
┌─────────────────────────────────────────────────────────────┐
│                    Target System (Modern)                   │
└─────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│   External    │    │   Bupa        │    │   BFF Layer   │
│   Systems     │    │   Systems     │    │               │
│               │    │               │    │ - Apollo      │
│ - IVR         │    │ - Cyclops     │    │ - Other BFFs  │
│ - Comparators │    │ - MyBupa      │    │               │
│ - BizTalk     │    │   (mobile)    │    │               │
│ - HCA Claim   │    │ - MyBupa      │    │               │
│   Event       │    │   (web)       │    │               │
│ - Hugo Batch  │    │ - Digital     │    │               │
│               │    │   card Domain │    │               │
└───────┬───────┘    └───────┬───────┘    └───────┬───────┘
        │                    │                    │
        └────────────────────┼────────────────────┘
                             │
                             ▼
        ┌─────────────────────────────────────────┐
        │     Domain APIs (Modern Architecture)    │
        │  ┌─────────────────────────────────────┐ │
        │  │  Customer Domain API (M2 - Done)   │ │
        │  │  Reference Domain API (M2 - Done)  │ │
        │  │  Payment Domain API (M3 - In Dev)  │ │
        │  │  Sales Domain API (M3 - In Dev)    │ │
        │  │  Membership Domain API (M3 - In Dev)│ │
        │  │  Interaction Domain API (M3 - In Dev)│ │
        │  │  Claim Domain API (M3 - In Dev)     │ │
        │  │  Premium Domain API (M3 - In Dev)   │ │
        │  └─────────────────────────────────────┘ │
        └───────────────┬───────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌───────────────┐ ┌───────────────┐ ┌───────────────┐
│   Hugo DB     │ │   Boss        │ │   External    │
│   (Database)  │ │   Enterprise  │ │   Services    │
│               │ │   Service     │ │   (Stripe,    │
│               │ │               │ │    PayPal)    │
└───────────────┘ └───────────────┘ └───────────────┘
```

## Key Technical Concepts

### WCF (Windows Communication Foundation)
- **What**: Legacy Microsoft framework for building service-oriented applications
- **Why being replaced**: Outdated technology, difficult to maintain, limited scalability
- **Replacement**: Modern REST APIs (Domain APIs)

### Domain APIs
- **What**: Microservices-style APIs organized by business domain
- **Benefits**:
  - Better separation of concerns
  - Easier to maintain and scale
  - Modern technology stack
  - Independent deployment

### BFF (Backend for Frontend)
- **What**: Layer between frontend applications and Domain APIs
- **Examples**: Apollo BFF, Cyclops
- **Purpose**: Aggregates data from multiple Domain APIs for frontend consumption

### MSMQ (Microsoft Message Queuing)
- **What**: Legacy messaging system
- **Status**: Part of decommissioning scope
- **Replacement**: Modern messaging/event-driven architecture

### Hugo Sync Service
- **What**: Service that synchronizes data
- **Status**: Part of decommissioning scope
- **Replacement**: Modern synchronization mechanisms in Domain APIs

## Migration Flow

```
Current State (Legacy)
        │
        ▼
┌───────────────────┐
│  Milestone 1      │
│  Foundation       │
│  (Infrastructure)│
└─────────┬─────────┘
          │
          ▼
┌───────────────────┐
│  Milestone 2      │
│  MVP              │
│  - Customer API   │
│  - Reference API  │
│  - 20 endpoints   │
└─────────┬─────────┘
          │
          ▼
┌───────────────────┐
│  Milestone 3      │
│  Full Migration   │
│  - All Domain APIs│
│  - All endpoints  │
│  - Decommission  │
│    Hugo Service   │
└─────────┬─────────┘
          │
          ▼
   Target State
   (Modern System)
```

## Important Notes

### M2 Status Summary:
- ✅ **Completed**: Customer Domain API (~20 endpoints), Reference Domain API
- 🔄 **Testing**: Currently in testing phase
- 📦 **Deployment**: Will go to production (but not fully usable)
- 🎯 **Purpose**: MVP to demonstrate approach to stakeholders

### Current Team Focus (M3):
- Developing remaining Domain APIs
- Completing remaining endpoints for Customer and Reference APIs
- Full integration with all systems
- Complete decommissioning of Hugo Service

### Key Takeaways:
1. **Hugo Service** is the legacy WCF system being replaced
2. **Foundation (M1)** provides infrastructure for all APIs
3. **MVP (M2)** validates the approach with 2 Domain APIs
4. **Full Implementation (M3)** completes the migration
5. **Domain APIs** are organized by business functionality
6. **BFF layer** (Apollo, Cyclops) sits between frontend and Domain APIs

---

*Technical notes extracted from KT session with Gouri Priya Jyothi L*

