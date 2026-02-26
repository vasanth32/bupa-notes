## SonarQube: show .NET test coverage (Demo.OrderApi POC)

This POC uses:

- `dotnet-sonarscanner` (SonarScanner for .NET / MSBuild)
- `coverlet.collector` to generate **OpenCover** coverage reports
- SonarQube reads coverage via: `sonar.cs.opencover.reportsPaths`

---

## 1) Start SonarQube (local)

If you already have SonarQube running (e.g., Docker compose from `SonarQube-POC-Guide.md`), skip this.

Typical UI URL:

- `http://localhost:9000`

---

## 2) Create a Sonar token

In SonarQube UI:

- My Account / Security → generate token

Set it in PowerShell (same terminal you’ll run the scan from):

```powershell
$env:SONAR_TOKEN = "<paste token>"
```

---

## 3) Run scan + tests + coverage (recommended script)

From:

- `d:\HexawareProjects\BUPA\Poc-Projects\Unit Test`

Run:

```powershell
.\run-sonar.ps1 -HostUrl "http://localhost:9000" -ProjectKey "demo-orderapi-unit-test-poc"
```

What the script does:

- `sonarscanner begin`
- `dotnet build`
- `dotnet test` with Coverlet collector configured to output **OpenCover**
- `sonarscanner end`

Coverage output is generated under:

- `tests\**\TestResults\**\coverage.opencover.xml`

---

## 4) Verify in SonarQube UI

Open your project in SonarQube and check:

- **Measures** → Coverage
- **Code** → see covered/uncovered lines per file

If coverage shows **0%**, the usual causes are:

- Coverage format is not OpenCover
- `sonar.cs.opencover.reportsPaths` doesn’t match the actual file path
- Tests didn’t run, or ran without the coverage collector

---

## Manual commands (if you don’t want to use the script)

From `d:\HexawareProjects\BUPA\Poc-Projects\Unit Test`:

```powershell
dotnet tool update --global dotnet-sonarscanner

dotnet sonarscanner begin `
  /k:"demo-orderapi-unit-test-poc" `
  /d:sonar.host.url="http://localhost:9000" `
  /d:sonar.token="$env:SONAR_TOKEN" `
  /d:sonar.cs.opencover.reportsPaths="**/TestResults/**/coverage.opencover.xml"

dotnet build .\UnitTestPoc.sln

dotnet test .\UnitTestPoc.sln --no-build --collect:"XPlat Code Coverage" --settings .\coverlet.runsettings

dotnet sonarscanner end /d:sonar.token="$env:SONAR_TOKEN"
```

