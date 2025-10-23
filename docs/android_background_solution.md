# Android Background Service Solution

## Problem
Version 1.0.4 removed Android background service components to fix Java callable wrapper build errors.

## Solution Options

### Option 1: User Implementation in Main Project
Users can implement background services directly in their main application projects (where Java callable wrapper issues don't occur).

#### Step 1: Create Background Service in Main Project
```csharp
// In your main MAUI project: Platforms/Android/FlicBackgroundService.cs
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using flic2lib.Maui;

[Service(Enabled = true, Exported = false, ForegroundServiceType = Android.Content.PM.ForegroundService.TypeConnectedDevice)]
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
                Description = "Keeps Flic buttons connected"
            };
            
            var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            notificationManager?.CreateNotificationChannel(channel);
        }
    }

    private Notification CreateNotification()
    {
        var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle("Flic Buttons Connected")
            .SetContentText("Your Flic buttons are ready to use")
            .SetSmallIcon(Resource.Drawable.notification_icon) // Add your icon
            .SetOngoing(true)
            .SetPriority(NotificationCompat.PriorityLow)
            .Build();

        return notification;
    }
}
```

#### Step 2: Create Boot Receiver in Main Project
```csharp
// In your main MAUI project: Platforms/Android/FlicBootReceiver.cs
using Android.App;
using Android.Content;
using Android.OS;

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

#### Step 3: Update AndroidManifest.xml
```xml
<!-- Add to your main project's Platforms/Android/AndroidManifest.xml -->
<application>
    <!-- Your background service -->
    <service android:name=".FlicBackgroundService"
             android:enabled="true"
             android:exported="false"
             android:foregroundServiceType="connectedDevice" />

    <!-- Boot receiver -->
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

<!-- Required permissions -->
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_CONNECTED_DEVICE" />
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
<uses-permission android:name="android.permission.WAKE_LOCK" />
```

#### Step 4: Start Service from MainActivity
```csharp
// In your MainActivity.cs
protected override void OnCreate(Bundle? savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    
    // Start background service for Flic buttons
    var serviceIntent = new Intent(this, typeof(FlicBackgroundService));
    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        StartForegroundService(serviceIntent);
    else
        StartService(serviceIntent);
}
```

### Option 2: Helper Methods in Package
We could add helper methods to the current package that make it easier for users to implement their own background services.

### Option 3: Separate Background Package
Create `walnut.flic2lib.Maui.BackgroundServices` as an optional add-on package that includes the background service implementations.

## Recommendation
**Option 1 (User Implementation)** is the most reliable approach because:
- No Java callable wrapper issues
- Full control over service implementation
- Can customize notifications and behavior
- Works with all Android versions