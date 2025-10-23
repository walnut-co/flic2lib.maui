#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
#endif

namespace flic2lib.Maui.Helpers;

/// <summary>
/// Helper class to provide template code for implementing Android background services
/// in your main application project. This avoids Java callable wrapper build issues
/// that occur when these components are in a NuGet package.
/// </summary>
public static class AndroidBackgroundServiceHelper
{
    /// <summary>
    /// Template method showing how to create a proper foreground service for Flic buttons.
    /// Copy this implementation to your main project's Platforms/Android folder.
    /// </summary>
    public static string GetBackgroundServiceTemplate()
    {
        return @"
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using flic2lib.Maui;

[Service(Enabled = true, Exported = false, ForegroundServiceType = Android.Content.PM.ForegroundService.TypeConnectedDevice)]
public class FlicBackgroundService : Service
{
    private const int NOTIFICATION_ID = 1001;
    private const string CHANNEL_ID = ""flic_background_channel"";

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
            var channel = new NotificationChannel(CHANNEL_ID, ""Flic Background Service"", NotificationImportance.Low)
            {
                Description = ""Keeps Flic buttons connected""
            };
            
            var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            notificationManager?.CreateNotificationChannel(channel);
        }
    }

    private Notification CreateNotification()
    {
        var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle(""Flic Buttons Connected"")
            .SetContentText(""Your Flic buttons are ready to use"")
            .SetSmallIcon(Resource.Drawable.notification_icon) // Add your icon
            .SetOngoing(true)
            .SetPriority(NotificationCompat.PriorityLow)
            .Build();

        return notification;
    }
}";
    }

    /// <summary>
    /// Template method showing how to create a boot receiver for Flic buttons.
    /// Copy this implementation to your main project's Platforms/Android folder.
    /// </summary>
    public static string GetBootReceiverTemplate()
    {
        return @"
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
}";
    }

    /// <summary>
    /// Gets the required AndroidManifest.xml entries for background service support.
    /// </summary>
    public static string GetManifestEntries()
    {
        return @"
<!-- Add to your main project's Platforms/Android/AndroidManifest.xml -->
<application>
    <!-- Your background service -->
    <service android:name="".FlicBackgroundService""
             android:enabled=""true""
             android:exported=""false""
             android:foregroundServiceType=""connectedDevice"" />

    <!-- Boot receiver -->
    <receiver android:name="".FlicBootReceiver""
              android:enabled=""true""
              android:exported=""true"">
        <intent-filter android:priority=""1000"">
            <action android:name=""android.intent.action.BOOT_COMPLETED"" />
            <action android:name=""android.intent.action.MY_PACKAGE_REPLACED"" />
            <category android:name=""android.intent.category.DEFAULT"" />
        </intent-filter>
    </receiver>
</application>

<!-- Required permissions -->
<uses-permission android:name=""android.permission.FOREGROUND_SERVICE"" />
<uses-permission android:name=""android.permission.FOREGROUND_SERVICE_CONNECTED_DEVICE"" />
<uses-permission android:name=""android.permission.RECEIVE_BOOT_COMPLETED"" />
<uses-permission android:name=""android.permission.WAKE_LOCK"" />";
    }

    /// <summary>
    /// Gets the MainActivity code needed to start the background service.
    /// </summary>
    public static string GetMainActivityCode()
    {
        return @"
// Add to your MainActivity.cs OnCreate method
protected override void OnCreate(Bundle? savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    
    // Start background service for Flic buttons
    var serviceIntent = new Intent(this, typeof(FlicBackgroundService));
    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        StartForegroundService(serviceIntent);
    else
        StartService(serviceIntent);
}";
    }

    /// <summary>
    /// Checks if background service implementation is recommended for this device.
    /// </summary>
    public static bool IsBackgroundServiceRecommended()
    {
#if ANDROID
        // Recommend background service for devices that support it well
        return Build.VERSION.SdkInt >= BuildVersionCodes.O; // Android 8.0+
#else
        return false; // Not Android
#endif
    }

    /// <summary>
    /// Gets user-friendly instructions for implementing background services.
    /// </summary>
    public static string GetImplementationInstructions()
    {
        return @"
To enable Flic button events when your app is in the background:

1. Copy the background service template to your main project
2. Copy the boot receiver template to your main project  
3. Add manifest entries to AndroidManifest.xml
4. Add service startup code to MainActivity
5. Add a notification icon resource

This implementation avoids build errors by keeping Android-specific 
components in your main application project rather than the NuGet package.

For complete code examples, see the documentation at:
https://github.com/walnut-co/flic2lib.maui/blob/main/docs/android_background_solution.md";
    }
}