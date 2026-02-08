<#
.SYNOPSIS
irooon build script

.DESCRIPTION
Build or publish the irooon interpreter.

.PARAMETER Configuration
Build configuration (Debug or Release). Default is Release.

.PARAMETER Clean
Run clean before build.

.PARAMETER Publish
Run publish build (generates self-contained executable).

.PARAMETER Test
Run tests after build.

.PARAMETER Runtime
Runtime identifier for publish (e.g., win-x64, linux-x64, osx-x64).

.EXAMPLE
.\build.ps1
Build with default settings (Release).

.EXAMPLE
.\build.ps1 -Configuration Debug -Test
Build in Debug mode and run tests.

.EXAMPLE
.\build.ps1 -Publish -Runtime win-x64
Generate self-contained executable for Windows x64.

.EXAMPLE
.\build.ps1 -Clean -Publish
Run clean build and publish.
#>

param(
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [Parameter()]
    [switch]$Clean,

    [Parameter()]
    [switch]$Publish,

    [Parameter()]
    [switch]$Test,

    [Parameter()]
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "irooon Build Script" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan
Write-Host ""

if ($Clean) {
    Write-Host "Running clean..." -ForegroundColor Yellow
    dotnet clean --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Clean failed"
        exit $LASTEXITCODE
    }
    Write-Host "Clean completed" -ForegroundColor Green
    Write-Host ""
}

if ($Publish) {
    Write-Host "Running publish build..." -ForegroundColor Yellow
    Write-Host "  Configuration: $Configuration" -ForegroundColor Gray
    Write-Host "  Runtime: $Runtime" -ForegroundColor Gray

    $publishArgs = @(
        "publish"
        "src/Irooon.Cli/Irooon.Cli.csproj"
        "--configuration", $Configuration
        "--runtime", $Runtime
        "--self-contained", "true"
        "--output", "publish/$Runtime"
        "/p:PublishSingleFile=true"
        "/p:PublishTrimmed=true"
    )

    & dotnet @publishArgs

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Publish failed"
        exit $LASTEXITCODE
    }

    Write-Host "Publish completed: publish/$Runtime" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "Running build..." -ForegroundColor Yellow
    Write-Host "  Configuration: $Configuration" -ForegroundColor Gray

    dotnet build --configuration $Configuration

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed"
        exit $LASTEXITCODE
    }

    Write-Host "Build completed" -ForegroundColor Green
    Write-Host ""
}

if ($Test) {
    Write-Host "Running tests..." -ForegroundColor Yellow

    dotnet test --configuration $Configuration --no-build --verbosity normal

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed"
        exit $LASTEXITCODE
    }

    Write-Host "Tests completed" -ForegroundColor Green
    Write-Host ""
}

Write-Host "All operations completed successfully!" -ForegroundColor Green
