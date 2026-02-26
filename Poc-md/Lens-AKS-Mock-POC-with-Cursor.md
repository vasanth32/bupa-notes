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

You said you **don’t have these installed yet**, so this section is the full setup.

#### A0.1 Install .NET SDK

- Install **.NET 8 SDK** (recommended).
  - Fast path (Windows Package Manager):
    - `winget install -e --id Microsoft.DotNet.SDK.8`
  - Verify:
    - `dotnet --version`

#### A0.2 Install Docker Desktop

- Install Docker Desktop.
  - Fast path:
    - `winget install -e --id Docker.DockerDesktop`
- Start Docker Desktop once installed.
- Verify:
  - `docker version`

> If Docker Desktop complains about virtualization/WSL2, you’ll need WSL enabled. Common fix is running `wsl --install` (admin) and rebooting.

#### A0.3 Enable Local Kubernetes (Docker Desktop)

This is the recommended “local AKS-like” setup on Windows.

- Open **Docker Desktop** → **Settings** → **Kubernetes**
- Turn on **Enable Kubernetes**
- Click **Apply & Restart**
- Wait until Kubernetes is ready (Docker Desktop shows it running)

#### A0.4 Install `kubectl`

- Fast path:
  - `winget install -e --id Kubernetes.kubectl`
- Verify:
  - `kubectl version --client`

When Docker Desktop Kubernetes is enabled, also verify cluster connectivity:

- `kubectl config get-contexts`
- `kubectl get nodes`

If `kubectl get nodes` works, your local cluster is ready for Track A.

#### A0.5 Install Lens / OpenLens

You have two scenarios:

- **Local learning (Track A)**: any recent Lens/OpenLens version is fine.
- **AKS in BUPA (Track B)**: your PDF specifies **OpenLens 6.2.5**.

Install approach:

- If you can use winget, try searching first:
  - `winget search lens`
  - `winget search openlens`
- If not available (common), install from the vendor/GitHub release:
  - OpenLens 6.2.5 reference (PDF): `https://github.com/MuhammedKalkan/OpenLens/releases/tag/v6.2.5`

After install:

- Open Lens/OpenLens → confirm it launches.

#### A0.6 Install Cursor (if needed)

- If Cursor isn’t installed yet, install it via your standard software portal or from `https://www.cursor.com/`.

---

### A0) Quick verification checklist (copy/paste)

- `dotnet --version`
- `docker version`
- `kubectl version --client`
- `kubectl get nodes`

> If your org expects a specific Lens/OpenLens version for AKS (the PDF says **OpenLens 6.2.5**), keep that for Track B. For local practice, any version works.

---

### A1) Create a .NET microservice (from scratch)

We’ll build a tiny microservice with:

- `GET /customers/{id}` (dummy JSON)
- `GET /health/live` and `GET /health/ready` (for probes)
- Console logging (so Lens logs are useful)
- Config via environment variable (to practice ConfigMap)

Create a new folder (example): `BUPA/poc/lens-dotnet-poc/`

#### Cursor prompt (generate .NET microservice)

Copy/paste into Cursor:

```text
Create a minimal ASP.NET Core (.NET 8) Web API microservice named Demo.CustomerApi.

Requirements:
- Endpoint: GET /customers/{id} returns JSON { id, name, email, message }
- Read `DEMO_MESSAGE` from environment variable and return it as `message`
- Add structured logging (ILogger) including a log line per request
- Add health endpoints:
  - GET /health/live returns 200 OK with { status: "live" }
  - GET /health/ready returns 200 OK with { status: "ready" }
- Listen on port 8080 (so container + k8s are consistent)
- Keep it simple and runnable locally

Also:
- Provide the exact dotnet commands to create/build/run it
- Keep folder structure clean
```

Run locally:

- `dotnet run`
- Test: `http://localhost:8080/customers/1`

#### Cursor prompt (explain the microservice)

```text
Explain this microservice like I’m new to ASP.NET Core.
Explain request flow, routing, dependency injection (if any), and logging.
Write the explanation as markdown notes for my POC.
```

---

### A2) Dockerize the microservice

#### Cursor prompt (Dockerfile)

```text
Create a production-ready Dockerfile for this ASP.NET Core microservice.

Requirements:
- Multi-stage build
- Final image runs as non-root if possible
- Expose port 8080
- Works with ASP.NET Core .NET 8
- Keep image small

Also provide:
- docker build command
- docker run command mapping 8080:8080
- a quick curl test to validate the container
```

Build + run:

- `docker build -t demo-customer-api:local .`
- `docker run --rm -p 8080:8080 -e DEMO_MESSAGE="hello-from-docker" demo-customer-api:local`
- Test: `http://localhost:8080/customers/1`

#### Cursor prompt (container concept → AKS)

```text
Explain image vs container, and why AKS needs images.
Relate this to "pods run containers" and why we standardize ports like 8080.
Keep it short and practical.
```

---

### A3) Deploy to local Kubernetes (AKS-like)

We will deploy:

- Namespace: `lens-poc`
- Deployment: 3 replicas
- Service: ClusterIP (we’ll use port-forward from Lens)
- Probes: liveness + readiness hitting `/health/live` and `/health/ready`
- ConfigMap: sets `DEMO_MESSAGE`

#### Important: make the image available to your local cluster

Pick the path that matches your setup:

- **Docker Desktop Kubernetes**: usually you can use the local image directly after `docker build`.
- **kind**: you must load the image, e.g. `kind load docker-image demo-customer-api:local`

#### Cursor prompt (Kubernetes YAML from scratch)

```text
Create Kubernetes manifests for my microservice.

Constraints:
- Namespace: lens-poc
- Deployment name: demo-customer-api
- Replicas: 3
- Container image: demo-customer-api:local
- Container port: 8080
- Add livenessProbe: GET /health/live on 8080
- Add readinessProbe: GET /health/ready on 8080
- Service: ClusterIP exposing port 8080 -> targetPort 8080
- ConfigMap: demo-customer-api-config with DEMO_MESSAGE="hello-from-k8s"
- Inject DEMO_MESSAGE env var from ConfigMap
- Add standard labels: app=demo-customer-api

Output:
- k8s/namespace.yaml
- k8s/configmap.yaml
- k8s/deployment.yaml
- k8s/service.yaml

Also provide the exact kubectl apply commands and verification commands.
```

Apply + verify (examples):

- `kubectl apply -f k8s/namespace.yaml`
- `kubectl apply -f k8s/configmap.yaml`
- `kubectl apply -f k8s/deployment.yaml`
- `kubectl apply -f k8s/service.yaml`
- `kubectl get all -n lens-poc`

---

### A4) Use Lens with your local cluster (the main learning)

In Lens:

- Open **Catalog** → **Clusters**
- Select your local cluster
- Pin it to **Hotbar** (attachment/pin icon)

Then:

- Go to **Workloads** → filter/select namespace `lens-poc`
- Go to **Pods** → pick a pod → open **Logs**
- Confirm you see your request logs when you hit `GET /customers/1`

#### Port-forward from Lens (practice)

- In Lens: **Pods** → select pod → container/ports → **Forward…**
- Set local port (e.g. `8081`) → Start
- Open `http://localhost:8081/customers/1`

#### Cursor prompt (Lens UI walkthrough notes)

```text
I deployed a microservice to Kubernetes and I’m using Lens.
Write a 1-page cheat-sheet (markdown) for:
- finding the deployment/pods in a namespace
- viewing logs and spotting errors
- port-forwarding from Lens to my service
- common mistakes that make Lens look "empty"
```

---

### A5) Scale + crash simulation (so logs/restarts make sense)

Scale:

- `kubectl scale deployment demo-customer-api -n lens-poc --replicas=5`

Crash simulation idea (simple):

- Update code so `/customers/99` throws an exception (or exits), rebuild image, redeploy.
- Observe pod restarts and read logs in Lens.

#### Cursor prompt (crash and observe)

```text
Help me simulate a failure in this API so I can observe restarts in Kubernetes and Lens.

Give me:
- a safe way to trigger an exception on /customers/99
- how to rebuild the docker image and roll the deployment
- what I should look for in:
  - kubectl get pods
  - kubectl describe pod
  - Lens pod details + logs
```

---

### A6) (Optional) Add per-request correlation in logs

#### Cursor prompt (logging upgrade)

```text
Upgrade the API logging so each request includes a correlation id.
If no header is provided, generate one.
Return it in response headers too.
Explain how this helps when reading logs in Lens across multiple replicas.
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

