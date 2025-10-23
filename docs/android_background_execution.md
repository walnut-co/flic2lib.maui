# Background Execution Implementation for Flic2 Android

This document explains the built-in background execution support in the Flic2 MAUI library, ensuring that Flic2 buttons remain connected and functional even when the app is not in the foreground.

## Overview

The Flic2 MAUI library now includes **built-in background execution support** for Android, eliminating the need for manual implementation in your app. The implementation consists of three main components that are automatically included in the library:

1. **Flic2BackgroundService** - Foreground service that keeps Flic connections alive
2. **Flic2BootReceiver** - Automatically restarts the service after device boot
3. **Flic2BackgroundManager** - Easy-to-use helper for starting background execution

## Quick Start

### 1. Update AndroidManifest.xml

Add the following to your `Platforms/Android/AndroidManifest.xml`:

```xml
<application android:allowBackup="true" android:icon="@mipmap/appicon"
    android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true">
    
    <!-- Flic2 Background Service (from library) -->
    <service android:name="flic2lib.Maui.Platforms.Android.Flic2BackgroundService"
        android:enabled="true"
        android:exported="false"
        android:foregroundServiceType="connectedDevice" />

    <!-- Flic2 Boot Receiver (from library) -->
    <receiver android:name="flic2lib.Maui.Platforms.Android.Flic2BootReceiver"
        android:enabled="true"
        android:exported="true">
        <intent-filter android:priority="1000">
            <action android:name="android.intent.action.BOOT_COMPLETED" />
            <action android:name="android.intent.action.MY_PACKAGE_REPLACED" />
            <category android:name="android.intent.category.DEFAULT" />
        </intent-filter>
    </receiver>
</application>

<!-- Required permissions for background execution -->
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_CONNECTED_DEVICE" />
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
<uses-permission android:name="android.permission.WAKE_LOCK" />
```

### 2. Enable Background Execution in MainActivity

In your `MainActivity.cs`, simply call:

```csharp
using flic2lib.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Keep Flic2 buttons connected when app is in background
        Configure.KeepFlicButtonsConnected(this);
        
        // Alternative method with longer descriptive name:
        // Configure.EnableFlic2BackgroundConnectivity(this);
    }
}
```

That's it! Your Flic2 buttons will now work in the background automatically.

## Library Components

### Flic2BackgroundService.cs (Library)

Located in `flic2lib.Maui.Platforms.Android.Flic2BackgroundService`, this service:
- Runs as a foreground service with a persistent notification
- Automatically initializes FlicManager when started
- Handles Android 8.0+ foreground service requirements
- Uses `StartCommandResult.Sticky` to automatically restart if killed

### Flic2BootReceiver.cs (Library)

Located in `flic2lib.Maui.Platforms.Android.Flic2BootReceiver`, this receiver:
- Listens for `BOOT_COMPLETED` and `MY_PACKAGE_REPLACED` events
- Automatically starts the background service after device boot
- Handles app updates by restarting the service
- Compatible with Android 8.0+ background execution limits

### Flic2BackgroundManager.cs (Library)

Located in `flic2lib.Maui.Platforms.Android.Flic2BackgroundManager`, this helper provides:
- `StartBackgroundService(Context)` - Start background execution
- `StopBackgroundService(Context)` - Stop background execution (optional)
- `IsBackgroundServiceRecommended()` - Check if background service is needed

### Configure.cs (Library)

The main library configuration class now includes:
- `KeepFlicButtonsConnected(Context)` - Simplified method to keep Flic buttons connected in background
- `EnableFlic2BackgroundConnectivity(Context)` - Descriptive method for background connectivity

## How It Works

### Application Startup
1. Call `Configure.KeepFlicButtonsConnected(this)` in MainActivity
2. Library starts `Flic2BackgroundService` automatically
3. Service creates notification channel and starts as foreground service
4. Service initializes FlicManager if not already initialized
5. FlicManager maintains connection to Flic2 buttons

### Background Operation
1. Foreground service keeps the app process alive
2. Persistent notification informs user that Flic buttons are connected
3. FlicManager continues to handle button events in the background
4. Service automatically restarts if killed by the system

### After Device Boot
1. `Flic2BootReceiver` receives `BOOT_COMPLETED` broadcast
2. Receiver starts `Flic2BackgroundService` automatically
3. Service reinitializes FlicManager and reconnects to buttons
4. Background operation resumes automatically

## Advantages of Library-Level Implementation

### ✅ **For Library Users**
- **Zero Implementation Required**: Just add manifest entries and one line of code
- **Automatic Updates**: Background execution improvements come with library updates
- **Consistent Behavior**: Same background execution across all apps using the library
- **Reduced Complexity**: No need to understand Android service implementation details

### ✅ **For Library Maintainers**
- **Centralized Logic**: All background execution code in one place
- **Easier Testing**: Can test background execution independently
- **Better Documentation**: Single source of truth for background execution
- **Future-Proof**: Can adapt to Android changes without requiring user code changes

### ✅ **Comparison with Sample Implementation**
| Aspect | Sample Project Implementation | Library Implementation |
|--------|------------------------------|----------------------|
| **Setup Complexity** | High - Copy multiple files | Low - Add manifest + 1 line |
| **Maintenance** | User maintains copies | Library team maintains |
| **Updates** | Manual copy from samples | Automatic with library updates |
| **Consistency** | Varies per implementation | Consistent across all users |
| **Testing** | Each user tests separately | Library team tests centrally |

## Android Version Compatibility

The implementation handles different Android versions automatically:
- **Android 8.0+ (API 26+)**: Uses `StartForegroundService()` for proper foreground service handling
- **Android 7.1 and below (API 25-)**: Falls back to `StartService()` for compatibility
- **All versions**: Proper API level checks prevent crashes on older devices

## User Experience

### Notification
- **Title**: "Flic2 Background Service"
- **Content**: "Keeping Flic buttons connected"
- **Priority**: Low (minimal user distraction)
- **Ongoing**: True (cannot be dismissed by user)
- **Tap Action**: Opens the main app

### Battery Optimization
- Service uses `NotificationImportance.Low` to minimize battery impact
- Foreground service type is `connectedDevice` for appropriate Android 10+ handling
- Service only runs when necessary (Flic buttons are in use)

## Troubleshooting

### Service Not Starting
- Check that all required permissions are granted in AndroidManifest.xml
- Verify service declaration uses full namespace: `flic2lib.Maui.Platforms.Android.Flic2BackgroundService`
- Ensure device isn't in aggressive battery optimization mode

### Buttons Disconnecting
- Verify `Configure.KeepFlicButtonsConnected(this)` is called in MainActivity.OnCreate()
- Check that foreground service notification is visible
- Check device logs for any Bluetooth-related errors

### Boot Receiver Not Working
- Ensure `RECEIVE_BOOT_COMPLETED` permission is granted
- Check that receiver is properly declared in manifest with full namespace
- Verify device allows apps to start at boot

## Migration from Sample Implementation

If you previously implemented background execution using sample code:

1. **Remove old files**: Delete any copied `Flic2BackgroundService.cs` and `Flic2BootReceiver.cs` from your project
2. **Update AndroidManifest.xml**: Change service/receiver names to use full library namespaces
3. **Simplify MainActivity**: Replace manual service starting code with `Configure.KeepFlicButtonsConnected(this)`
4. **Test thoroughly**: Verify background execution still works with the library implementation

## Best Practices

1. **Call KeepFlicButtonsConnected early**: Call it in MainActivity.OnCreate() for immediate background support
2. **Trust the library**: The library handles all Android version compatibility automatically
3. **Keep manifest updated**: Always include the required permissions and service declarations
4. **Monitor library updates**: Background execution improvements come with library updates
5. **User transparency**: The persistent notification keeps users informed automatically

This library-level implementation ensures reliable background operation while following Android's best practices and providing the easiest possible integration for developers.