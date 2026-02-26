# SonarQube POC with .NET API - Practical Guide

## 🎯 Objective
Create a Proof of Concept (POC) to understand and demonstrate SonarQube integration with .NET API, similar to BUPA's Domain APIs architecture.

## 📋 Prerequisites
- .NET 8 SDK installed
- Docker Desktop (for running SonarQube locally) OR SonarQube Cloud account
- Visual Studio Code / Cursor IDE
- Git

---

## 🚀 Step-by-Step POC Plan

### Phase 1: Setup SonarQube

#### **Cursor Prompt 1: Setup SonarQube with Docker**
```
I want to set up SonarQube locally using Docker. Create a docker-compose.yml file that:
1. Runs SonarQube Community Edition on port 9000
2. Uses PostgreSQL as the database (instead of default H2)
3. Sets up proper volume mounts for data persistence
4. Includes health checks
5. Uses environment variables for configuration

Also create a README with instructions on how to start/stop the services and access SonarQube UI.
```

#### **Expected Output:**
- `docker-compose.yml` file
- `README.md` with setup instructions
- Environment configuration

---

### Phase 2: Create Sample .NET API

#### **Cursor Prompt 2: Create .NET 8 Web API Project**
```
Create a .NET 8 Web API project similar to BUPA's Domain API structure with:
1. Project name: CustomerApi.Poc
2. Structure following Domain-Driven Design:
   - Controllers folder (CustomerController)
   - Services folder (CustomerService)
   - Models folder (Customer, CustomerDto)
   - Repositories folder (ICustomerRepository, CustomerRepository)
3. Include Swagger/OpenAPI
4. Add basic CRUD operations for Customer entity
5. Include some intentional code smells for SonarQube to detect:
   - Unused variables
   - Magic numbers
   - Missing null checks
   - Duplicate code
   - Security vulnerabilities (hardcoded secrets, SQL injection risk)
6. Add a few unit tests using xUnit
7. Include appsettings.json with configuration

Make it similar to BUPA's Customer Domain API structure but simplified for POC purposes.
```

#### **Expected Output:**
- Complete .NET 8 Web API project
- Sample code with intentional issues for SonarQube to detect
- Basic unit tests

---

### Phase 3: Configure SonarQube Scanner

#### **Cursor Prompt 3: Add SonarQube Scanner Configuration**
```
Add SonarQube scanner configuration to the .NET API project:
1. Create a sonar-project.properties file with:
   - Project key and name
   - Source and test directories
   - Exclusions for bin/obj folders
   - Code coverage exclusions
2. Create a .sonarqube folder structure if needed
3. Add SonarScanner.MSBuild NuGet package reference
4. Create a build script (PowerShell or bash) that:
   - Runs dotnet build
   - Runs dotnet test with code coverage
   - Executes SonarQube scanner
   - Generates coverage report
5. Add instructions for running the scanner

Include both local SonarQube and SonarCloud options.
```

#### **Expected Output:**
- `sonar-project.properties`
- Build scripts
- Configuration files

---

### Phase 4: Integrate with CI/CD (Optional but Recommended)

#### **Cursor Prompt 4: Create GitHub Actions Workflow**
```
Create a GitHub Actions workflow file (.github/workflows/sonarqube.yml) that:
1. Builds the .NET API project
2. Runs unit tests with code coverage
3. Analyzes code with SonarQube
4. Uploads results to SonarQube
5. Fails the build if quality gate fails
6. Includes steps for both SonarQube Server and SonarCloud

Also create a workflow for pull requests that runs SonarQube analysis.
```

#### **Expected Output:**
- GitHub Actions workflow files
- CI/CD integration guide

---

### Phase 5: Analyze and Fix Issues

#### **Cursor Prompt 5: Analyze SonarQube Results**
```
I've run SonarQube analysis on my .NET API. Help me:
1. Understand the different types of issues reported:
   - Bugs
   - Vulnerabilities
   - Code Smells
   - Security Hotspots
2. Prioritize which issues to fix first
3. Create a checklist of issues to address
4. Explain what each issue means in the context of .NET development
5. Provide code examples for fixing common issues

Focus on issues that are relevant to BUPA's Domain API development standards.
```

#### **Expected Output:**
- Issue analysis document
- Prioritized fix list
- Code examples for common fixes

---

### Phase 6: Fix Code Issues

#### **Cursor Prompt 6: Fix SonarQube Issues**
```
Review the SonarQube issues in my .NET API and help me fix them:
1. Fix all Critical and Blocker severity issues
2. Address High severity vulnerabilities
3. Refactor code smells (duplicate code, long methods, etc.)
4. Remove unused code and variables
5. Add proper null checks and exception handling
6. Replace magic numbers with constants
7. Fix security issues (remove hardcoded secrets, fix SQL injection risks)
8. Improve code coverage by adding missing tests

After each fix, explain why the fix improves code quality and maintainability.
```

#### **Expected Output:**
- Refactored code
- Improved test coverage
- Documentation of fixes

---

### Phase 7: Quality Gates and Metrics

#### **Cursor Prompt 7: Configure Quality Gates**
```
Help me understand and configure SonarQube Quality Gates:
1. Explain what Quality Gates are
2. Create a custom quality gate for .NET projects with:
   - Code coverage threshold (minimum 80%)
   - Duplicated lines threshold (maximum 3%)
   - Maintainability rating (A)
   - Security rating (A)
   - Reliability rating (A)
3. Set up quality gate conditions relevant to BUPA's standards
4. Create documentation on how to interpret quality gate results
5. Add quality gate status badge to README

Also explain how to configure quality gates for different environments (dev, staging, prod).
```

#### **Expected Output:**
- Quality gate configuration
- Documentation
- Badge setup

---

### Phase 8: Generate Reports and Documentation

#### **Cursor Prompt 8: Create POC Documentation**
```
Create comprehensive documentation for this SonarQube POC:
1. Executive summary of findings
2. Before/after comparison of code quality metrics
3. List of issues found and fixed
4. Code coverage improvements
5. Security improvements
6. Recommendations for BUPA Domain APIs
7. Integration guide for existing projects
8. Best practices for maintaining code quality
9. Screenshots of SonarQube dashboard
10. Lessons learned

Format it as a professional POC report that can be shared with the team.
```

#### **Expected Output:**
- Complete POC report
- Documentation
- Recommendations

---

## 📝 Quick Reference: Cursor Prompts for Common Tasks

### **Prompt: Setup SonarQube Project**
```
Create a new SonarQube project in the SonarQube UI and generate the token. 
Then help me configure the project to connect to my .NET API using the scanner.
```

### **Prompt: Run Analysis**
```
Help me run SonarQube analysis on my .NET API project. 
Include commands for:
- Installing SonarScanner
- Running the analysis
- Viewing results
```

### **Prompt: Understand Specific Issue**
```
I see this SonarQube issue: [paste issue]. 
Explain what it means, why it's a problem, and show me how to fix it with code examples.
```

### **Prompt: Improve Code Coverage**
```
My SonarQube report shows code coverage is 45%. 
Help me identify which parts of my code need tests and create unit tests to improve coverage to at least 80%.
```

### **Prompt: Security Analysis**
```
Analyze my .NET API code for security vulnerabilities using SonarQube findings. 
Focus on:
- Authentication/Authorization issues
- Input validation
- SQL injection risks
- Sensitive data exposure
- Dependency vulnerabilities
```

### **Prompt: Technical Debt**
```
Calculate and explain the technical debt in my project based on SonarQube metrics. 
Create a plan to reduce technical debt with prioritized tasks.
```

---

## 🔧 Tools and Commands Reference

### **SonarQube Scanner Commands**
```bash
# Install SonarScanner for .NET
dotnet tool install --global dotnet-sonarscanner

# Begin analysis
dotnet sonarscanner begin /k:"CustomerApi.Poc" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="your-token"

# Build and test
dotnet build
dotnet test --collect:"XPlat Code Coverage"

# End analysis
dotnet sonarscanner end /d:sonar.login="your-token"
```

### **Docker Commands**
```bash
# Start SonarQube
docker-compose up -d

# View logs
docker-compose logs -f sonarqube

# Stop SonarQube
docker-compose down
```

---

## 📊 Key Metrics to Track

1. **Code Coverage**: Target 80%+
2. **Duplicated Lines**: Target < 3%
3. **Maintainability Rating**: Target A
4. **Security Rating**: Target A
5. **Reliability Rating**: Target A
6. **Technical Debt**: Track and reduce over time
7. **Code Smells**: Reduce count
8. **Bugs**: Zero critical/high severity
9. **Vulnerabilities**: Zero critical/high severity

---

## 🎓 Learning Objectives

By the end of this POC, you should understand:
- ✅ How to set up and run SonarQube
- ✅ How to integrate SonarQube with .NET projects
- ✅ How to interpret SonarQube reports
- ✅ How to fix common code quality issues
- ✅ How to configure quality gates
- ✅ How to integrate with CI/CD pipelines
- ✅ Best practices for maintaining code quality

---

## 📚 Additional Resources

- [SonarQube Documentation](https://docs.sonarqube.org/)
- [SonarScanner for .NET](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner-for-msbuild/)
- [SonarQube Quality Gates](https://docs.sonarqube.org/latest/user-guide/quality-gates/)
- [.NET Code Analysis Rules](https://docs.sonarqube.org/latest/user-guide/rules/)

---

## 🚨 Troubleshooting Common Issues

### **Issue: Cannot connect to SonarQube**
**Cursor Prompt:**
```
I'm getting a connection error when trying to run SonarQube scanner. 
Help me troubleshoot:
1. Check if SonarQube is running
2. Verify the URL and port
3. Check firewall settings
4. Verify authentication token
```

### **Issue: Coverage not showing**
**Cursor Prompt:**
```
Code coverage is not appearing in SonarQube. 
Help me:
1. Configure code coverage tool (coverlet)
2. Generate coverage report in correct format
3. Configure SonarQube to read coverage file
4. Verify coverage file path in sonar-project.properties
```

### **Issue: Too many false positives**
**Cursor Prompt:**
```
SonarQube is reporting many false positive issues. 
Help me:
1. Configure issue exclusions
2. Mark false positives as "Won't Fix"
3. Adjust rule severity
4. Create custom rule sets
```

---

## ✅ POC Completion Checklist

- [ ] SonarQube running locally or cloud instance configured
- [ ] .NET API project created with sample code
- [ ] SonarQube scanner configured and working
- [ ] Initial analysis completed
- [ ] Issues documented and prioritized
- [ ] Critical and high severity issues fixed
- [ ] Code coverage improved to target (80%+)
- [ ] Quality gate configured and passing
- [ ] CI/CD integration completed (optional)
- [ ] POC documentation and report created
- [ ] Lessons learned documented
- [ ] Recommendations for BUPA project prepared

---

*This guide is designed to be used with Cursor AI to help you learn SonarQube through hands-on practice.*

