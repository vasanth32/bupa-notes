# Health Insurance Project - Technical Architecture & Migration Strategy

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

**Purpose**: Complete the migration by developing remaining Domain APIs and endpoints

#### Scope:
1. **Remaining Domain APIs**:
   - Payment Domain API
   - Sales Domain API
   - Membership Domain API
   - Interaction Domain API
   - Claim Domain API
   - Premium Domain API (if not already included)

2. **Additional Endpoints**:
   - Remaining endpoints for Customer Domain API
   - Additional endpoints for Reference Domain API

3. **Complete Integration**:
   - All Bupa Systems integration
   - All external system integrations
   - Full end-to-end functionality

4. **Decommissioning**:
   - Complete migration from Hugo Service
   - Decommission Hugo Service (WCF legacy)
   - Decommission MSMQ and Hugo Sync Service

**Status**: This is where the current team is working on

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

