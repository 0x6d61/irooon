# irooon リリーススクリプト
# プロジェクトルート/Release ディレクトリに単一の実行可能ファイルを出力

param(
    [string]$Version = "0.7.0",
    [switch]$Windows,
    [switch]$Linux,
    [switch]$Mac,
    [switch]$All
)

$ErrorActionPreference = "Stop"

# プロジェクトルートに移動
$scriptDir = Split-Path -Parent $PSCommandPath
$projectRoot = Split-Path -Parent $scriptDir
Set-Location $projectRoot

Write-Host "=== irooon Release Build ===" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Green
Write-Host ""

# Releaseディレクトリを作成
$releaseDir = Join-Path $projectRoot "Release"
if (Test-Path $releaseDir) {
    Write-Host "Cleaning Release directory..." -ForegroundColor Yellow
    Remove-Item -Path $releaseDir -Recurse -Force
}
New-Item -Path $releaseDir -ItemType Directory -Force | Out-Null

# ビルド関数
function Build-Platform {
    param(
        [string]$Runtime,
        [string]$PlatformName
    )

    Write-Host "Building for $PlatformName ($Runtime)..." -ForegroundColor Cyan

    $outputDir = Join-Path $releaseDir $PlatformName

    # dotnet publish で単一ファイル実行可能ファイルを生成
    dotnet publish src/Irooon.Cli/Irooon.Cli.csproj `
        --configuration Release `
        --runtime $Runtime `
        --self-contained true `
        --output $outputDir `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:EnableCompressionInSingleFile=true `
        -p:Version=$Version

    if ($LASTEXITCODE -ne 0) {
        throw "Build failed for $PlatformName"
    }

    Write-Host "✓ Build completed: $outputDir" -ForegroundColor Green

    # 実行可能ファイルのパスを表示
    $exeName = if ($Runtime -like "win-*") { "Irooon.Cli.exe" } else { "Irooon.Cli" }
    $exePath = Join-Path $outputDir $exeName
    if (Test-Path $exePath) {
        $fileSize = (Get-Item $exePath).Length / 1MB
        Write-Host "  Executable: $exePath" -ForegroundColor Gray
        Write-Host "  Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Gray
    }
    Write-Host ""
}

# プラットフォームごとにビルド
if ($All -or $Windows) {
    Build-Platform -Runtime "win-x64" -PlatformName "windows-x64"
}

if ($All -or $Linux) {
    Build-Platform -Runtime "linux-x64" -PlatformName "linux-x64"
}

if ($All -or $Mac) {
    Build-Platform -Runtime "osx-x64" -PlatformName "macos-x64"
    Build-Platform -Runtime "osx-arm64" -PlatformName "macos-arm64"
}

# デフォルト（オプション指定なし）の場合はWindows x64のみビルド
if (-not ($All -or $Windows -or $Linux -or $Mac)) {
    Build-Platform -Runtime "win-x64" -PlatformName "windows-x64"
}

Write-Host "=== Release Build Complete ===" -ForegroundColor Green
Write-Host "Output directory: $releaseDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "Usage:" -ForegroundColor Yellow
Write-Host "  .\scripts\release.ps1           # Windows x64 のみ"
Write-Host "  .\scripts\release.ps1 -All      # すべてのプラットフォーム"
Write-Host "  .\scripts\release.ps1 -Windows  # Windows のみ"
Write-Host "  .\scripts\release.ps1 -Linux    # Linux のみ"
Write-Host "  .\scripts\release.ps1 -Mac      # macOS のみ"
