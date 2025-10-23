using Android.App;
using Android.Content;
using Android.OS;

namespace flic2lib.Maui.Platforms.Android;

/// <summary>
/// BroadcastReceiver that automatically starts the Flic2 background service when the device boots
/// or when the app package is updated. This ensures Flic2 buttons remain connected across
/// device restarts and app updates.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionMyPackageReplaced })]
public class Flic2BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent?.Action == null)
            return;

        // Check if this is a boot completed or package replaced event
        if (intent.Action == Intent.ActionBootCompleted ||
            intent.Action == Intent.ActionMyPackageReplaced)
        {
            // Start the Flic2 background service
            var serviceIntent = new Intent(context, typeof(Flic2BackgroundService));

            // Use StartForegroundService for Android 8.0+ (API 26+), StartService for older versions
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                context.StartForegroundService(serviceIntent);
            }
            else
            {
                context.StartService(serviceIntent);
            }
        }
    }
}