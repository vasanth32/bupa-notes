# Lens (OpenLens) + AKS Mock POC (Fast Track) — with Cursor Prompts

Source reference: `BUPA/Pdf/Lens-Aks/DigitalDevOps-How to use LENS-250226-113620.pdf` and `BUPA/Confluence/Lens-Aks/How-to-use-LENS.md`.

Goal: get productive with **Lens/OpenLens day-to-day AKS ops** in the shortest time by doing a **mock POC** (local first, AKS optional), while using **Cursor** to generate commands, troubleshoot, and write learning notes.

Timebox: **2–4 hours** (local) + **30–60 mins** (optional AKS access).

---

## What you will be able to do after this POC

- Open Lens and connect to a Kubernetes cluster via kubeconfig
- Navigate **Catalog**, **Hotbar**, **Workloads**, **Pods**, **Logs**
- Add and switch **Namespaces**
- Do **port-forwarding** from Lens to access a service locally
- Diagnose common “it doesn’t show up” issues (kubeconfig, auth, namespaces, extension)
- Produce a concise set of personal notes (generated with Cursor prompts below)

---

## POC Structure (recommended order)

- **Track A (Local, no access needed)**: Learn Lens UI + Kubernetes basics using a local cluster so you can practice freely.
- **Track B (Optional AKS)**: If you have access, connect Lens to `nonprod*aks` the same way you’ll do in the real environment.

Do Track A first. Track B becomes easy once A is clear.

---

## Track A — Local mock cluster (fast practice)

### A0) Prereqs (Windows)

- Docker Desktop (with Kubernetes enabled) *or* kind/minikube
- `kubectl`
- Lens/OpenLens installed (any recent Lens is fine for local learning; keep the **AKS version constraint** for Track B)
- Cursor

> If your org expects a specific Lens/OpenLens version for AKS (the PDF says **OpenLens 6.2.5**), keep that for Track B. For local practice, any version works.

---

### A1) Create a “practice namespace” and a sample app

If you already have a sample app and YAML from `Local-Kubernetes-AKS-Mock-POC-Guide.md`, reuse it.

Otherwise, do the quickest possible sample: `nginx` + service.

#### Cursor prompt (generate local sample YAML)

Copy/paste into Cursor:

```text
Create Kubernetes YAML for a quick local practice app:
- Namespace: lens-poc
- Deployment: nginx, 2 replicas
- Service: ClusterIP on port 80
- Add labels consistently

Output: a single file named k8s-lens-poc.yaml
Also include the exact kubectl commands to apply and verify.
```

Apply and verify (example commands):

- `kubectl apply -f k8s-lens-poc.yaml`
- `kubectl get ns`
- `kubectl get pods -n lens-poc`
- `kubectl get svc -n lens-poc`

---

### A2) Add the local cluster in Lens and learn the navigation

In Lens:

- Open **Catalog** → **Clusters**
- Select your local cluster
- Pin it to **Hotbar** (attachment/pin icon)

What to practice in Lens (local cluster):

- **Namespaces**: find your namespace list, filter to `lens-poc`
- **Workloads**: locate the Deployment and Pods
- **Pods → Logs**: open logs and follow output live

#### Cursor prompt (Lens UI walkthrough notes)

```text
I’m new to Lens. Based on this checklist:
- Catalog -> Clusters
- Add to Hotbar
- Workloads
- Pods
- Logs
- Namespaces

Write a 1-page "Lens UI map" cheat-sheet for me (markdown).
Keep it action oriented: "click X then Y, what to look for, common mistakes".
```

---

### A3) Port-forward from Lens (the “daily life” skill)

In Lens (local cluster):

- Go to **Pods**
- Select a pod in `lens-poc`
- Find container/ports area and use **Forward…**
- Change random local port to something you want (e.g., `8080`)
- Start forward and open in browser: `http://localhost:8080`

#### Cursor prompt (explain port-forward like I’m new)

```text
Explain Kubernetes port-forward in simple terms.

Include:
- Why it exists (ClusterIP not reachable)
- What traffic path looks like
- How Lens port-forward compares to kubectl port-forward
- Common issues (port already in use, wrong namespace, wrong pod)
```

---

## Track B — Optional: Connect Lens to AKS (BUPA nonprod)

### B0) Access prerequisite (only for real AKS)

You typically need to be added to:

- `RES-CLD-AzureRM-AKSDevelopers`

---

### B1) Install kubelogin (required for Azure/Entra auth)

High-level steps (from the PDF):

- Download `kubelogin` (Windows zip, e.g. `kubelogin-win-amd64.zip`)
- Copy `kubelogin.exe` to a folder like `C:\Workspace\kubelogin`
- Add that folder to **PATH** (User variables if System doesn’t work)
- Restart terminal/PC if PATH changes don’t apply

#### Cursor prompt (verify kubelogin and PATH)

```text
I installed kubelogin.exe and added it to PATH on Windows.
Give me a short verification checklist + commands to confirm it works.
Also list the most common PATH mistakes and how to fix them.
```

---

### B2) Azure CLI login

From PowerShell:

- `az login`

If `az aks get-credentials` fails, one known cause (PDF) is **Zscaler enabled** (Azure CLI may not work).

#### Cursor prompt (AZ CLI troubleshooting script)

```text
Create a PowerShell troubleshooting checklist for Azure CLI + AKS credentials:
- az login
- az account show
- az account set
- az aks get-credentials

Include how to capture useful error output for a Service Desk ticket.
Also mention the Zscaler issue as a possible blocker.
```

---

### B3) Pull AKS credentials into kubeconfig

Lens reads clusters from your kubeconfig (commonly `%USERPROFILE%\.kube\config`).

From the PDF (examples):

**nonprod1aks**

- `az account set --subscription 4cef4169-6e79-4afe-a286-dbb3a671e8e8`
- `az aks get-credentials --resource-group bupaanz-mel-test-caas-vnet-rg --name nonprod1aks`

**nonprod2aks**

- `az account set --subscription 4cef4169-6e79-4afe-a286-dbb3a671e8e8`
- `az aks get-credentials --resource-group bupaanz-mel-test-caas-vnet-rg --name nonprod2aks`

**nonprod3aks**

- `az account set --subscription 4cef4169-6e79-4afe-a286-dbb3a671e8e8`
- `az aks get-credentials --resource-group bupaanz-mel-test-caas-vnet-rg --name nonprod3aks`

**nonprod4aks**

- `az account set --subscription ed8412b9-fed3-4074-b7e1-5b4427a078e1`
- `az aks get-credentials --resource-group devops-caas-test-rg01 --name nonprod4aks`

#### Cursor prompt (make it safe + avoid overwriting)

```text
I’m going to run az aks get-credentials on Windows.
Explain what it changes in ~/.kube/config, and how to avoid overwriting existing contexts.

Give me:
- Safe command examples (including --overwrite-existing guidance)
- How to list contexts and rename contexts if needed
- How to confirm which context is active
```

---

### B4) Install OpenLens (version required by the PDF)

The PDF specifies **OpenLens 6.2.5**.

After installing:

- Open Lens → **Catalog** → **Clusters**
- If empty: **+** (bottom-left) → **Sync kubeconfig file(s)** → browse to `%USERPROFILE%\.kube` → select `config` → sync
- Add your AKS cluster to **Hotbar**
- If prompted to re-auth: copy URL + code and complete Azure auth

#### Cursor prompt (Lens cluster not showing)

```text
Lens is installed but my AKS clusters don’t appear in Catalog.
Give me a step-by-step diagnosis path:
- kubeconfig location and validity
- contexts present vs missing
- re-auth flow
- Sync kubeconfig in Lens
- what "empty catalog" usually means
```

---

### B5) Add Namespaces (critical for seeing your app)

From the PDF:

- Right-click the cluster in **Hotbar**
- Go to **Namespaces** tab
- Type namespaces you can access **one-by-one**, press Enter after each
- Press **Esc** to return
- Then go to **Workloads** and pick the namespace from the dropdown

#### Cursor prompt (namespace mental model)

```text
Explain Kubernetes namespaces like I’m new.
Also explain why Lens may look "empty" until I add/select the correct namespaces.
Give me a checklist to find my app if I only know the app name.
```

---

### B6) Logs + Port Forwarding in AKS (the two daily ops)

Logs (PDF flow):

- Cluster → **Pods** → select Pod → **Logs**

Port-forward (PDF flow):

- **Pods** → scroll to container section → select container → **Forward…**
- Change random port to the one you want → **Start**

---

### B7) If “pod logs” menu is missing (OpenLens extension)

PDF note: in OpenLens **6.3+**, some pod menus can differ.

Fix:

- Install/enable extension: `@alebcay/openlens-node-pod-menu`

#### Cursor prompt (extension install steps)

```text
I can’t see the pod logs menu in OpenLens (possibly 6.3+).
Explain why it happens and give me exact steps to install/enable:
@alebcay/openlens-node-pod-menu
Also provide a fallback path to view logs without that extension.
```

---

## Cursor “Learning Accelerator” prompts (use throughout)

### 1) Turn any error into a fix plan

```text
Here is the exact error output (paste below).
Give me:
- likely causes (ranked)
- fastest verification steps
- the smallest safe fix
- how to confirm it’s resolved
```

### 2) Generate my personal notes (from what I did today)

After you finish Track A and/or B, paste this into Cursor:

```text
Create my personal learning notes in markdown from today’s Lens + Kubernetes practice.

Must include:
- My goal in one sentence
- Lens UI map (Catalog, Hotbar, Workloads, Pods, Logs, Forward)
- kubeconfig + contexts (what they are, where they are on Windows)
- namespaces (why they matter)
- port-forwarding (why + how)
- a troubleshooting section: "Lens empty", "auth prompts", "no logs menu"
- a 10-item checklist I can reuse next time

Tone: simple, practical, short sentences.
```

### 3) Explain “what I’m looking at” in Lens

```text
In Lens I’m looking at: <paste screenshot text if any, or describe what you see>.
Explain what each panel/section means and what I should click next to debug an issue.
```

---

## POC Completion Checklist (copy/paste into your notes)

- [ ] I can see my cluster in Lens Catalog
- [ ] I can pin cluster to Hotbar
- [ ] I understand kubeconfig and contexts location on Windows
- [ ] I can add/select namespaces and explain why it matters
- [ ] I can find a Deployment and its Pods in Workloads
- [ ] I can open and follow Pod logs
- [ ] I can port-forward from Lens and access an app via localhost
- [ ] I know what to do if clusters don’t show (sync kubeconfig)
- [ ] I know what to do if logs menu is missing (extension / fallback)
- [ ] I generated a 1–2 page markdown note from today using Cursor

