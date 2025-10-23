namespace flic2lib.Maui;

public static class Configure
{
    public static MauiAppBuilder UseFlic2lib(this MauiAppBuilder builder, bool initManager = false)
    {
        builder.Services
           .AddSingleton<IFlicManager>(FlicManager.Instance)
           .AddSingleton<IFlicButtonHandler>(FlicButtonHandler.Instance);

        if (initManager)
        {
            FlicManager.Init();
        }

        return builder;
    }

#if ANDROID
    /// <summary>
    /// Enables background connectivity for Flic2 buttons on Android.
    /// This keeps Flic buttons connected and responsive even when your app is in the background.
    /// Call this from your MainActivity.OnCreate() method.
    /// </summary>
    /// <param name="context">The Android context (typically MainActivity)</param>
    public static void EnableFlic2BackgroundConnectivity(Android.Content.Context context)
    {
        Platforms.Android.Flic2BackgroundManager.StartBackgroundService(context);
    }

    /// <summary>
    /// Alias for EnableFlic2BackgroundConnectivity for shorter method name.
    /// Keeps Flic2 buttons connected when app is backgrounded.
    /// </summary>
    /// <param name="context">The Android context (typically MainActivity)</param>
    public static void KeepFlicButtonsConnected(Android.Content.Context context)
    {
        Platforms.Android.Flic2BackgroundManager.StartBackgroundService(context);
    }
#endif

#if IOS
    /// <summary>
    /// Enables background connectivity for Flic2 buttons on iOS.
    /// This configures background app refresh and maintains Flic buttons connected when possible.
    /// Call this from your AppDelegate.FinishedLaunching() method.
    /// </summary>
    public static void EnableFlic2BackgroundConnectivity()
    {
        Platforms.iOS.Flic2BackgroundManager.EnableBackgroundConnectivity();
    }

    /// <summary>
    /// Alias for EnableFlic2BackgroundConnectivity for shorter method name.
    /// Keeps Flic2 buttons connected when app is backgrounded.
    /// </summary>
    public static void KeepFlicButtonsConnected()
    {
        Platforms.iOS.Flic2BackgroundManager.EnableBackgroundConnectivity();
    }
#endif
}
