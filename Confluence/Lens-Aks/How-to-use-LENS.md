## How to use LENS (OpenLens) for AKS – BUPA Digital DevOps

Source: `BUPA/Pdf/Lens-Aks/DigitalDevOps-How to use LENS-250226-113620.pdf`

### What “Lens” is (in simple words)

Lens (OpenLens) is a desktop UI for Kubernetes. It helps you **see and operate** Kubernetes resources (namespaces, pods, deployments, logs, etc.) without needing to do everything via `kubectl`.

---

## 1) Access prerequisites

### 1.1 Add to security group (developers)

Raise a Service Desk request (or ask DevOps) to add you to:

- `RES-CLD-AzureRM-AKSDevelopers`

> Note from PDF: this is for **Developers** (not DevOps team members).

---

## 2) Install `kubelogin` (required for Azure/Entra auth)

1. Download `kubelogin` from GitHub releases:
   - `Releases · Azure/kubelogin (github.com)` (PDF reference)
   - Example artifact: `kubelogin-win-amd64.zip`
2. Extract and copy `kubelogin.exe` to a workspace folder, e.g.:
   - `C:\Workspace\kubelogin`
3. Add that folder to your **PATH** environment variable.
   - If it doesn’t work, add it under **User variables** (not System variables).
4. Restart your PC if needed (so PATH changes take effect).

---

## 3) Install Azure CLI + login

1. Install Azure CLI (if not already installed):
   - `https://aka.ms/installazurecliwindows` (PDF link)
2. Open PowerShell and run:

```powershell
az login
```

This opens a browser window for authentication.

---

## 4) Create/update kubeconfig for AKS clusters

Lens reads your Kubernetes cluster list from kubeconfig. The PDF’s approach is:

- Use Azure CLI to pull cluster credentials into:
  - `%USERPROFILE%\.kube\config`

### 4.1 nonprod1aks

```powershell
az account set --subscription 4cef4169-6e79-4afe-a286-dbb3a671e8e8
az aks get-credentials --resource-group bupaanz-mel-test-caas-vnet-rg --name nonprod1aks
```

### 4.2 nonprod2aks

```powershell
az account set --subscription 4cef4169-6e79-4afe-a286-dbb3a671e8e8
az aks get-credentials --resource-group bupaanz-mel-test-caas-vnet-rg --name nonprod2aks
```

### 4.3 nonprod3aks

```powershell
az account set --subscription 4cef4169-6e79-4afe-a286-dbb3a671e8e8
az aks get-credentials --resource-group bupaanz-mel-test-caas-vnet-rg --name nonprod3aks
```

### 4.4 nonprod4aks

```powershell
az account set --subscription ed8412b9-fed3-4074-b7e1-5b4427a078e1
az aks get-credentials --resource-group devops-caas-test-rg01 --name nonprod4aks
```

### 4.5 Common issue: Zscaler blocks Azure CLI

PDF note:

- If `az aks ...` fails, check **Zscaler**.
- If Zscaler is enabled, Azure CLI may not work.
- Raise a Service Desk request with the error and ask for Zscaler to be disabled.

---

## 5) Install Lens (OpenLens) and sync kubeconfig

### 5.1 Install the required Lens version

The PDF specifically says to install **Lens Desktop version 6.2.5**:

- “Release v6.2.5 · MuhammedKalkan/OpenLens” (PDF reference)

Steps:

1. Download Lens Desktop (must be the mentioned version in the PDF).
2. Copy the installer to your workspace (example in PDF):
   - `Lens Setup 6.2.5-latest.exe` (or similar)
3. Install Lens/OpenLens.

### 5.2 Confirm clusters appear (Catalog)

1. Open Lens.
2. Click **Catalog**.
3. Select **Clusters** and check whether your clusters appear.

If the list is empty:

1. Hover the **+ (Plus)** button (bottom-left in the app)
2. Choose **Sync kubeconfig file(s)**
3. In the file browser, go to:
   - `%USERPROFILE%\.kube`
4. Select `config` and sync.

### 5.3 Add cluster to “Hotbar”

PDF step:

- Add your cluster to the **Hotbar** by clicking the “attachment” icon.

### 5.4 Azure re-auth prompt

If Azure asks you to authenticate again:

- Copy the URL and the code from the prompt to complete the login.

---

## 6) Configure namespaces (so you see your app)

You need to add the namespaces your applications are deployed into.

1. Right-click the cluster in the Hotbar.
2. Go to the **Namespaces** tab.
3. Type namespaces you can access **one-by-one** and press Enter after each.
4. When you finish adding namespaces, press **Esc** to go back.

Then:

- Select the cluster → **Workloads**
- Choose the namespace from the dropdown (top-left)

---

## 7) Day-to-day usage (logs + port forwarding)

### 7.1 View pod logs

1. In the cluster menu go to **Pods**
2. Select a pod
3. Click **Logs**

### 7.2 Port-forward to access an app locally

The PDF suggests:

1. Go to **Pods**
2. Scroll to the containers section
3. Select a container and choose **Forward…**
4. A window opens with a random local port.
5. Change it to a port you prefer (especially if using Postman etc.)
6. Click **Start**

---

## 8) If “pod logs” menu is missing (OpenLens extension)

PDF note: in OpenLens **6.3+**, the pod menu may not show logs the same way.

Fix:

- Install/enable extension: `@alebcay/openlens-node-pod-menu`

---

## Quick checklist (most common successful path)

- Added to `RES-CLD-AzureRM-AKSDevelopers`
- `kubelogin.exe` installed and on PATH
- `az login` works
- Ran `az aks get-credentials ...` for your target nonprod cluster
- Lens installed (6.2.5 as per PDF) and kubeconfig synced from `%USERPROFILE%\.kube\config`
- Namespaces added in Lens
- You can open Pods → Logs, and use Forward… when needed

