# iOS Linking Fix for flic2lib.Platforms

## Issue
When consuming the `walnut.flic2lib.Platforms` NuGet package in an iOS app, you may encounter linking errors:

```
error : Undefined symbols for architecture arm64:
error :   "_OBJC_CLASS_$_FLICButton", referenced from:
error :        in registrar.o
error :   "_OBJC_CLASS_$_FLICManager", referenced from:
error :        in registrar.o
```

## Solution
Version 1.0.9+ includes automatic linking fixes through MSBuild props and targets files. The package will automatically:

1. Add required iOS framework references (`Foundation`, `CoreBluetooth`)
2. Include proper linker flags (`-ObjC`) to load Objective-C categories 
3. Force load the native flic2lib framework
4. Include the xcframework resources in the app bundle

## Manual Fix (if needed)
If you still encounter issues, you can manually add these properties to your iOS app project file:

```xml
<PropertyGroup Condition="'$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)'))' == 'ios'">
  <MtouchExtraArgs>$(MtouchExtraArgs) -gcc_flags "-framework Foundation -framework CoreBluetooth -ObjC"</MtouchExtraArgs>
</PropertyGroup>
```

## Troubleshooting
1. Clean and rebuild your project
2. Delete bin/obj folders
3. Ensure you're using .NET 9 iOS workload
4. Verify the package version is 1.0.9 or higher

The `-ObjC` linker flag is crucial as it ensures Objective-C categories and classes are properly loaded from the static library.