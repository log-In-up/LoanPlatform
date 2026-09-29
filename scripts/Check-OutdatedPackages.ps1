
$ErrorActionPreference = "Stop"

$solution = Join-Path $PSScriptRoot "..\LoanPlatform.sln"

if (-not (Test-Path $solution)) {
    Write-Error "Solution file not found: $solution"
    exit 1
}

Write-Host "Checking outdated NuGet packages..." -ForegroundColor Cyan
Write-Host ""

dotnet list $solution package --outdated --highest-patch

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to check outdated packages."
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Package check completed." -ForegroundColor Green