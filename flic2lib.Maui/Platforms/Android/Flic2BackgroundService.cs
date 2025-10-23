using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace flic2lib.Maui.Platforms.Android;

/// <summary>
/// Background service that keeps Flic2 buttons connected when the app is not in the foreground.
/// This service runs as a foreground service with a persistent notification to comply with Android's
/// background execution limitations and ensure reliable button connectivity.
/// </summary>
[Service(Enabled = true, Exported = false)]
public class Flic2BackgroundService : Service
{
    private const int ServiceNotificationId = 12345;
    private const string NotificationChannelId = "Flic2BackgroundChannel";
    private const string NotificationChannelName = "Flic2 Background Service";

    public override void OnCreate()
    {
        base.OnCreate();
        CreateNotificationChannel();
        StartAsForegroundService();

        // Initialize FlicManager if not already done
        if (!FlicManager.IsInitialized)
        {
            FlicManager.Init();
        }
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        // Ensure the service keeps running and automatically restarts if killed
        return StartCommandResult.Sticky;
    }

    public override IBinder? OnBind(Intent? intent)
    {
        // This service doesn't support binding
        return null;
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                NotificationChannelId,
                NotificationChannelName,
                NotificationImportance.Low)
            {
                Description = "Keeps Flic2 buttons connected in the background"
            };

            var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            notificationManager?.CreateNotificationChannel(channel);
        }
    }

    private void StartAsForegroundService()
    {
        // Create intent that opens the main activity when notification is tapped
        var packageName = PackageName;
        var launchIntent = PackageManager?.GetLaunchIntentForPackage(packageName!);

        var pendingIntent = launchIntent != null
            ? PendingIntent.GetActivity(
                this,
                0,
                launchIntent,
                PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable)
            : null;

        var notification = new NotificationCompat.Builder(this, NotificationChannelId)
            .SetContentTitle("Flic2 Background Service")
            .SetContentText("Keeping Flic buttons connected")
            .SetSmallIcon(17301575) // ic_menu_mylocation system icon
            .SetContentIntent(pendingIntent)
            .SetOngoing(true)
            .SetPriority(NotificationCompat.PriorityLow)
            .SetCategory(NotificationCompat.CategoryService)
            .Build();

        StartForeground(ServiceNotificationId, notification);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        StopForeground(true);
    }
}