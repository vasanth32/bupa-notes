# Hugo.Batch - Quick Reference Guide

## ?? TL;DR (Too Long; Didn't Read)

### What is Hugo.Batch?
**A scheduled batch processing system** that runs jobs automatically (like a factory assembly line).

### NOT What?
- ? Not WCF (no web services)
- ? Not API (no HTTP endpoints)
- ? Not Web App (no UI)
- ? Not real-time (runs on schedule)

### What IS It?
? **Windows Service that runs jobs on a schedule**

---

## ??? Technology Stack (Quick View)

| Category | Technology | Version |
|----------|-----------|---------|
| **Language** | C# | 7.3 |
| **Framework** | .NET Framework | 4.8 |
| **Database** | Oracle | 11g / 12c |
| **Service Host** | Windows Service | 2016+ |
| **IDE** | Visual Studio | 2019/2022 |
| **Testing** | NUnit / MSTest | 3.x / Latest |
| **Logging** | log4net | 2.x |
| **Plugin System** | MEF (System.AddIn) | .NET 4.8 |

---

## ?? Project Structure (One Look)

```
Hugo.Batch/
??? HugoBatchJobs/        ? Individual batch jobs
?   ??? Tax/
?       ??? TaxGenPst (PostGenerationJob.cs) ? YOUR TASK
?
??? HugoBatchProcessor/   ? Framework & utilities
?   ??? Bupa.Hugo.Batch.Common/
?
??? Hugo_Services/        ? Windows Service host
    ??? BatchServiceHost/
```

---

## ?? How It Works (Simple)

```
1. Windows Service Starts (24/7)
   ?
2. Scheduler Checks Time
   ?
3. Is Job Due? 
   ?? NO: Wait
   ?? YES: Continue
   ?
4. Load Job (TaxGenPst as plugin)
   ?
5. Execute Job.Work()
   ?? Read Arguments
   ?? Validate Mode
   ?? Call Repository
   ?? Execute Oracle Stored Procedure
   ?
6. Log Results
   ?
7. Exit Gracefully
```

---

## ?? Your Task: TaxGenPst Job

### File Location
```
Server\HugoBatchJobs\Tax\Bupa.Hugo.Batch.Job.Tax.PostGeneration\
??? PostGenerationJob.cs
```

### What It Does
- Runs **after** tax statements are generated
- **Does NOT generate** statements
- Finalizes annual runs OR executes adjustments

### Supported Modes
- Annual (finalize)
- AdjustmentForMailHouse (adjust)
- AdjustmentForATO (adjust)
- SystemFix (fix)
- ManualReissue ? (NOT ALLOWED)

### The 3 Bugs to Fix
```
Bug 1 (Line 30):  ContainsKeys ? ContainsKey
Bug 2 (Line 59):  ContainsKeys ? ContainsKey
Bug 3 (Line 71):  adjustementRequestRepository ? adjustmentRequestRepository
```

---

## ?? Batch vs Real-Time (Quick Comparison)

### Batch Processing (Hugo.Batch) ?
- Scheduled
- Large datasets
- Offline
- No UI
- Cost-effective
- Example: Tax generation

### Real-Time Processing (API/WCF) ?
- User-triggered
- Small transactions
- Online
- User interfaces
- Higher cost
- Example: Login/checkout

---

## ?? Essential Tools

| Tool | Purpose |
|------|---------|
| **Visual Studio 2019/2022** | Development IDE |
| **Git** | Version control |
| **SQL Developer** | Oracle queries |
| **Windows Event Viewer** | Job logs |
| **Notepad++** | Quick file edits |
| **Test Explorer** | Running tests |

---

## ?? Key Files to Know

| File | Purpose |
|------|---------|
| `PostGenerationJob.cs` | TaxGenPst job (YOUR FOCUS) |
| `JobBase.cs` | Parent class for all jobs |
| `MembershipDataRepository.cs` | Database access for annual |
| `AdjustmentRequestRepository.cs` | Database access for adjustments |
| `App.config` | Configuration |
| `BatchServiceHost.cs` | Windows Service host |

---

## ?? Getting Started Checklist

- [ ] Clone repository (`git clone ...`)
- [ ] Open solution in Visual Studio
- [ ] Build solution (Ctrl+Shift+B)
- [ ] Navigate to PostGenerationJob.cs
- [ ] Fix 3 bugs
- [ ] Build to verify
- [ ] Commit to Git
- [ ] Submit for review

---

## ? Common Questions

**Q: Is this a web service?**
A: No, it's a batch processing system (Windows Service).

**Q: Does it have a UI?**
A: No, it runs headless in the background.

**Q: Is it accessed via HTTP?**
A: No, it's scheduled jobs, not HTTP endpoints.

**Q: Does it use WCF?**
A: No, no SOAP or web services.

**Q: Is it an API?**
A: No, no REST endpoints.

**Q: How do jobs run?**
A: Automatically on a schedule (like cron jobs).

**Q: Can I trigger a job manually?**
A: Maybe (depends on implementation), but designed for scheduled runs.

**Q: What language is used?**
A: C# with .NET Framework 4.8.

**Q: Where is data stored?**
A: Oracle Database (11g or 12c).

**Q: How are new jobs added?**
A: As plugins using MEF (System.AddIn).

---

## ?? Reading Order (Recommended)

1. **This Quick Reference** (5 min) ? You are here
2. **ProjectOverview.md - Architecture Type section** (10 min)
3. **ProjectOverview.md - Batch vs Real-Time** (5 min)
4. **NewTask.md** (15 min)
5. **PostGenerationJob.cs code** (20 min)
6. **Other Tax Jobs** (30 min)

**Total: ~1 hour** to understand the basics

---

## ?? Vocabulary (For Beginners)

| Term | Meaning |
|------|---------|
| **Batch Job** | A program that runs automatically on a schedule |
| **Scheduled** | Set to run at specific times (like 2 AM daily) |
| **Plugin/AddIn** | A piece of code loaded dynamically at runtime |
| **MEF** | Framework for loading plugins |
| **Repository** | Class that handles database operations |
| **Stored Procedure** | SQL code stored in Oracle database |
| **Oracle** | Enterprise database (used here) |
| **Windows Service** | Background program that runs on Windows |
| **ETL** | Extract ? Transform ? Load (data pipeline) |
| **Headless** | No user interface, runs in background |

---

## ?? Database Operations

### Oracle Stored Procedures Used

1. **TAX_GEN_PKG.FinaliseGeneration**
   - Called for annual mode
   - Finalizes tax generation
   - Called by: MembershipDataRepository

2. **TAX_ADJ.PostGeneration**
   - Called for adjustment modes
   - Executes adjustment logic
   - Called by: AdjustmentRequestRepository

---

## ?? Where to Find Help

| Question | Where to Look |
|----------|---------------|
| What does this job do? | NewTask.md |
| How does architecture work? | ProjectOverview.md |
| What are the bugs? | PostGenerationJob.cs comments |
| How to run tests? | Visual Studio Test Explorer |
| Database structure? | SQL Developer / Oracle |
| Job logs? | Windows Event Viewer |
| Error details? | Application log files |

---

## ?? Typical Job Workflow

```
PostGenerationJob.Work()
?? 1. Get connection to Oracle
?? 2. Read arguments (Mode, Year)
?? 3. Validate Mode exists
?? 4. Parse Mode enum
?? 5. Enable Oracle tracing (for debugging)
?? 6. Block ManualReissue mode
?? 7. Branch:
?  ?? If Annual:
?  ?  ?? Call MembershipDataRepository.FinaliseGeneration()
?  ?? If Adjustment:
?     ?? Validate Year required
?     ?? Call AdjustmentRequestRepository.PostGeneration()
?? 8. Return (job done)
?? 9. Service logs results
```

---

## ? Success Criteria

When you've understood Hugo.Batch, you should be able to answer:

1. ? What type of project is it? (Batch processing system)
2. ? What does TaxGenPst do? (Post-processing after tax generation)
3. ? What are the 3 bugs? (Method names and typos)
4. ? What's the tech stack? (C#, .NET 4.8, Oracle, MEF)
5. ? Why not a web service? (Doesn't need HTTP/real-time)
6. ? How does it run? (Windows Service on schedule)
7. ? What tools do you need? (VS, Git, SQL Developer)
8. ? How do you fix bugs? (Edit, build, test, commit)

---

## ?? Next: Fix the Bugs

**Ready to fix the 3 bugs in PostGenerationJob.cs?**

```csharp
// Line 30: Change this
if (!Arguments.ContainsKeys("Mode"))

// To this
if (!Arguments.ContainsKey("Mode"))


// Line 59: Change this
if (!Arguments.ContainsKeys("Year"))

// To this
if (!Arguments.ContainsKey("Year"))


// Line 71: Change this
var adjustementRequestRepository = new AdjustmentRequestRepository(connection)

// To this
var adjustmentRequestRepository = new AdjustmentRequestRepository(connection)
```

Then rebuild and verify! ?

---

## ?? Quick Reference Links

- **Main Guide:** ProjectOverview.md
- **What's New:** WHATS_NEW_IN_ProjectOverview.md
- **Task Details:** NewTask.md
- **Job Code:** PostGenerationJob.cs

---

**Last Updated:** 2024
**For:** New Joiners to Hugo.Batch Project
**Time to Read:** 5-10 minutes
**Difficulty:** Beginner-Friendly
