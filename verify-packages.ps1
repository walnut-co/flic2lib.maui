# NuGet Package Verification Script
# Run this to verify the generated packages work correctly

Write-Host "=== NuGet Package Verification ===" -ForegroundColor Green

# Check if packages exist
$mauiPackage = "walnut.flic2lib.Maui.1.0.4.nupkg"
$platformsPackage = "walnut.flic2lib.Platforms.1.0.4.nupkg"

if (Test-Path $mauiPackage) {
    Write-Host "✅ MAUI package found: $mauiPackage" -ForegroundColor Green
    $mauiSize = (Get-Item $mauiPackage).Length
    Write-Host "   Size: $($mauiSize / 1KB) KB" -ForegroundColor Gray
}
else {
    Write-Host "❌ MAUI package NOT found: $mauiPackage" -ForegroundColor Red
}

if (Test-Path $platformsPackage) {
    Write-Host "✅ Platforms package found: $platformsPackage" -ForegroundColor Green
    $platformsSize = (Get-Item $platformsPackage).Length
    Write-Host "   Size: $($platformsSize / 1KB) KB" -ForegroundColor Gray
}
else {
    Write-Host "❌ Platforms package NOT found: $platformsPackage" -ForegroundColor Red
}

# Extract and verify package contents
Write-Host "`n=== Package Content Verification ===" -ForegroundColor Green

# Create temp directories
$tempDir = "package_verification_temp"
if (Test-Path $tempDir) {
    Remove-Item $tempDir -Recurse -Force
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

try {
    # Extract MAUI package
    $mauiExtractPath = Join-Path $tempDir "maui"
    Expand-Archive $mauiPackage -DestinationPath $mauiExtractPath
    
    Write-Host "✅ MAUI package extracted successfully" -ForegroundColor Green
    
    # Check key files
    $mauiLibPath = Join-Path $mauiExtractPath "lib"
    if (Test-Path $mauiLibPath) {
        $androidDll = Get-ChildItem -Path $mauiLibPath -Filter "*android*" -Recurse | Where-Object { $_.Extension -eq ".dll" }
        $iosDll = Get-ChildItem -Path $mauiLibPath -Filter "*ios*" -Recurse | Where-Object { $_.Extension -eq ".dll" }
        
        if ($androidDll) {
            Write-Host "   ✅ Android DLL found: $($androidDll.Name)" -ForegroundColor Green
        }
        if ($iosDll) {
            Write-Host "   ✅ iOS DLL found: $($iosDll.Name)" -ForegroundColor Green
        }
    }
    
    # Extract Platforms package
    $platformsExtractPath = Join-Path $tempDir "platforms"
    Expand-Archive $platformsPackage -DestinationPath $platformsExtractPath
    
    Write-Host "✅ Platforms package extracted successfully" -ForegroundColor Green
    
    # Check key files
    $platformsLibPath = Join-Path $platformsExtractPath "lib"
    if (Test-Path $platformsLibPath) {
        $androidAAR = Get-ChildItem -Path $platformsLibPath -Filter "*.aar" -Recurse
        $xcframework = Get-ChildItem -Path $platformsLibPath -Filter "*xcframework*" -Recurse -Directory
        
        if ($androidAAR) {
            Write-Host "   ✅ Android AAR found: $($androidAAR.Name)" -ForegroundColor Green
        }
        if ($xcframework) {
            Write-Host "   ✅ iOS XCFramework found: $($xcframework.Count) directories" -ForegroundColor Green
        }
    }
    
}
catch {
    Write-Host "❌ Error during package extraction: $($_.Exception.Message)" -ForegroundColor Red
}
finally {
    # Clean up temp directory
    if (Test-Path $tempDir) {
        Remove-Item $tempDir -Recurse -Force
    }
}

# Verify package metadata
Write-Host "`n=== Package Metadata Verification ===" -ForegroundColor Green

try {
    # Read nuspec files
    [xml]$mauiNuspec = Get-Content "flic2lib.Maui.nuspec"
    [xml]$platformsNuspec = Get-Content "flic2lib.Platforms.fixed.nuspec"
    
    $mauiVersion = $mauiNuspec.package.metadata.version
    $platformsVersion = $platformsNuspec.package.metadata.version
    
    Write-Host "✅ MAUI package version: $mauiVersion" -ForegroundColor Green
    Write-Host "✅ Platforms package version: $platformsVersion" -ForegroundColor Green
    
    # Check if versions match
    if ($mauiVersion -eq $platformsVersion) {
        Write-Host "✅ Package versions are consistent" -ForegroundColor Green
    }
    else {
        Write-Host "⚠️ Package versions don't match!" -ForegroundColor Yellow
    }
    
    # Check dependencies
    $mauiDeps = $mauiNuspec.package.metadata.dependencies.group.dependency
    $platformsDep = $mauiDeps | Where-Object { $_.id -eq "walnut.flic2lib.Platforms" }
    
    if ($platformsDep -and $platformsDep.version -eq $platformsVersion) {
        Write-Host "✅ Dependency versions are correct" -ForegroundColor Green
    }
    else {
        Write-Host "⚠️ Dependency version mismatch!" -ForegroundColor Yellow
    }
    
}
catch {
    Write-Host "❌ Error reading package metadata: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== Package Generation Complete ===" -ForegroundColor Green
Write-Host "📦 Ready to publish:" -ForegroundColor Cyan
Write-Host "   • $mauiPackage" -ForegroundColor Gray
Write-Host "   • $platformsPackage" -ForegroundColor Gray
Write-Host "`n🚀 Next steps:" -ForegroundColor Cyan
Write-Host "   1. Test packages in a new MAUI project" -ForegroundColor Gray
Write-Host "   2. Publish to NuGet.org or private feed" -ForegroundColor Gray
Write-Host "   3. Update documentation and release notes" -ForegroundColor Gray