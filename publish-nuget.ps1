<#
.SYNOPSIS
    Build, pack, and publish Salar.BinaryBuffers to NuGet.

.DESCRIPTION
    This script:
      1. Restores and builds the library project in Release configuration.
      2. Runs the test suite (skippable with -SkipTests).
      3. Packs the project into a .nupkg file.
      4. Pushes the package to nuget.org.

    The NuGet API key is never required on the command line.
    Resolution order:
      a) -ApiKey parameter (useful for non-interactive automation)
      b) NUGET_API_KEY environment variable (recommended for CI/CD)
      c) Interactive prompt (masked input — key is not echoed or stored)

.PARAMETER Version
    Override the package version (e.g. 3.4.0). When omitted the version
    declared in the .csproj file is used.

.PARAMETER ApiKey
    NuGet API key. Prefer setting the NUGET_API_KEY environment variable
    instead of passing this parameter, so the key is never visible in your
    shell history.

.PARAMETER OutputDir
    Directory where .nupkg files are written. Defaults to 'artifacts/nuget'
    under the repository root.

.PARAMETER SkipTests
    Skip the test run.

.PARAMETER SkipPush
    Build and pack only; do not push to nuget.org. Useful for a dry run.

.EXAMPLE
    # Interactive — prompts for the API key
    .\publish-nuget.ps1

.EXAMPLE
    # CI/CD — key comes from an environment variable
    $env:NUGET_API_KEY = '<secret>'
    .\publish-nuget.ps1 -Version 3.4.0

.EXAMPLE
    # Dry run: build and pack without pushing
    .\publish-nuget.ps1 -SkipPush
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $Version   = '',
    [string] $ApiKey    = '',
    [string] $OutputDir = '',
    [switch] $SkipTests,
    [switch] $SkipPush
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

# ---------------------------------------------------------------------------
# Paths
# ---------------------------------------------------------------------------
$repoRoot = $PSScriptRoot
$libProj  = Join-Path $repoRoot 'Salar.BinaryBuffers/Salar.BinaryBuffers.csproj'
$testProj = Join-Path $repoRoot 'Salar.BinaryBuffers.Tests/Salar.BinaryBuffers.Tests.csproj'

if (-not $OutputDir) {
    $OutputDir = Join-Path $repoRoot 'artifacts/nuget'
}

# ---------------------------------------------------------------------------
# Helper
# ---------------------------------------------------------------------------
function Invoke-Cmd {
    param([string]$Description, [scriptblock]$Cmd)
    Write-Host ""
    Write-Host "==> $Description" -ForegroundColor Cyan
    & $Cmd
    if ($LASTEXITCODE -ne 0) {
        Write-Error "$Description failed with exit code $LASTEXITCODE."
    }
}

# ---------------------------------------------------------------------------
# Version override (optional)
# ---------------------------------------------------------------------------
$versionArgs = @()
if ($Version) {
    $versionArgs = @("/p:Version=$Version")
    Write-Host "Using version override: $Version" -ForegroundColor Yellow
}

# ---------------------------------------------------------------------------
# Clean output directory
# ---------------------------------------------------------------------------
if (Test-Path $OutputDir) {
    Remove-Item -Recurse -Force $OutputDir
}
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

# ---------------------------------------------------------------------------
# Restore & Build
# ---------------------------------------------------------------------------
Invoke-Cmd 'Restore packages' {
    dotnet restore $libProj
}

Invoke-Cmd 'Build Salar.BinaryBuffers (Release)' {
    dotnet build $libProj --configuration Release --no-restore @versionArgs
}

# ---------------------------------------------------------------------------
# Test
# ---------------------------------------------------------------------------
if (-not $SkipTests) {
    if (Test-Path $testProj) {
        Invoke-Cmd 'Run tests' {
            dotnet test $testProj --configuration Release
        }
    }
    else {
        Write-Host "No test project found at '$testProj' — skipping tests." -ForegroundColor Yellow
    }
}

# ---------------------------------------------------------------------------
# Pack
# ---------------------------------------------------------------------------
Invoke-Cmd 'Pack Salar.BinaryBuffers' {
    dotnet pack $libProj --configuration Release --no-build --output $OutputDir @versionArgs
}

Write-Host ""
Write-Host "Package written to: $OutputDir" -ForegroundColor Green
Get-ChildItem -Path $OutputDir -Filter '*.nupkg' | ForEach-Object {
    Write-Host "  $($_.Name)" -ForegroundColor Green
}

if ($SkipPush) {
    Write-Host ""
    Write-Host "-SkipPush specified — package was not published." -ForegroundColor Yellow
    return
}

# ---------------------------------------------------------------------------
# Resolve API key — never required on the command line
# ---------------------------------------------------------------------------
if (-not $ApiKey) {
    if ($env:NUGET_API_KEY) {
        $ApiKey = $env:NUGET_API_KEY
        Write-Host ""
        Write-Host "Using API key from NUGET_API_KEY environment variable." -ForegroundColor DarkGray
    }
    else {
        Write-Host ""
        # Read-Host masks the input so the key is not visible or logged.
        $secureKey = Read-Host 'Enter your nuget.org API key' -AsSecureString
        if ($secureKey.Length -eq 0) {
            Write-Error 'No API key provided. Aborting push.'
        }
        $ApiKey = [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
            [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureKey)
        )
    }
}

# ---------------------------------------------------------------------------
# Push
# ---------------------------------------------------------------------------
$nugetSource = 'https://api.nuget.org/v3/index.json'
$packages    = Get-ChildItem -Path $OutputDir -Filter '*.nupkg'

if (-not $packages) {
    Write-Error "No .nupkg files found in '$OutputDir'."
}

foreach ($pkg in $packages) {
    Invoke-Cmd "Push $($pkg.Name)" {
        dotnet nuget push $pkg.FullName `
            --api-key $ApiKey `
            --source $nugetSource `
            --skip-duplicate
    }
}

Write-Host ""
Write-Host "Package published successfully." -ForegroundColor Green
