## Environments & key links

Source: Confluence PDF “Environment List” exported on 2025-02-26.

### Purpose of this page

- Keep a **single place** to find environment names (D12/D24/D33/D34/etc.)
- Provide quick links to dashboards and key Azure DevOps repos/pipelines referenced by the project

---

## Hugo Cap Mod environments (as captured in PDF)

| Environment | Code/name | Notes |
| --- | --- | --- |
| DEV | D24 | Has environment dashboard link in PDF |
| DEV | T19 | Listed as “Dashboard Link (New)” in PDF |
| DEV | D12 | Has environment dashboard link in PDF |
| TEST | D34 | Has environment dashboard link in PDF |
| UAT | D33 | Has environment dashboard link in PDF |
| PERF | PERF4 | Has environment dashboard link in PDF |

### Environment dashboard links (from PDF)

The PDF references this dashboard base URL with different `Name=` query values:

- `https://environmentdashboard.internal.bupa.com.au/?Name=D24`
- `https://environmentdashboard.internal.bupa.com.au/?Name=D12`
- `https://environmentdashboard.internal.bupa.com.au/?Name=D33`
- `https://environmentdashboard.internal.bupa.com.au/?Name=D34`
- `https://environmentdashboard.internal.bupa.com.au/?Name=PERF4`

> Note: these are internal links; you’ll need network access/VPN as per BUPA standards.

### Azure resource groups (RG) referenced (from PDF text)

The following RG names appear in the export (verify current naming in Azure Portal):

- `HICBOCoreServices-d24-rg01`
- `HICBOCoreServices-dev-rg01`
- `HICBOCoreServices-test-rg01`
- `HICBOCoreServices-d34-rg01`

Redis cache is mentioned for:

- DEV: D24
- TEST: D34, PERF4

APIM is referenced for DEV/TEST (details not fully captured in the PDF text extraction).

---

## BUPA dependencies (called out in the PDF)

### Apollo CRM

- DEV (D24): `https://bupaanz-hugo-cbo.crm6.dynamics.com/`
- TEST (D34): `https://bupaanz-hugo-cbo-sit.crm6.dynamics.com/`

---

## Azure DevOps repos (as captured in the PDF)

These repo links were listed (you may need to sign in to Azure DevOps):

- Reference API: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.Reference`
- Interaction API: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.Interaction`
- Customer API: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.Customer`
- Membership API: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.Membership`
- Payment API: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.Payment`
- Sales API: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.Sales`
- Claim API: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.Claim`
- Apollo data stream: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.ApolloDataStream`
- Hugo → Apollo sync: `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_git/HI.API.HugoToApolloSync`

---

## Azure DevOps pipelines (as captured in the PDF)

The PDF lists pipeline definition links like:

- Hugo Mod pipeline (example): `https://bupaaunz.visualstudio.com/HI-CBO-Apps/_build?definitionId=9000`

Other definition IDs captured in the PDF output include:

- `8999`
- `8962`
- `8977`
- `9328`
- `9311`
- `9025`
- `9352`
- `9353`

Because the PDF extraction loses context around which ID maps to which repo, treat these as a **starting point**:

1. Open Azure DevOps pipelines.
2. Search by `definitionId`.
3. Update this page once you confirm the mapping (repo → pipeline).

