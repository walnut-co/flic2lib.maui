# iOS Background Execution Implementation for Flic2

This document explains the built-in background execution support in the Flic2 MAUI library for iOS, designed to maintain Flic2 button connectivity when the app is backgrounded.

## Overview

The Flic2 MAUI library includes **iOS background execution support** that works within Apple's strict background execution limitations. The implementation provides the best possible background connectivity while respecting iOS system constraints.

⚠️ **Important iOS Limitations**: iOS has strict background execution policies. Unlike Android, iOS apps cannot run indefinitely in the background. The library maximizes available background time but cannot guarantee continuous connectivity.

## Quick Start

### 1. Update Info.plist

Add the following background modes to your `Platforms/iOS/Info.plist`:

```xml
<key>UIBackgroundModes</key>
<array>
    <string>bluetooth-central</string>
    <string>background-processing</string>
    <string>background-fetch</string>
</array>
```

### 2. Enable Background Execution in AppDelegate

In your `AppDelegate.cs`, add:

```csharp
using Foundation;
using flic2lib.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var result = base.FinishedLaunching(application, launchOptions);
        
        // Keep Flic2 buttons connected when app is in background
        Configure.KeepFlicButtonsConnected();
        
        return result;
    }
}
```

### 3. User Must Enable Background App Refresh

Users must enable "Background App Refresh" for your app in iOS Settings:
- Settings → General → Background App Refresh → [Your App] → ON

## Library Components

### Flic2BackgroundManager.cs (Library)

Located in `flic2lib.Maui.Platforms.iOS.Flic2BackgroundManager`, this manager:
- Registers for background/foreground notifications
- Requests background execution time when app is backgrounded
- Maintains FlicManager state during transitions
- Provides status checking methods

### Configure.cs (Library)

The main library configuration class includes iOS methods:
- `KeepFlicButtonsConnected()` - Simplified method for iOS background connectivity
- `EnableFlic2BackgroundConnectivity()` - Descriptive method for background connectivity

## How It Works

### Application Launch
1. Call `Configure.KeepFlicButtonsConnected()` in AppDelegate.FinishedLaunching
2. Library registers for background/foreground notifications
3. FlicManager is initialized and maintains connections

### App Backgrounded
1. System notifies app it's entering background
2. Library requests background execution time (limited by iOS)
3. Background task maintains FlicManager for available time
4. Background time typically 30 seconds to a few minutes

### App Foregrounded
1. System notifies app it's entering foreground
2. Library ensures FlicManager is ready for full operation
3. Bluetooth connections are re-established if needed

## iOS Background Execution Limitations

### ⏱️ **Time Limits**
- **Standard Background Time**: 30 seconds to 3 minutes
- **Background App Refresh**: Periodic system-controlled wake-ups
- **Bluetooth Central**: Extended time for active Bluetooth operations

### 🔋 **Battery Considerations**
- iOS prioritizes battery life over background execution
- Heavy background usage may reduce future background time allocation
- System may terminate background execution for low battery

### 👤 **User Control**
- Users can disable Background App Refresh globally or per-app
- Users can force-quit apps, immediately terminating background execution
- iOS may limit background execution based on app usage patterns

## Background Modes Explained

### `bluetooth-central`
- **Purpose**: Maintains Bluetooth connections in background
- **Duration**: Extended background time for active Bluetooth operations
- **Requirement**: Essential for Flic2 button connectivity

### `background-processing`
- **Purpose**: Allows app to complete tasks when backgrounded
- **Duration**: Limited time (usually 30 seconds to a few minutes)
- **Requirement**: Helps maintain FlicManager state

### `background-fetch`
- **Purpose**: Periodic wake-ups to refresh app content
- **Duration**: System-controlled, typically 30 seconds
- **Requirement**: Allows periodic Flic connection maintenance

## Best Practices for iOS

### ✅ **For Library Users**
1. **Set User Expectations**: Inform users that iOS background connectivity is limited
2. **Encourage Background App Refresh**: Guide users to enable it in Settings
3. **Design for Foreground Use**: Primary Flic functionality should work when app is active
4. **Handle Reconnection**: Expect buttons may need to reconnect when app becomes active

### ✅ **Implementation Guidelines**
1. **Call Early**: Enable background execution in AppDelegate.FinishedLaunching
2. **Check Status**: Use `IsBackgroundAppRefreshEnabled()` to inform users
3. **Graceful Degradation**: App should work even if background execution fails
4. **Battery Conscious**: Don't perform unnecessary work in background

## User Experience

### Settings Required
Users must manually enable Background App Refresh:
```
iOS Settings → General → Background App Refresh → [Your App] → ON
```

### Expected Behavior
- **Foreground**: Full Flic2 button functionality
- **Background**: Limited-time connectivity (30 seconds to few minutes)
- **Long Background**: Buttons may disconnect, will reconnect when app becomes active
- **Force Quit**: All background execution stops immediately

## Troubleshooting

### Buttons Disconnect Quickly
- Verify Background App Refresh is enabled for your app
- Check battery level (iOS limits background execution on low battery)
- Ensure all required background modes are in Info.plist
- Consider that iOS intentionally limits background time

### Background App Refresh Disabled
```csharp
var status = Flic2BackgroundManager.GetBackgroundStatus();
// Show user-friendly message guiding to Settings
```

### Buttons Not Reconnecting
- FlicManager reinitializes when app becomes active
- Bluetooth may need time to re-establish connections
- Check iOS Bluetooth permissions are granted

## Migration from Sample Implementation

If you previously implemented iOS background execution using sample code:

1. **Remove old files**: Delete any custom background handling from your project
2. **Update AppDelegate**: Replace manual background code with `Configure.KeepFlicButtonsConnected()`
3. **Update Info.plist**: Ensure all required background modes are present
4. **Test thoroughly**: Verify background execution works within iOS limitations

## Comparison: iOS vs Android Background Execution

| Aspect | iOS Implementation | Android Implementation |
|--------|-------------------|----------------------|
| **Background Duration** | Limited (30s-3min) | Indefinite (foreground service) |
| **User Control** | Background App Refresh setting | Can disable service notification |
| **System Termination** | Common (battery/performance) | Rare (foreground service protection) |
| **Connectivity Guarantee** | Limited | Strong (with proper implementation) |
| **Battery Impact** | System-managed | Visible to user via notification |
| **Setup Complexity** | Medium (user education needed) | Low (automatic) |

## Code Example: Checking Background Status

```csharp
public void CheckBackgroundCapabilities()
{
    var isEnabled = Flic2BackgroundManager.IsBackgroundAppRefreshEnabled();
    var status = Flic2BackgroundManager.GetBackgroundStatus();
    
    if (!isEnabled)
    {
        // Show user guidance to enable Background App Refresh
        await DisplayAlert("Background App Refresh", 
            "To keep Flic buttons connected in background, enable Background App Refresh in Settings → General → Background App Refresh → [Your App]", 
            "OK");
    }
}
```

## Development Without Mac

Since iOS development typically requires a Mac, consider these approaches:

### 🔄 **Cross-Platform Design**
- Design iOS implementation to match Android patterns
- Use same method names and configuration approach
- Create comprehensive documentation and code samples

### ☁️ **Cloud Build Services**
- **Visual Studio App Center**: Build iOS apps without Mac
- **Azure DevOps**: iOS build agents available
- **GitHub Actions**: macOS runners for iOS builds

### 🤝 **Community/Partner Mac Access**
- Partner with iOS developers for testing
- Use Mac rental services for short-term access
- Community testing through TestFlight

### 📋 **Code Review Approach**
- Implement based on iOS documentation and best practices
- Have iOS developers review code before testing
- Use static analysis tools that work cross-platform

This iOS implementation provides the best possible background connectivity within Apple's strict limitations while maintaining the same easy-to-use API as the Android version.