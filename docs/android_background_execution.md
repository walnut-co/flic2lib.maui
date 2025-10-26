# Background Execution Implementation for Flic2 Android

This document explains how to implement background execution support for Flic2 buttons in your MAUI Android app, ensuring that Flic2 buttons remain connected and functional even when the app is not in the foreground.

## Overview

To keep Flic2 buttons working in the background on Android, you need to implement background services in your main application project. The Flic2 MAUI library provides **helper templates and guidance** to make this implementation easier, but the actual service components must be in your app project to avoid Java callable wrapper build issues.

The implementation consists of three main components that you'll add to your project:

1. **FlicBackgroundService** - Foreground service that keeps Flic connections alive
2. **FlicBootReceiver** - Automatically restarts the service after device boot  
3. **Helper code in MainActivity** - Starts the background service

## Why Components Must Be in Your Project

Due to Android's Java callable wrapper requirements, background service components cannot be distributed in NuGet packages. They must be compiled as part of your main application. The library provides helper templates and documentation to make implementation straightforward.

## Implementation Steps

### 1. Add Background Service to Your Project

Create a new file `Platforms/Android/FlicBackgroundService.cs` in your MAUI project:

```csharp
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using flic2lib.Maui;

namespace YourApp.Platforms.Android;

[Service(Enabled = true, Exported = false, ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeConnectedDevice)]
public class FlicBackgroundService : Service
{
    private const int NOTIFICATION_ID = 1001;
    private const string CHANNEL_ID = "flic_background_channel";

    public override IBinder? OnBind(Intent? intent) => null;

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        CreateNotificationChannel();
        StartForeground(NOTIFICATION_ID, CreateNotification());

        // Initialize Flic manager for background operation
        FlicManager.Init();

        return StartCommandResult.Sticky;
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(CHANNEL_ID, "Flic Background Service", NotificationImportance.Low)
            {
                Description = "Keeps Flic buttons connected when app is in background"
            };

            var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            notificationManager?.CreateNotificationChannel(channel);
        }
    }

    private Notification CreateNotification()
    {
        var intent = new Intent(this, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.SingleTop);
        var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle("Flic Buttons Connected")
            .SetContentText("Your Flic buttons are ready to use")
            .SetSmallIcon(Resource.Drawable.notification_icon) // Add your notification icon
            .SetOngoing(true)
            .SetPriority(NotificationCompat.PriorityLow)
            .SetContentIntent(pendingIntent)
            .Build();

        return notification;
    }
}
```

### 2. Add Boot Receiver to Your Project

Create a new file `Platforms/Android/FlicBootReceiver.cs` in your MAUI project:

```csharp
using Android.App;
using Android.Content;
using Android.OS;

namespace YourApp.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionMyPackageReplaced })]
public class FlicBootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent?.Action == null) return;

        if (intent.Action == Intent.ActionBootCompleted || 
            intent.Action == Intent.ActionMyPackageReplaced)
        {
            var serviceIntent = new Intent(context, typeof(FlicBackgroundService));
            
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                context.StartForegroundService(serviceIntent);
            else
                context.StartService(serviceIntent);
        }
    }
}
```

### 3. Update AndroidManifest.xml

Add the following to your `Platforms/Android/AndroidManifest.xml`:

```xml
<application android:allowBackup="true" android:icon="@mipmap/appicon"
    android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true">
    
    <!-- Your Flic Background Service -->
    <service android:name=".FlicBackgroundService"
        android:enabled="true"
        android:exported="false"
        android:foregroundServiceType="connectedDevice" />

    <!-- Your Flic Boot Receiver -->
    <receiver android:name=".FlicBootReceiver"
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

Add this to your `MainActivity.cs`:

```csharp
using Microsoft.Extensions.Logging;

namespace YourApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, 
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | 
              ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | 
              ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Start the background service for handling Flic button events
            var serviceIntent = new Intent(this, typeof(FlicBackgroundService));
            StartForegroundService(serviceIntent);
        }
    }
}
```

## How the Service Works

The background service maintains Bluetooth connections to your Flic buttons and handles button events even when your app is not in the foreground. The service:

- Runs as an Android foreground service with persistent notification
- Maintains Bluetooth connections to paired Flic buttons  
- Processes button events (click, double-click, hold) in the background
- Can trigger actions, send notifications, or wake your app
- Automatically restarts after device boot or app updates

## Template Files Provided

The library includes helper templates and documentation to guide your implementation:

### AndroidBackgroundServiceHelper.cs (Template Helper)
Located in `flic2lib.Maui/Helpers/AndroidBackgroundServiceHelper.cs`, this helper provides:
- Guidance on implementing Android foreground services
- Template methods for notification handling
- Documentation on Android background execution requirements

### Sample Implementation  
The `flic2lib.Maui.Sample` project contains a complete working example:
- `FlicBackgroundService.cs` - Full service implementation
- `FlicBootReceiver.cs` - Boot receiver implementation  
- Proper AndroidManifest.xml configuration
- Integration with MainActivity

## How It Works

### Your Implementation Approach
1. Copy template files from the sample project to your app
2. Customize the service to handle your specific button events
3. Add service and receiver declarations to your AndroidManifest.xml
4. Start the service from your MainActivity
5. Handle button events according to your app's needs

### Background Operation
1. Your foreground service keeps the app process alive when needed
2. Persistent notification informs user that Flic buttons are connected
3. FlicManager continues to handle button events in the background  
4. Your service processes events and triggers appropriate actions
5. Service automatically restarts if killed by the system

### After Device Boot
1. Your `FlicBootReceiver` receives `BOOT_COMPLETED` broadcast
2. Receiver starts your background service automatically
3. Service reinitializes FlicManager and reconnects to buttons
4. Background operation resumes automatically

To handle button events in your background service, modify the `OnFlicButtonEvent` method in your `FlicBackgroundService.cs`:

```csharp
private void OnFlicButtonEvent(FlicButton button, FlicButtonEvent buttonEvent)
{
    // Handle different button actions
    switch (buttonEvent)
    {
        case FlicButtonEvent.Click:
            // Single click action
            ShowNotification($"Button {button.Name} clicked!");
            break;
            
        case FlicButtonEvent.DoubleClick:
            // Double click action - launch app to specific page
            LaunchAppToPage("MainPage");
            break;
            
        case FlicButtonEvent.Hold:
            // Long press action
            TriggerCustomAction(button);
            break;
    }
}

private void ShowNotification(string message)
{
    var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
        .SetContentTitle("Flic Button")
        .SetContentText(message)
        .SetSmallIcon(Resource.Drawable.notification_icon)
        .SetAutoCancel(true)
        .Build();

    var notificationManager = NotificationManagerCompat.From(this);
    notificationManager.Notify(Random.Shared.Next(), notification);
}

private void LaunchAppToPage(string pageName)
{
    var intent = new Intent(this, typeof(MainActivity));
    intent.PutExtra("page", pageName);
    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
    StartActivity(intent);
}
```

## Android Version Compatibility

Your implementation should handle different Android versions:
- **Android 8.0+ (API 26+)**: Use `StartForegroundService()` for proper foreground service handling
- **Android 7.1 and below (API 25-)**: Fall back to `StartService()` for compatibility
- **All versions**: Include proper API level checks to prevent crashes

Example in your MainActivity:
```csharp
var serviceIntent = new Intent(this, typeof(FlicBackgroundService));
if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
    StartForegroundService(serviceIntent);
else
    StartService(serviceIntent);
```

## Troubleshooting

### Common Issues

1. **Service not starting**: 
   - Ensure all permissions are declared in AndroidManifest.xml
   - Check that service is properly declared with correct class name
   - Verify device isn't in aggressive battery optimization mode

2. **Buttons not responding**: 
   - Check that Bluetooth is enabled and buttons are paired through your main app first
   - Verify FlicManager is properly initialized in the service
   - Check device logs for Bluetooth-related errors

3. **Service stops unexpectedly**: 
   - Make sure notification channel is created before starting foreground service
   - Use `StartCommandResult.Sticky` to automatically restart if killed
   - Handle service lifecycle properly (OnStartCommand, OnDestroy)

4. **Boot receiver not working**: 
   - Ensure `RECEIVE_BOOT_COMPLETED` permission is granted
   - Check that receiver is properly declared in manifest  
   - Verify device allows apps to start at boot (some manufacturers disable this)

### Required Permissions

Ensure these permissions are in your AndroidManifest.xml:

```xml
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_CONNECTED_DEVICE" />
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
<uses-permission android:name="android.permission.WAKE_LOCK" />
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

## Best Practices

1. **Test thoroughly**: Test your background service on different Android versions and manufacturers
2. **Handle errors gracefully**: Include proper error handling in your service implementation
3. **Minimize battery usage**: Only keep the service running when Flic buttons are actively needed
4. **User communication**: Use clear notifications to inform users when background services are active
5. **Regular updates**: Keep your implementation up to date with Android best practices and library updates

## Additional Resources

- Sample implementation: `flic2lib.Maui.Sample` project
- Android foreground services: [Android Developer Documentation](https://developer.android.com/guide/components/foreground-services)
- Background execution limits: [Android Background Execution Limits](https://developer.android.com/about/versions/oreo/background)
- Cross-platform background guide: [Cross Platform Background Execution](cross_platform_background_execution.md)

1. **Call KeepFlicButtonsConnected early**: Call it in MainActivity.OnCreate() for immediate background support
2. **Trust the library**: The library handles all Android version compatibility automatically
3. **Keep manifest updated**: Always include the required permissions and service declarations
4. **Monitor library updates**: Background execution improvements come with library updates
5. **User transparency**: The persistent notification keeps users informed automatically

This library-level implementation ensures reliable background operation while following Android's best practices and providing the easiest possible integration for developers.