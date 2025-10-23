# walnut.flic2lib.Maui

[![NuGet](https://img.shields.io/nuget/v/walnut.flic2lib.Maui.svg)](https://www.nuget.org/packages/walnut.flic2lib.Maui/)
[![NuGet](https://img.shields.io/nuget/v/walnut.flic2lib.Platforms.svg)](https://www.nuget.org/packages/walnut.flic2lib.Platforms/)

.NET 9 MAUI bindings for Flic2 smart buttons with comprehensive background event handling support.

## Packages

This repository contains two NuGet packages:

### walnut.flic2lib.Platforms
Contains .NET 9 Android and iOS bindings for the Flic2 native SDKs.

- **Android**: Targets API 29+ (Android 10+)
- **iOS**: Targets iOS 13.0+
- Includes native libraries and framework files

### walnut.flic2lib.Maui
Contains .NET 9 MAUI implementation for cross-platform Flic2 button integration.

- Cross-platform API for Flic2 button management
- Background event handling support
- Android foreground service integration
- iOS background task management

## Installation

```xml
<PackageReference Include="walnut.flic2lib.Maui" Version="1.0.6" />
```

The `walnut.flic2lib.Platforms` package will be automatically included as a dependency.

## Native SDK References

This binding is based on the official Flic2 SDKs:

- [iOS SDK](https://github.com/50ButtonsEach/flic2lib-ios) → Flic2lib.iOS
- [Android SDK](https://github.com/50ButtonsEach/flic2lib-android) → Flic2lib.Android

## Documentation

- [MAUI Implementation Guide](docs/maui_readme.md)
- [Platform Bindings Guide](docs/platforms_readme.md)
- [Android Background Execution](docs/android_background_execution.md)
- [iOS Background Execution](docs/ios_background_execution.md)
- [Cross-Platform Background Guide](docs/cross_platform_background_execution.md)

## Development

### Building Locally

```bash
# Restore dependencies
dotnet restore flic2lib.maui.sln

# Build all projects
dotnet build flic2lib.maui.sln --configuration Release
```

### Creating Packages

Use the provided test script to create packages locally:

```powershell
.\test-deployment.ps1 -Version "1.0.7"
```

### Deployment

See [DEPLOYMENT_WORKFLOW.md](DEPLOYMENT_WORKFLOW.md) for information about automated NuGet deployment via GitHub Actions.

## Requirements

- .NET 9.0
- Visual Studio 2022 or JetBrains Rider
- Android SDK (API 29+)
- Xcode (for iOS development)

## License

MIT License - see individual SDK repositories for native library licensing.

