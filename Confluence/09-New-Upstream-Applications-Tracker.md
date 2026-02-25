## New upstream applications tracker (inflight changes)

Source: Confluence PDF “New Upstream Applications” exported on 2025-02-26.

### What this tracker is for

This tracker exists because multiple programs may change **Hugo Core services** or **Domain APIs** at the same time.

Use it to answer:

- “Is anyone else changing this domain/API right now?”
- “Who is the SPOC (single point of contact) for this change?”
- “Do we have upstream/downstream impacts to coordinate before merge/release?”

> The PDF contains many rows and release-specific details. This page captures the high-signal parts and suggests how to keep it up to date.

---

## Snapshot: projects listed (high level)

| # | Project | Domain/API area (as listed) | SPOC / contact (as listed) | Notes |
| --- | --- | --- | --- | --- |
| 1 | CCaaS | Payment domain | Girish Nalawadi & Sristy Paul | Inflight change to consider during Payment work |
| 2 | Retention Revolution Program | Claims domain | Sumanta Nandi | Inflight change to consider during Claims work |
| 3 | Mod Order | BOSS integration packages + Processor Function App | Manikandan Sigamani (CBO) | PDF references multiple PRs and WSDL/BOSS proxy changes |
| 4 | Core Mod | Claims domain | Mustafa Guldagon | Inflight Claims changes |
| 5 | BSB | Reference domain | Sanjay and Girish | Notes mention refactor + bsb file processing |
| 6 | Hugo Core service changes | Reference domain | Adeal A; Girish | Inflight changes impacting Reference |
| 7 | CCM CommsHub archival | Membership domain (and broader arch/design) | Pratik; Jason; Mikita; Girish | PDF references ADC packs and phased release identifiers |

---

## Payment domain: “PGR releases” section (what to do with it)

The PDF includes a long list of Payment-related endpoints, mapped to releases (R1/R2/R3), and references to:

- designs in Confluence
- PR numbers
- feature branches

Because these details change frequently, a practical way to maintain this is:

1. Keep this page as the **index**.
2. Store the detailed “endpoint-by-release” list in a separate living document (or export a table from ADO).
3. For each release, keep:
   - feature branch name(s)
   - PR link(s)
   - testing scope (what upstreams need to validate)
   - rollout plan (flags, compatibility, deployment order)

If you want, I can extract the entire endpoint list from the PDF into a structured table in this repo as a follow-up.

