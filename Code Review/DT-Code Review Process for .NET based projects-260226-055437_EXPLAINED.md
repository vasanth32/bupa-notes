# Code Review Process for .NET projects — Explained (Dev-Feature branches)

Source: `DT-Code Review Process for .NET based projects-260226-055437.pdf`

This document describes the **required PR process** for **dev-feature branches** merging into a corresponding **feature branch**, including **mandatory quality gates** (SonarQube/Checkmarx), **reviewer requirements**, and **exceptions**.

---

## What this process applies to

- **Primary scope**: PRs from **dev-feature branches → feature branch**
- **Tooling referenced**: Azure DevOps (ADO), SonarQube, Checkmarx

---

## 1) Pull Request (PR) requirement

- **Rule**: Every dev-feature branch must go through a **PR into the corresponding feature branch**.
- **Review focus**: **Functional correctness** and **completeness** against **User Story Acceptance Criteria**.

**Practical self-check before PR**
- Re-read Acceptance Criteria and confirm:
  - You implemented *all* scenarios (happy path + negative/edge).
  - You didn’t introduce behavior changes outside the story scope.

---

## 2) User Story linking (mandatory)

- **Rule**: Every dev-feature PR must be linked to **at least one User Story** in ADO.

**Practical self-check before PR**
- Ensure the PR shows:
  - Linked work item(s)
  - Correct story/feature identifiers

---

## 3) Approver group, minimum reviewers, and timing

- **Approver group**: ADO group `HI-Feature-Branch-Reviewers`
  - Members listed in the PDF: Vignesh, Mani, Franklin, Girish, Ashwin, Yatharth
- **Minimum reviewers**: **2 reviewers** required for each PR.
- **Review lead**: Application Support Lead (Franklin) oversees reviewer assignment via the **Code Review Forum**.
- **Time expectation**: Provide **at least 2 working days** for review (quality + thoughtful feedback).

**Practical self-check before PR**
- Add at least **two reviewers** from the correct group.
- Don’t raise “urgent” PRs that bypass the 2-day expectation unless there’s an approved exception.

---

## 4) Static code analysis requirements (mandatory)

- **Baseline scan**: Run immediately **after branch creation** to initialize scan history.
- **Pre-PR scan**: Run again **before raising the PR** so scans cover all changed content.
- **Reviewer duty**: Reviewers must verify scan presence and results during PR review.

**Practical self-check before PR**
- Confirm your PR has evidence of:
  - A **baseline scan** on the branch (early)
  - A **latest scan** right before PR creation
- Ensure scan results correspond to the latest commit(s).

---

## 5) SonarQube quality gate criteria (mandatory unless exempted)

Thresholds that must be met (unless formally exempted):
- **Security issues**: 0
- **Reliability issues**: 0
- **Maintainability issues**: 0
- **Security hotspots**: 0
- **Code coverage**: **≥ 90%**
- **Code duplication**: **< 2%**

**Practical self-check before PR**
- Fix issues rather than “acknowledging” them:
  - Coverage: add tests for branches, edge cases, and exception paths.
  - Duplication: refactor repeated logic into a single implementation (but avoid generic “Helpers” if your checklist forbids them).

---

## 6) Checkmarx security scan criteria (mandatory unless exempted)

All must be **0**:
- **SAST vulnerabilities**: 0
- **SCA vulnerabilities**: 0
- **API vulnerabilities**: 0
- **IaC vulnerabilities**: 0
- **Container vulnerabilities**: 0

**Practical self-check before PR**
- If anything is non-zero:
  - Remediate and re-scan, or
  - Follow the exemption process (below) *before* asking for approval.

---

## 7) Exemptions (formal approval required)

- **Rule**: Any deviation (failing quality gates / unresolved findings) must be formally approved by:
  - **Technology Manager**, or
  - **Head of Technology**

**Practical self-check**
- Don’t assume “it’s fine for now”.
- Capture the exemption approval in a traceable way (per your team practice in ADO/PR comments).

---

## Exceptions — PRs into non-feature branches (develop / hotfix / release)

The PDF lists special handling when merging into **non-feature** branches.

### Scenario 1: feature → develop

- **Functional review**: Full functional review is **not required again**.
- **Reviewer must validate** the **originating feature branch** was properly reviewed and approved.
- **Static analysis**: SonarQube and Checkmarx reports for the **current branch** must still be present and validated.

### Scenario 2: bugfix → develop (via PR)

- Follow the **dev-feature branch process** (i.e., the full process above).

### Scenario 3: bugfix → release or hotfix (via PR)

- May receive a **lighter functional review**, depending on urgency.
- SonarQube/Checkmarx may be **exempted depending on urgency**.
- Exemption must be approved by the **Technology Manager** or **Head of Technology**.

---

## How reviewers verify the originating feature branch was reviewed properly

When a PR is merging into develop (Scenario 1), reviewers must:

1) **Locate the original PR**
- Find the PR that merged the corresponding **dev-feature branch → feature branch** in ADO history.

2) **Confirm review details**
- The original PR:
  - Had **≥ 2 approvals** from `HI-Feature-Branch-Reviewers`
  - Was linked to **one or more User Stories**
  - Had **no unresolved comments**, or had explicit approved exceptions

3) **Validate static analysis scans**
- SonarQube + Checkmarx scans are present and acceptable in the original PR.

4) **Optionally review commit history**
- Confirm no major code was added to the feature branch **after** the PR without additional review.

---

## The “don’t-get-rejected” PR checklist (quick)

- **Work item**: PR linked to at least one User Story in ADO
- **Reviewers**: ≥ 2 from `HI-Feature-Branch-Reviewers`
- **Timing**: allow 2 working days for review
- **Scans**:
  - Baseline scan after branch creation
  - Final scan before PR
- **SonarQube**: 0 security/reliability/maintainability/hotspots, coverage ≥ 90%, duplication < 2%
- **Checkmarx**: all categories 0
- **Exemptions**: only with formal approval (Tech Manager / Head of Technology)

