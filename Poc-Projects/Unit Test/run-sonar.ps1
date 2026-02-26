param(
  [Parameter(Mandatory = $false)]
  [string] $ProjectKey = "demo-orderapi-unit-test-poc",

  [Parameter(Mandatory = $false)]
  [string] $ProjectName = "Demo.OrderApi - Unit Test POC",

  [Parameter(Mandatory = $false)]
  [string] $HostUrl = "http://localhost:9000",

  # Set SONAR_TOKEN in your environment (recommended)
  [Parameter(Mandatory = $false)]
  [string] $Token = $env:SONAR_TOKEN
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($Token)) {
  throw "SONAR_TOKEN is not set. Set it first, e.g. `$env:SONAR_TOKEN = '...'` then re-run."
}

Write-Host "==> Ensuring dotnet-sonarscanner is installed"
dotnet tool update --global dotnet-sonarscanner | Out-Host

Write-Host "==> Sonar begin"
dotnet sonarscanner begin `
  /k:"$ProjectKey" `
  /n:"$ProjectName" `
  /d:sonar.host.url="$HostUrl" `
  /d:sonar.token="$Token" `
  /d:sonar.cs.opencover.reportsPaths="**/TestResults/**/coverage.opencover.xml" `
  /d:sonar.exclusions="**/bin/**,**/obj/**" | Out-Host

try {
  Write-Host "==> Build"
  dotnet build .\UnitTestPoc.sln | Out-Host

  Write-Host "==> Test + Coverage (OpenCover)"
  dotnet test .\UnitTestPoc.sln `
    --no-build `
    --collect:"XPlat Code Coverage" `
    --settings .\coverlet.runsettings | Out-Host
}
finally {
  Write-Host "==> Sonar end"
  dotnet sonarscanner end /d:sonar.token="$Token" | Out-Host
}

