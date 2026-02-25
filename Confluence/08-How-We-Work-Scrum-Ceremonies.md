## How we work: Scrum ceremonies & sprint process

Source: Confluence PDF “Hugo Modernisation Scrum Ceremonies and Sprint Process” exported on 2025-02-26.

### Summary (practical)

- Work is planned and tracked in **Azure DevOps (ADO)**.
- Sprints are **2 weeks**.
- Stories should not start until **acceptance criteria** are defined and endorsed.
- Story point estimation uses **Fibonacci**.
- Guideline: **max 13 points** per story; larger work must be split.

---

## Ceremonies (what + cadence)

| Ceremony | Purpose | Cadence (as described) |
| --- | --- | --- |
| Backlog refinement | Review/prioritise backlog, elaborate stories | Every fortnight; should happen ~2 weeks ahead of planning |
| Sprint planning | Commit to work for upcoming sprint | Day before sprint starts or first day |
| Daily scrum | Sync progress + blockers | Daily, ~15–30 minutes |
| Sprint review | Demo completed work + get stakeholder feedback | Last day of sprint (fortnightly) |
| Retrospective | Reflect + improve process | After each sprint, or monthly combining 2 sprints |

---

## Roles & responsibilities (as captured in the PDF)

- **Business Analyst (BA)**
  - Loads backlog into ADO with the defined hierarchy: **Epic → Feature → User stories**
  - Elaborates/refines stories with acceptance criteria
  - Captures integration impacts and upstream/downstream impacts

- **Product Owner (PO)**
  - Prioritises backlog with BA + dev lead
  - Reviews/endorses acceptance criteria

- **Scrum Master**
  - Facilitates ceremonies
  - Ensures RAID (Risks/Assumptions/Issues/Dependencies) items are captured in ADO
  - Schedules monthly RAID review

- **Dev lead + development team**
  - Provide technical inputs during refinement
  - Help break down large work into smaller deliverables
  - Estimate stories

- **Testing team**
  - Must join refinement and planning to understand integration impacts
  - Ensures testing planning includes impacted teams

---

## Working agreements / guardrails

- Do not start building a story until it is **refined** and acceptance criteria are defined.
- Acceptance criteria are defined by BA with Dev lead (and designer where needed) and sent to PO for endorsement.
- Dependencies and integration impacts must be documented (especially important in this programme due to many upstream systems).

---

## Notes on sprint calendar (as captured)

- Sprints start on a **Wednesday** and end on the **fortnight Wednesday**.
- Sprint planning should be completed **a week prior** to sprint commencement (guideline).

The PDF includes an example schedule around late April/early May (treat as historical guidance rather than a current commitment).

