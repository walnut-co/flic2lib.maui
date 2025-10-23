#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Test the package creation process locally before deploying to NuGet
.DESCRIPTION
    This script simulates the GitHub Actions workflow locally to test package creation
.PARAMETER Version
    The version to use for the packages (e.g., "1.0.7")
.PARAMETER SkipBuild
    Skip the build process and only create packages from existing binaries
.EXAMPLE
    .\test-deployment.ps1 -Version "1.0.7"
.EXAMPLE
    .\test-deployment.ps1 -Version "1.0.7" -SkipBuild
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    
    [Parameter(Mandatory = $false)]
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

# Color output functions
function Write-Success($message) { Write-Host "✓ $message" -ForegroundColor Green }
function Write-Info($message) { Write-Host "ℹ $message" -ForegroundColor Cyan }
function Write-Warning($message) { Write-Host "⚠ $message" -ForegroundColor Yellow }
function Write-Error($message) { Write-Host "✗ $message" -ForegroundColor Red }

# Validate version format
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Error "Version must be in format X.Y.Z (e.g., 1.0.7)"
    exit 1
}

Write-Info "Testing package creation for version $Version"

try {
    # Clean previous test results
    Write-Info "Cleaning previous test results..."
    Remove-Item -Path "test-packages" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item -Path "*.nupkg" -Force -ErrorAction SilentlyContinue
    New-Item -ItemType Directory -Path "test-packages" -Force | Out-Null

    # Update version in nuspec files
    Write-Info "Updating version in nuspec files..."
    
    # Update flic2lib.Platforms.simple.nuspec
    $platformsNuspec = Get-Content "flic2lib.Platforms.simple.nuspec" -Raw
    $originalPlatformsVersion = [regex]::Match($platformsNuspec, '(?<=<metadata>[\s\S]*?)<version>([\d\.]+)</version>').Groups[1].Value
    $platformsNuspec = $platformsNuspec -replace '(?<=<metadata>[\s\S]*?)<version>[\d\.]+</version>', "<version>$Version</version>"
    Set-Content "flic2lib.Platforms.simple.nuspec" -Value $platformsNuspec
    Write-Success "Updated Platforms nuspec: $originalPlatformsVersion → $Version"
    
    # Update flic2lib.Maui.nuspec with more specific patterns
    $mauiNuspec = Get-Content "flic2lib.Maui.nuspec" -Raw
    $originalMauiVersion = [regex]::Match($mauiNuspec, '(?<=<metadata>[\s\S]*?)<version>([\d\.]+)</version>').Groups[1].Value
    
    # Replace package version (in metadata section only)
    $mauiNuspec = $mauiNuspec -replace '(?<=<metadata>[\s\S]*?)<version>[\d\.]+</version>', "<version>$Version</version>"
    
    # Replace dependency versions
    $mauiNuspec = $mauiNuspec -replace '(?<=<dependency[^>]*version=")[^"]*(?=")', $Version
    
    Set-Content "flic2lib.Maui.nuspec" -Value $mauiNuspec
    Write-Success "Updated MAUI nuspec: $originalMauiVersion → $Version"

    if (-not $SkipBuild) {
        # Build projects
        Write-Info "Building projects..."
        
        Write-Info "Restoring dependencies..."
        dotnet restore flic2lib.maui.sln
        if ($LASTEXITCODE -ne 0) { throw "Failed to restore dependencies" }
        
        Write-Info "Building Android binding project..."
        dotnet build flic2lib.android/flic2lib.android.csproj --configuration Release --no-restore
        if ($LASTEXITCODE -ne 0) { throw "Failed to build Android project" }
        
        Write-Info "Building iOS binding project..."
        dotnet build flic2lib.ios/flic2lib.ios.csproj --configuration Release --no-restore
        if ($LASTEXITCODE -ne 0) { throw "Failed to build iOS project" }
        
        Write-Info "Building MAUI library..."
        dotnet build flic2lib.Maui/flic2lib.Maui.csproj --configuration Release --no-restore
        if ($LASTEXITCODE -ne 0) { throw "Failed to build MAUI project" }
        
        Write-Success "All projects built successfully"
    } else {
        Write-Warning "Skipping build process - using existing binaries"
    }

    # Verify build outputs exist
    Write-Info "Verifying build outputs..."
    $requiredFiles = @(
        "flic2lib.android/bin/Release/net9.0-android/flic2lib.android.dll",
        "flic2lib.ios/bin/Release/net9.0-ios/flic2lib.ios.dll",
        "flic2lib.Maui/bin/Release/net9.0-android/flic2lib.Maui.dll",
        "flic2lib.Maui/bin/Release/net9.0-ios/flic2lib.Maui.dll"
    )
    
    foreach ($file in $requiredFiles) {
        if (-not (Test-Path $file)) {
            Write-Error "Required file missing: $file"
            throw "Build verification failed"
        } else {
            Write-Success "Found: $file"
        }
    }

    # Create packages
    Write-Info "Creating NuGet packages..."
    
    # Create Platforms package
    Write-Info "Creating Platforms package..."
    nuget pack flic2lib.Platforms.simple.nuspec -Properties "copyright=Copyright 2025 Walnut"
    if ($LASTEXITCODE -ne 0) { throw "Failed to create Platforms package" }
    
    $platformsPackage = "walnut.flic2lib.Platforms.$Version.nupkg"
    if (-not (Test-Path $platformsPackage)) {
        throw "Platforms package not found: $platformsPackage"
    }
    Write-Success "Created Platforms package: $platformsPackage"
    
    # Create MAUI package
    Write-Info "Creating MAUI package..."
    nuget pack flic2lib.Maui.nuspec
    if ($LASTEXITCODE -ne 0) { throw "Failed to create MAUI package" }
    
    $mauiPackage = "walnut.flic2lib.Maui.$Version.nupkg"
    if (-not (Test-Path $mauiPackage)) {
        throw "MAUI package not found: $mauiPackage"
    }
    Write-Success "Created MAUI package: $mauiPackage"

    # Move packages to test directory and show results
    Copy-Item $platformsPackage "test-packages/"
    Copy-Item $mauiPackage "test-packages/"
    
    Write-Success "Package creation completed successfully!"
    Write-Info "Package files created:"
    Get-ChildItem "test-packages" -Filter "*.nupkg" | ForEach-Object {
        $size = [math]::Round($_.Length / 1KB, 2)
        Write-Host "  $($_.Name) ($size KB)" -ForegroundColor Cyan
    }
    
    # Basic package validation
    Write-Info "Performing basic package validation..."
    
    # Check if packages can be extracted (basic format validation)
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    
    try {
        $zip = [System.IO.Compression.ZipFile]::OpenRead((Resolve-Path "test-packages/$platformsPackage").Path)
        $entryCount = $zip.Entries.Count
        $zip.Dispose()
        Write-Success "Platforms package format valid ($entryCount files)"
    } catch {
        Write-Error "Platforms package format validation failed: $($_.Exception.Message)"
    }
    
    try {
        $zip = [System.IO.Compression.ZipFile]::OpenRead((Resolve-Path "test-packages/$mauiPackage").Path)
        $entryCount = $zip.Entries.Count
        $zip.Dispose()
        Write-Success "MAUI package format valid ($entryCount files)"
    } catch {
        Write-Error "MAUI package format validation failed: $($_.Exception.Message)"
    }
    
    Write-Success "Test deployment completed successfully!"
    Write-Warning "Packages are ready in the 'test-packages' directory"
    Write-Info "To deploy to NuGet, use the GitHub Actions workflow or run:"
    Write-Info "  nuget push test-packages/$platformsPackage -ApiKey YOUR_API_KEY -Source https://api.nuget.org/v3/index.json"
    Write-Info "  nuget push test-packages/$mauiPackage -ApiKey YOUR_API_KEY -Source https://api.nuget.org/v3/index.json"

} catch {
    Write-Error "Test deployment failed: $($_.Exception.Message)"
    exit 1
} finally {
    # Restore original versions in nuspec files if they were changed
    Write-Info "Restoring original nuspec versions..."
    
    if ($originalPlatformsVersion) {
        $platformsNuspec = Get-Content "flic2lib.Platforms.simple.nuspec" -Raw
        $platformsNuspec = $platformsNuspec -replace '(?<=<metadata>[\s\S]*?)<version>[\d\.]+</version>', "<version>$originalPlatformsVersion</version>"
        Set-Content "flic2lib.Platforms.simple.nuspec" -Value $platformsNuspec
        Write-Info "Restored Platforms nuspec to version $originalPlatformsVersion"
    }
    
    if ($originalMauiVersion) {
        $mauiNuspec = Get-Content "flic2lib.Maui.nuspec" -Raw
        $mauiNuspec = $mauiNuspec -replace '(?<=<metadata>[\s\S]*?)<version>[\d\.]+</version>', "<version>$originalMauiVersion</version>"
        $mauiNuspec = $mauiNuspec -replace '(?<=<dependency[^>]*version=")[^"]*(?=")', $originalMauiVersion
        Set-Content "flic2lib.Maui.nuspec" -Value $mauiNuspec
        Write-Info "Restored MAUI nuspec to version $originalMauiVersion"
    }
}