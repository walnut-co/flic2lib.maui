# NuGet Deployment Script for version 1.0.6
# Replace YOUR_API_KEY_HERE with your actual NuGet.org API key

$apiKey = "YOUR_API_KEY_HERE"
$source = "https://api.nuget.org/v3/index.json"

Write-Host "🚀 Starting NuGet Package Deployment v1.0.6..." -ForegroundColor Green

# Deploy Platforms package first (dependency)
Write-Host "📦 Deploying Platforms package v1.0.6..." -ForegroundColor Yellow
dotnet nuget push walnut.flic2lib.Platforms.1.0.6.nupkg --source $source

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Platforms package v1.0.6 deployed successfully!" -ForegroundColor Green
    
    # Wait a moment for package to be available
    Write-Host "⏳ Waiting 30 seconds for package indexing..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    # Deploy MAUI package
    Write-Host "📦 Deploying MAUI package v1.0.6..." -ForegroundColor Yellow
    dotnet nuget push walnut.flic2lib.Maui.1.0.6.nupkg --source $source
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ MAUI package v1.0.6 deployed successfully!" -ForegroundColor Green
        Write-Host "🎉 All packages v1.0.6 deployed to NuGet.org!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📋 Package URLs:" -ForegroundColor Cyan
        Write-Host "   Platforms: https://www.nuget.org/packages/walnut.flic2lib.Platforms/1.0.6" -ForegroundColor Gray
        Write-Host "   MAUI: https://www.nuget.org/packages/walnut.flic2lib.Maui/1.0.6" -ForegroundColor Gray
        Write-Host ""
        Write-Host "🔄 Version 1.0.6 Changes:" -ForegroundColor Cyan
        Write-Host "   ✅ Updated minimum Android version to API 29 (Android 10)" -ForegroundColor Gray
        Write-Host "   ✅ Updated minimum iOS version to 13.0" -ForegroundColor Gray
        Write-Host "   ✅ Enhanced security and compatibility with modern platforms" -ForegroundColor Gray
        Write-Host "   ✅ Improved iOS AppDelegate with reliable service access" -ForegroundColor Gray
        Write-Host "   ✅ Optimized Android metadata transformations" -ForegroundColor Gray
    }
    else {
        Write-Host "❌ Failed to deploy MAUI package!" -ForegroundColor Red
    }
}
else {
    Write-Host "❌ Failed to deploy Platforms package!" -ForegroundColor Red
}

Write-Host ""
Write-Host "📝 Manual Deployment Commands:" -ForegroundColor Cyan
Write-Host "If you prefer to deploy manually, use these commands:" -ForegroundColor Gray
Write-Host ""
Write-Host "# 1. Deploy Platforms package first:" -ForegroundColor Yellow
Write-Host "dotnet nuget push walnut.flic2lib.Platforms.1.0.6.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor White
Write-Host ""
Write-Host "# 2. Wait a few minutes, then deploy MAUI package:" -ForegroundColor Yellow
Write-Host "dotnet nuget push walnut.flic2lib.Maui.1.0.6.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor White

Write-Host ""
Write-Host "📊 Platform Version Requirements:" -ForegroundColor Cyan
Write-Host "   📱 iOS: Minimum version 13.0 (improved from 14.0)" -ForegroundColor Green
Write-Host "   🤖 Android: Minimum API 29 / Android 10 (improved from API 21)" -ForegroundColor Green
Write-Host "   🔒 Enhanced security and modern platform compatibility" -ForegroundColor Green