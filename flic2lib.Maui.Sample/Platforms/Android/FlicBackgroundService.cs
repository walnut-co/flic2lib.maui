using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using flic2lib.Maui;
using flic2lib.Maui.Sample.Services;
using Microsoft.Extensions.DependencyInjection;

namespace flic2lib.Maui.Sample.Platforms.Android;

/// <summary>
/// Android foreground service that keeps Flic buttons connected when the app is in the background.
/// This implementation is in the main application project to avoid Java callable wrapper build errors
/// that occur when these components are in NuGet packages.
/// </summary>
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
        // This ensures buttons stay connected when app is backgrounded
        FlicManager.Init();

        // Start event handling for background button clicks
        // This ensures the FlicEventHandler receives events even when app is backgrounded
        EnsureEventHandlerInitialized();

        // Return Sticky to restart service if killed by system
        return StartCommandResult.Sticky;
    }

    private void EnsureEventHandlerInitialized()
    {
        try
        {
            // The FlicEventHandler is registered as a singleton in MauiProgram.cs
            // We need to get it from the service provider to ensure it's initialized
            // and listening for events even when the app is in the background

            // Get the MauiApp service provider through Platform.CurrentActivity
            if (Platform.CurrentActivity?.Application is MauiApplication mauiApp)
            {
                var services = mauiApp.Services;
                var flicEventHandler = services.GetService<FlicEventHandler>();

                if (flicEventHandler != null)
                {
                    System.Diagnostics.Debug.WriteLine("FlicEventHandler initialized for background service");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Warning: FlicEventHandler not found in service provider");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing FlicEventHandler: {ex.Message}");
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        // Service is being destroyed - cleanup if needed
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
        // Create intent to return to app when notification is tapped
        var intent = new Intent(this, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.SingleTop);
        var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle("Flic Buttons Connected")
            .SetContentText("Your Flic buttons are ready to use")
            .SetSmallIcon(Resource.Drawable.flic_notification) // We'll create this icon
            .SetOngoing(true) // Can't be dismissed by user
            .SetPriority(NotificationCompat.PriorityLow)
            .SetContentIntent(pendingIntent)
            .SetAutoCancel(false)
            .Build();

        return notification;
    }
}