## Local Kubernetes (AKS-like) Mock POC – Fast Learning Guide

This is a **super fast mock POC** to quickly understand the day-to-day AKS/Kubernetes concepts used in the Hugo modernisation program.

You will learn by building and operating a tiny service end-to-end:

- Docker (image/container)
- Kubernetes (deployments/pods/services)
- “AKS-like” workflow (locally)
- Logs
- Scaling
- Port forwarding
- Config injection
- Crash recovery (CrashLoopBackOff-style behavior)

**Goal**: finish basic understanding in **2–3 hours**.

---

## 🚀 Overview of the POC

We will build:

```plaintext
Simple Customer API (.NET 8)
    ↓
Dockerize it
    ↓
Run in Local Kubernetes (kind or Docker Desktop K8s)
    ↓
Deploy 3 replicas
    ↓
View logs
    ↓
Scale it
    ↓
Simulate crash
```

This mirrors your real project shape (AKS + domain API), but simplified.

---

## 🧠 Step 0 – What you need installed

Make sure you have:

- .NET 8 SDK
- Docker Desktop
- Kubernetes enabled in Docker Desktop
- `kubectl`
- Cursor

That’s enough.

---

## 🟢 Step 1 – Create basic API

### 🧾 Cursor Prompt 1 – Generate API

Paste this in Cursor:

```
Create a minimal .NET 8 Web API project called Demo.CustomerApi.

Requirements:
- One controller: CustomerController
- GET /customers/{id}
- Return dummy JSON with Id, Name, Email
- Add ILogger usage
- Add try-catch and proper logging
- Use clean architecture style folder structure
- Keep it simple and runnable
```

Run:

```bash
dotnet run
```

Test:

```text
http://localhost:5000/customers/1
```

### 🧾 Cursor Prompt 2 – Ask Cursor to explain code

```
Explain this project to me step by step.
Explain like I am new to ASP.NET Core.
Explain how request flows from controller to response.
Add notes in markdown format.
```

---

## 🐳 Step 2 – Dockerize it

### 🧾 Cursor Prompt 3 – Generate Dockerfile

```
Create a production-ready Dockerfile for this .NET 8 Web API.

Requirements:
- Use multi-stage build
- Use mcr.microsoft.com/dotnet/sdk:8.0 for build
- Use aspnet:8.0 for runtime
- Expose port 8080
- Optimize image size
- Add comments explaining each step
```

Build image:

```bash
docker build -t demo-customer-api .
```

Run:

```bash
docker run -p 8080:8080 demo-customer-api
```

Test:

```text
http://localhost:8080/customers/1
```

### 🧾 Cursor Prompt 4 – Explain container concept

```
Explain what just happened when I built and ran this Docker image.

Explain:
- What is image
- What is container
- Why container is isolated
- How this relates to AKS in my real project
```

---

## ☸ Step 3 – Deploy to Kubernetes locally

Now we simulate AKS locally.

### 🧾 Cursor Prompt 5 – Generate Kubernetes YAML

```
Create Kubernetes YAML files for this API.

Requirements:
- Deployment with 3 replicas
- Container image demo-customer-api
- Port 8080
- Service of type ClusterIP
- Add readiness and liveness probe
- Add comments explaining each section
```

Save as:

```text
k8s-deployment.yaml
```

Apply it:

```bash
kubectl apply -f k8s-deployment.yaml
```

Check:

```bash
kubectl get pods
```

You should see **3 pods**.

### 🧾 Cursor Prompt 6 – Explain Kubernetes concepts

```
Explain:

- What is Deployment
- What is Pod
- What is ReplicaSet
- What is Service
- What is readiness and liveness probe

Relate everything to my Hugo project in simple language.
```

---

## 📈 Step 4 – Scale it

Run:

```bash
kubectl scale deployment demo-customer-api --replicas=5
```

Check:

```bash
kubectl get pods
```

### 🧾 Cursor Prompt 7 – Explain scaling

```
Explain how scaling works in Kubernetes.

Explain:
- What happens internally when I scale to 5
- How load balancing works
- How this is similar to HPA in AKS
```

---

## 🔥 Step 5 – Simulate crash

Modify controller (example):

```csharp
if(id == 99)
    throw new Exception("Crash simulation");
```

Rebuild + redeploy.

Call:

```text
/customers/99
```

Check:

```bash
kubectl get pods
kubectl logs <pod-name>
```

You should observe the pod restarting automatically.

### 🧾 Cursor Prompt 8 – Explain crash recovery

```
Explain what happened when the API crashed.

Explain:
- How Kubernetes detected failure
- What liveness probe does
- How this relates to CrashLoopBackOff in AKS
```

---

## 🔎 Step 6 – View logs

Run:

```bash
kubectl logs <pod-name>
```

Or use Lens/OpenLens if you have it.

### 🧾 Cursor Prompt 9 – Explain logging in cloud

```
Explain how logs work in Kubernetes.

Explain:
- Where console logs go
- How OpenTelemetry would capture them
- How Application Insights fits here
- How this is similar to my Bupa project logging design
```

---

## 🧪 Step 7 – Add ConfigMap

### 🧾 Cursor Prompt 10 – Add ConfigMap example

```
Update Kubernetes YAML to:

- Add a ConfigMap with key DemoMessage
- Inject it as environment variable
- Modify controller to read from environment variable
- Return that message in response

Explain how this is similar to Azure App Configuration.
```

---

## 🎯 Optional – Add Redis

If you want advanced practice:

```
Add Redis container to Kubernetes.
Modify API to use StackExchange.Redis.
Cache response.
Explain how this relates to HI.Library.Cache.
```

---

## 📝 Bonus – Ask Cursor to create learning notes

### 🧾 Cursor Prompt 11 – Generate personal notes

```
Generate structured learning notes for me based on this POC.

Sections:
- Docker basics
- Kubernetes basics
- Deployment flow
- Pod lifecycle
- Logging flow
- Scaling
- Crash recovery
- How this maps to my Hugo Modernisation project

Keep it simple and concise.
```

---

## ✅ Expected outcome

After this POC, you should understand:

| Concept | You will understand |
| --- | --- |
| Docker | Image vs container |
| Kubernetes | Pod/deployment lifecycle |
| AKS | Managed Kubernetes mapping |
| Replica | Scaling |
| Logs | Debugging |
| Crash | Auto-recovery |
| Config | Env injection |
| Service | Networking basics |

### ⚡ Fast execution plan (2 hours)

- API creation → ~20 mins
- Docker → ~20 mins
- K8s deploy → ~30 mins
- Scaling + crash → ~20 mins
- Notes generation → ~15 mins

