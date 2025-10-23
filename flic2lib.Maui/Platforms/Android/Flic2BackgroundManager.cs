using Android.Content;
using Android.OS;

namespace flic2lib.Maui.Platforms.Android;

/// <summary>
/// Helper class to manage Flic2 background execution on Android.
/// Provides easy-to-use methods for starting and stopping background services
/// that keep Flic2 buttons connected when the app is not in the foreground.
/// </summary>
public static class Flic2BackgroundManager
{
    /// <summary>
    /// Starts the Flic2 background service to maintain button connectivity.
    /// This should be called from your MainActivity.OnCreate() or similar lifecycle method.
    /// </summary>
    /// <param name="context">Android context (typically MainActivity)</param>
    public static void StartBackgroundService(Context context)
    {
        if (context == null) return;

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

    /// <summary>
    /// Stops the Flic2 background service.
    /// Note: This will stop background button connectivity.
    /// </summary>
    /// <param name="context">Android context</param>
    public static void StopBackgroundService(Context context)
    {
        if (context == null) return;

        var serviceIntent = new Intent(context, typeof(Flic2BackgroundService));
        context.StopService(serviceIntent);
    }

    /// <summary>
    /// Checks if the Flic2 background service is required for this Android version.
    /// Background services are generally required for Android 8.0+ due to background execution limits.
    /// </summary>
    /// <returns>True if background service is recommended for reliable operation</returns>
    public static bool IsBackgroundServiceRecommended()
    {
        return Build.VERSION.SdkInt >= BuildVersionCodes.O;
    }
}