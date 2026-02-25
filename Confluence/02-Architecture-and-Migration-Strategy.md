## Architecture & migration strategy (current → target)

### The mental model (1 minute)

This programme is moving BUPA Health Insurance from:

- **Legacy**: Hugo Core **WCF** services (on-prem, .NET Framework), plus **MSMQ** + Windows services for async processing
- **Modern**: **Domain APIs** (REST/JSON, modern .NET) hosted in **Azure**, plus **Azure Service Bus** + **Azure Functions** for async processing

The key design idea is **domain ownership**: each Domain API should eventually own its business logic and contracts instead of proxying Hugo.

### Current state (legacy)

- Many upstream apps (Cyclops, web/mobile, CRM, external systems) call **Hugo Service (WCF)** as the central integration + business logic layer.
- Hugo talks to internal data/services (Hugo DB, BOSS enterprise service, WDM, etc.).
- Async workflows use **MSMQ** and long-running **Windows services**.

### Target state (modern)

- Upstream apps call **Domain APIs** (Customer, Membership, Payment, Sales, Interaction, Claim, Reference, etc.).
- Domain APIs use shared “foundation” capabilities (auth, logging, exception handling, config).
- Async workflows are implemented with **Azure Service Bus queues** and **Azure Functions**.
- The legacy **Hugo WCF service** is decommissioned once consumers have migrated and the replacement functionality is complete.

### Who calls what (high-level)

- **External systems**: IVR, comparators, BizTalk, claim events, batch jobs
- **DMZ layer** (security + façade): IVR wrapper, relay façade
- **Internal consumers**: Cyclops (desktop), MyBupa (web/mobile), BFFs (e.g., Apollo BFF), other domains (digital card, optical, titanium APIs)

All of these are progressively migrated to call Domain APIs rather than Hugo WCF directly.

---

## Migration strategy: M1 / M2 / M3

### M1 — Foundation (cross-cutting capabilities)

Goal: make it easy to build consistent Domain APIs by providing reusable building blocks (often shipped as internal NuGet packages), such as:

- **Authentication & authorization**
- **Logging + distributed tracing** (Application Insights)
- **Exception handling**
- **Caching**
- **Configuration + secrets** (Key Vault integration patterns)

Outcome: new services can adopt the same standards quickly.

### M2 — MVP (prove the approach)

Goal: deliver a small but real slice to validate architecture and integration.

Typical characteristics in M2:

- A limited set of Domain APIs/endpoints are built (KT notes mention **Customer** and **Reference** with ~20 endpoints).
- Some endpoints behave like **proxies**: Client → Domain API → Hugo WCF (until the logic is fully migrated).
- Integration begins with key consumers (e.g., Cyclops and a BFF).

Outcome: stakeholders can see the approach working end-to-end, even if coverage is partial.

### M3 — Full migration (complete the move + decommission)

Goal: migrate the remaining business logic, rewire consumers, and retire legacy components.

Two streams typically happen in parallel for any domain:

1. **Domain API work**: implement the real business logic + data access inside the Domain API (not just forwarding to Hugo).
2. **Consumer integration work**: update clients (Cyclops, BFFs, etc.) to use the new endpoints/contracts.

Additionally for async processing:

- Replace **MSMQ** with **Azure Service Bus**
- Replace **Windows services** with **Azure Functions**

Outcome: consumers no longer depend on Hugo WCF, and legacy can be switched off safely.

---

## Design guidance (important in M3)

- Avoid “copy Hugo 1:1” endpoint designs.
- Prefer **resource-oriented REST** endpoints with clear request/response models.
- Treat Domain APIs as the long-term contract boundary; consumers should not need Hugo-specific models.

