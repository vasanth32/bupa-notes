## Hugo Modernisation Project – Overview

### What this project is

The goal is to **modernise “Hugo”** by transforming legacy **Hugo WCF services** into a set of **domain-based REST APIs** (“Domain APIs”), hosted in Azure. Once consumers move to the new APIs, the legacy **Hugo WCF Service is intended to be decommissioned**.

In plain terms:

- **Before**: Many upstream apps call a large WCF-based Hugo service.
- **After**: Upstream apps call **Domain APIs** (REST/JSON). Domain APIs wrap/replace Hugo integration and provide a cleaner, reusable interface.

### Business value (why we are doing it)

- **Faster delivery**: reduce release-cycle bottlenecks.
- **Reduce technical debt**: move away from legacy patterns/tech.
- **Scalability and standardisation**: consistent, secure, reusable microservice APIs.

### Scope (from Confluence PDF)

- **API-led integration** using **domain-oriented APIs**, deployed to **Azure** (regions referenced: *Southeast AU* and *East AU*).
- **Modernise Hugo** as a domain-based REST service.
- Upgrade existing Hugo vertical component **NuGet packages to .NET 8.0**.
- Move **On-Prem MSMQ → Azure Service Bus** (async processing modernisation).

### Domain APIs in scope

The Confluence export lists these Domain APIs as in scope:

1. **Customer**
2. **Interaction**
3. **Reference**
4. **Membership**
5. **Sales**
6. **Claims** (noted as “TBA” in one place, but later shown in the timeline table)
7. **Payments**
8. **Provider**
9. **Streaming**
10. **Sync**

Related note from KT docs in `BUPA/KT`: the broader programme is migrating from **Hugo Core (WCF/.NET Framework/MSMQ)** to **Domain APIs + Azure Service Bus + Azure Functions**.

### Timelines (from Confluence PDF)

Overall phases:

- **Foundation**: Dec 2024 – Mar 2025 (**completed**)
- **Implementation**: Apr 2025 – Apr 2026 (**in progress**)
- **Closure**: May 2026 (**yet to commence**)

Domain API target dates (as captured in the PDF table; treat as indicative):

| Domain API | API build commencement | Development completion | Production go-live |
| --- | --- | --- | --- |
| Customer | In progress | Oct 2025 | Nov 2025 |
| Interaction | Aug 2025 | Aug 2025 | Sep 2025 |
| Reference | In progress | Dec 2025 | Jan 2026 |
| Membership | In progress | Dec 2025 | Jan 2026 |
| Sales | Aug 2025 | Oct 2025 | Dec 2025 |
| Payment | Oct 2025 | Dec 2025 | Jan 2026 |
| Provider | Oct 2025 | Dec 2025 | Jan 2026 |
| Streaming | In progress | Aug 2025 | Sep 2025 |
| Sync | In progress | Aug 2025 | Sep 2025 |
| Claims | Aug 2025 | Dec 2025 | Jan 2026 |

### Core team roles (from Confluence PDF)

Names were listed in the PDF; in day-to-day work, use the project’s official contact list/Teams channel for the latest ownership.

- **Program Manager**
- **Product Owner**
- **Solutions Architect**
- **Application Architect**
- **Business Analyst**
- **Project Manager**
- **Delivery Manager**
- **Developers**
- **Test support**
- **DevOps**
- **Release Manager**

