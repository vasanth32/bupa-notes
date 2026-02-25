## Domain APIs & integrations (who owns what)

### Why “Domain APIs” exist

Domain APIs are intended to be:

- **Business-capability APIs** (DDD-style) that own their domain behaviour
- Independently deployable services (microservice-style)
- The long-term contract boundary for upstream consumers (Cyclops, MyBupa, BFFs, etc.)

In early phases some endpoints may still proxy Hugo WCF, but the target is for each Domain API to contain the real business logic and data access.

---

## Domain APIs (functional scope)

This list merges what appears in KT notes and the Confluence project overview.

- **Customer Domain API**
  - Customer profile, customer types (including **OSHC**), customer search/read/update flows
  - KT notes: MVP endpoints were implemented here (≈20)

- **Reference Domain API**
  - Document rendering/generation, email, document storage, and “utility/reference” capabilities
  - KT notes: MVP implementation exists

- **Membership Domain API**
  - Membership / policy lifecycle (create/update/renewal), membership details

- **Sales Domain API**
  - Sales process and membership acquisition workflows

- **Payment Domain API**
  - Payment methods, transactions, refunds
  - Integrations may include external payment providers (PDF mentions Stripe/PayPal in KT notes)

- **Claim Domain API**
  - Claim submission/processing, provider interactions, refunds/settlements

- **Interaction Domain API**
  - Customer interactions, communications tracking (depends on actual backlog/design)

- **Provider Domain API**
  - Provider-related operations (TBA at implementation level; named as in-scope on Confluence export)

- **Streaming / Data Stream Domain API**
  - Data streaming to/from upstream platforms (named on Confluence export; details depend on design)

- **Sync Domain API**
  - Synchronisation flows between systems (named on Confluence export; details depend on design)

---

## Upstream consumers (examples from KT)

Internal/partner-facing applications that consume the platform:

- **Cyclops**: desktop (Windows Forms) application used by business users
- **MyBupa**: web and mobile apps
- **BFF layer**: e.g., Apollo BFF (Backend-for-Frontend that aggregates Domain APIs for a specific frontend)
- **Digital card domain**, **BCA**, **Corporate (CPC)**, **Optical BFF API**, **Titanium APIs**

External systems called out in KT notes:

- **IVR** (via DMZ wrapper)
- **Comparators**
- **BizTalk**
- **HCA claim event** source
- **Hugo batch** jobs

### DMZ layer (security façade)

The DMZ layer sits between external systems and internal APIs/services:

- **IVR wrapper**
- **Relay façade**

Purpose: request validation, routing, security boundary, protocol translation, monitoring.

---

## Business domain in 10 lines (for new joiners)

Health insurance lifecycle (simplified):

1. Customer chooses a package/options
2. Premium is calculated (including rules like **no-claim bonus**)
3. Customer pays
4. Membership/policy becomes active
5. Digital card is generated
6. Customer uses healthcare services
7. Claim is submitted
8. Claim is assessed/processed
9. Refund/settlement occurs (if applicable)
10. Renewal occurs and feeds into next year’s premium rules

Key business term:

- **OSHC**: Overseas Student Health Cover (special customer category)

