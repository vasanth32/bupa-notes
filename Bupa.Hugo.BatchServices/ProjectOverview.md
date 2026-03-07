# Hugo.Batch Project - Complete Beginner's Guide

## ?? Table of Contents

1. [Project Overview](#project-overview)
2. [What is Hugo.Batch?](#what-is-hugobatch)
3. [Project Structure](#project-structure)
4. [Key Components](#key-components)
5. [Technology Stack](#technology-stack)
6. [Technologies & Versions (Detailed)](#-technologies--versions-detailed)
7. [Development Tools Used](#-development-tools-used)
8. [Architecture Type: WHAT IS THIS PROJECT?](#-architecture-type-what-is-this-project)
9. [Batch vs Real-Time Processing](#-batch-vs-real-time-processing)
10. [How It Works](#how-it-works)
11. [The TaxGenPst Job (Your Task)](#the-taxgenpst-job-your-task)
12. [Getting Started](#getting-started)
13. [Common Workflows](#common-workflows)
14. [FAQ for Beginners](#faq-for-beginners)

---

## Project Overview

### What is This Project?

**Hugo.Batch** is a **batch processing system** built by **Bupa Australia & New Zealand** (a health insurance company) for processing tax-related data and operations.

**Simple Explanation:** Imagine you have thousands of customer tax records that need to be processed overnight. Hugo.Batch is the system that:
- ? Schedules jobs to run automatically at specific times
- ? Processes large amounts of data efficiently
- ? Validates and organizes tax documents
- ? Sends reports to different departments
- ? Handles errors gracefully if something goes wrong

---

## What is Hugo.Batch?

### Purpose

Hugo.Batch processes **tax generation workflows** for Bupa insurance members. It:

1. **Generates** tax statements for members
2. **Distributes** these documents via email or mail
3. **Validates** the output
4. **Makes adjustments** if needed
5. **Finalizes** annual tax generation runs

### Why is it Important?

- Handles **millions of customer records**
- Runs **automatically** without manual intervention
- **Ensures accuracy** of tax documents
- **Tracks history** of all operations
- **Scales efficiently** for large datasets

---

## Project Structure

### Directory Layout

```
Hugo.Batch/
?
??? Server/                              (Main server-side code)
?   ??? HugoBatchJobs/                   (Individual batch jobs)
?   ?   ??? Tax/                         (Tax-related jobs)
?   ?   ?   ??? Bupa.Hugo.Batch.Job.Tax.PostGeneration/    ? TaxGenPst
?   ?   ?   ??? Bupa.Hugo.Batch.Job.Tax.Generation/
?   ?   ?   ??? Bupa.Hugo.Batch.Job.Tax.Distribution/
?   ?   ?   ??? Bupa.Hugo.Batch.Job.Tax.Trigger/
?   ?   ?   ??? Bupa.Hugo.Batch.Job.Tax.Verify/
?   ?   ?   ??? ... (other tax jobs)
?   ?   ?
?   ?   ??? PaymentFileTransfer/         (Payment processing)
?   ?   ??? AgeBased/                    (Age-based operations)
?   ?   ??? SalesCommissions/            (Commission calculations)
?   ?
?   ??? HugoBatchProcessor/              (Core batch framework)
?   ?   ??? Bupa.Hugo.Batch.Common/      (Shared utilities)
?   ?   ??? Bupa.Hugo.Batch.Job.Common/  (Base job classes)
?   ?   ??? Bupa.Hugo.Batch.Master/      (Job orchestration)
?   ?   ??? ... (other framework components)
?   ?
?   ??? Hugo_Services/                   (Service layer)
?       ??? BatchServices/               (Batch service host)
?       ?   ??? BatchServiceHost/        (Windows Service host)
?       ?   ??? BatchServiceLibrary/     (Service implementation)
?       ?   ??? BatchEmailProcessor/     (Email handling)
?       ?   ??? BatchProspectProcessor/  (Prospect data)
?       ?   ??? ... (other services)
?       ?
?       ??? CommonTests/                 (Shared test code)
?
??? Common/                              (Shared libraries)
    ??? Encryption/                      (Encryption utilities)
    ??? Cyclops_Lib/                     (Common utilities)
```

### Simple Breakdown

| Folder | Purpose | Example |
|--------|---------|---------|
| **HugoBatchJobs** | Individual tasks (jobs) | TaxGenPst job runs after tax statement generation |
| **HugoBatchProcessor** | Framework & infrastructure | Job scheduling, logging, error handling |
| **Hugo_Services** | Service layer (background service) | Hosts all jobs and runs them on schedule |
| **Common** | Shared utilities | Encryption, database helpers |

---

## Key Components

### 1. **Hugo.Batch.Job.Tax.PostGeneration** (TaxGenPst)

**What is it?**
A batch job that runs **after** tax statements are generated and distributed.

**What does it do?**
- ? Validates execution mode
- ? Finalizes annual tax generation runs
- ? Executes adjustment logic for non-annual runs
- ? Does NOT generate statements (that's a different job)

**File Location:**
```
Server\HugoBatchJobs\Tax\Bupa.Hugo.Batch.Job.Tax.PostGeneration\
??? PostGenerationJob.cs  (Main code with 3 bugs to fix)
```

**Key Features:**
```
Supported Modes:
- Annual              ? Finalize annual tax generation
- AdjustmentForMailHouse ? Adjust records for mail house
- AdjustmentForATO   ? Adjust records for ATO (tax office)
- SystemFix          ? System correction adjustments
- ManualReissue      ? NOT ALLOWED (blocked)
```

---

### 2. **Hugo.Batch.Job.Tax.Generation**

**Purpose:** Generates tax statements for Bupa members

**Flow:**
1. Read member data from database
2. Calculate tax information
3. Generate PDF documents
4. Store in file system

---

### 3. **Hugo.Batch.Job.Tax.Distribution**

**Purpose:** Distributes generated tax statements

**Flow:**
1. Pick up generated statements
2. Format for email or postal mail
3. Send via email processor or mail house
4. Track delivery status

---

### 4. **Hugo_Services (BatchService)**

**What is it?**
A **Windows Service** that hosts and runs all batch jobs

**How it works:**
- Runs 24/7 in the background
- Executes jobs on a schedule
- Monitors job execution
- Logs all activity
- Handles failures gracefully

**Location:**
```
Server\Hugo_Services\BatchServices\BatchServiceHost\
```

---

### 5. **Bupa.Hugo.Batch.Common** (Shared Framework)

**Purpose:** Provides common utilities used by all jobs

**Includes:**
- Database connection helpers
- Oracle database helpers
- Logging framework
- Job scheduling
- Exception handling
- Configuration management

---

## Technology Stack

### Programming Language & Framework
- **Language:** C# (.NET Framework 4.8)
- **Framework:** .NET Framework 4.8 (legacy, but stable)

### Database
- **Oracle Database** (used for storing tax data)
- **OracleHelper** - Manages database connections
- **Stored Procedures** - SQL logic in Oracle

### Job Framework
- **Microsoft Managed Extensibility Framework (MEF)**
  - Jobs are loaded as "AddIns"
  - Allows dynamic job loading without recompiling

### Key Libraries & Tools

| Library | Purpose | Used For |
|---------|---------|----------|
| **Bupa.Common.Extension** | Extension methods | Dictionary operations, string utilities |
| **Bupa.Common.Oracle.Helper** | Oracle database access | SQL execution, connection management |
| **System.AddIn** | Plugin framework | Job discovery and loading |
| **NUnit / MSTest** | Unit testing | Writing tests for jobs |
| **log4net** | Logging | Tracking job execution |

---

## ?? Technologies & Versions (Detailed)

### Core Framework & Language

| Technology | Version | Purpose | Released |
|------------|---------|---------|----------|
| **.NET Framework** | 4.8 | Foundation for all C# applications | April 2019 |
| **C#** | 7.3 | Programming language | May 2018 |
| **Windows** | Server 2016+ | Operating system for service host | 2016+ |
| **Visual Studio** | 2019/2022 | Development IDE | 2019/2022 |

### Database & ORM

| Technology | Version | Purpose | Usage |
|------------|---------|---------|-------|
| **Oracle Database** | 11g / 12c | Enterprise database | Stores all tax data, member info |
| **Oracle Data Provider (ODP.NET)** | Latest | .NET Oracle driver | Connect to Oracle from C# |
| **Stored Procedures** | PL/SQL | Database logic | TAX_GEN_PKG, TAX_ADJ packages |
| **SQL** | Standard | Query language | Data retrieval and manipulation |

### Testing Frameworks

| Framework | Version | Purpose | Usage |
|-----------|---------|---------|-------|
| **NUnit** | 3.x | Unit testing framework | Writing unit tests |
| **MSTest** | Latest | Microsoft testing framework | Integration tests |
| **Moq** | Latest | Mocking library | Creating mock objects for tests |

### Logging & Monitoring

| Library | Version | Purpose | Configuration |
|---------|---------|---------|----------------|
| **log4net** | 2.x | Logging framework | log4net.config file |
| **Enterprise Library** | 6.0+ | Logging blocks | Application blocks |
| **Windows Event Viewer** | Built-in | System logging | Track job execution |
| **Oracle Trace** | Built-in | Database tracing | Performance monitoring |

### Additional Libraries

| Library | Version | Purpose | Used In |
|---------|---------|---------|---------|
| **NuGet** | Latest | Package manager | Managing dependencies |
| **MEF (System.AddIn)** | .NET 4.8 | Plugin framework | Job loading mechanism |
| **System.Configuration** | .NET 4.8 | Configuration management | App.config reading |
| **System.ServiceProcess** | .NET 4.8 | Windows Service | BatchServiceHost |
| **System.Xml** | .NET 4.8 | XML handling | Configuration files |

---

## ?? Development Tools Used

### Primary Tools

| Tool | Version | Purpose |
|------|---------|---------|
| **Visual Studio** | 2019 or 2022 | IDE for development |
| **Git / Azure DevOps** | Latest | Version control |
| **SQL Developer / Toad** | Latest | Oracle database tools |
| **Notepad++** | 8.x | Text editing |
| **Fiddler** | Latest | HTTP debugging (if API calls) |
| **Windows Scheduler** | Built-in | Job scheduling |

### Build & Deployment

| Tool | Purpose | Used For |
|------|---------|----------|
| **MSBuild** | Build system | Compiling C# projects |
| **NuGet Package Manager** | Dependency management | Installing libraries |
| **Visual Studio Installer** | Setup | Installing .NET Framework |
| **Windows Service Installer** | Deployment | Installing batch service |

### Testing Tools

| Tool | Purpose |
|------|---------|
| **Test Explorer** (Visual Studio) | Running unit tests |
| **Code Coverage** | Measuring test coverage |
| **Resharper** | Code analysis and quality |
| **StyleCop** | Code style enforcement |

---

## ?? Architecture Type: WHAT IS THIS PROJECT?

### Is This WCF? ? **NO**

**WCF (Windows Communication Foundation)** is for:
- Building web services (SOAP)
- REST APIs
- Distributed communication

**This Project:** ? Not WCF
- This is a **Batch Processing System**, not a web service
- No HTTP endpoints
- No SOAP or REST APIs
- No client-server communication

---

### Is This API? ? **NO**

**API (Application Programming Interface)** is for:
- REST endpoints (HTTP)
- JSON/XML responses
- Client applications calling services
- Real-time communication

**This Project:** ? Not an API
- No HTTP endpoints
- No web requests
- No JSON responses
- Runs offline/scheduled

---

### Is This Web Form / Web Application? ? **NO**

**Web Forms / MVC / ASP.NET** are for:
- Browser-based user interfaces
- Web pages
- User interactions
- Session management

**This Project:** ? Not Web-based
- No web pages
- No user interface
- No browsers
- No HTTP requests from users

---

### What IS This Project? ? **BATCH PROCESSING SYSTEM**

```
????????????????????????????????????????????????????????????????
?                                                               ?
?           BATCH PROCESSING SYSTEM / SCHEDULED JOBS            ?
?                                                               ?
?   A background service that runs jobs automatically on        ?
?   a schedule, without user interaction                        ?
?                                                               ?
????????????????????????????????????????????????????????????????
