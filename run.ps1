<#
.SYNOPSIS
Run an irooon script

.PARAMETER Script
The .iro script file to execute

.EXAMPLE
.\run.ps1 examples/hello.iro
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$Script
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $Script)) {
    Write-Error "Script file not found: $Script"
    exit 1
}

dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj -- $Script
