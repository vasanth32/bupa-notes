## Onboarding: first-week checklist (practical)

This checklist is meant to help a new engineer become productive quickly on the Hugo Modernisation programme.

### Day 1: get the mental model right

- Read:
  - `01-Project-Overview.md`
  - `02-Architecture-and-Migration-Strategy.md`
  - `03-Domain-APIs-and-Integrations.md`
  - (Optional deep dive) `BUPA/KT/3-Technical-Architecture-KT-Notes.md`
- Be able to explain in your own words:
  - What is being **decommissioned** (Hugo WCF, MSMQ, Windows services)
  - What replaces it (Domain APIs, Service Bus, Functions)
  - What “M1/M2/M3” mean in practice

### Day 2: know your domain + upstream impact

- Identify which Domain API you’re working on (Customer/Payment/etc.).
- Check upstream owners:
  - `06-Stakeholders-and-Contacts.md`
  - `09-New-Upstream-Applications-Tracker.md`
- Understand “who breaks if we change this” before you code.

### Day 3: environment + repo access

- Confirm you can access environment dashboards and ADO repos:
  - `05-Environments-and-DevOps-Links.md`
- Note the environment codes used in conversation (D24/D34/D33/PERF4).
- Confirm you can find the correct build pipeline for your repo (by pipeline name/definitionId).

### Day 4: ways of working

- Read:
  - `07-Design-Review-Guidelines.md`
  - `08-How-We-Work-Scrum-Ceremonies.md`
- Ensure you follow the key guardrails:
  - no dev work before acceptance criteria are ready
  - avoid version numbers in URIs (project rule)
  - confirm auth headers (`SessionToken`) + APIM subscription key requirements

### Day 5: pick one end-to-end flow to trace

Pick a real scenario relevant to your domain and trace it end-to-end:

- **Sync/async path**: review `04-Async-Processing-MSMQ-to-ServiceBus.md`
- **API path**: trace Client → Domain API → downstream dependencies
- Confirm where logging/telemetry is visible (Application Insights) and what correlation IDs to use.

---

## “Fast learning” topics (high ROI)

- REST endpoint design in this programme (resource naming, error codes)
- Token handling at consumers vs token validation/authorization in APIs
- Service Bus basics (sessions, retries, DLQ, poison messages)
- How support teams handle Error/Wait queues (Sync Admin Tool concept)

