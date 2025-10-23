using Android.App;
using Android.Content;
using Android.OS;

namespace flic2lib.Maui.Sample.Platforms.Android;

/// <summary>
/// Broadcast receiver that automatically starts the Flic background service when:
/// - Device is rebooted (ACTION_BOOT_COMPLETED)
/// - App is updated (ACTION_MY_PACKAGE_REPLACED)
/// 
/// This ensures Flic buttons remain connected across device restarts and app updates.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionMyPackageReplaced })]
public class FlicBootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent?.Action == null)
            return;

        // Check if this is a boot completed or package replaced event
        if (intent.Action == Intent.ActionBootCompleted ||
            intent.Action == Intent.ActionMyPackageReplaced)
        {
            StartFlicBackgroundService(context);
        }
    }

    private static void StartFlicBackgroundService(Context context)
    {
        var serviceIntent = new Intent(context, typeof(FlicBackgroundService));

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