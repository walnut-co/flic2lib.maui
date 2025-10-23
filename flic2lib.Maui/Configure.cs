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
    /// Android background service methods have been removed from the package to prevent Java callable wrapper build errors.
    /// 
    /// To enable background Flic button events on Android:
    /// 1. Use AndroidBackgroundServiceHelper.GetBackgroundServiceTemplate() to get template code
    /// 2. Implement the background service in your main application project
    /// 3. Add required permissions to AndroidManifest.xml
    /// 
    /// See docs/android_background_solution.md for complete implementation guide.
    /// </summary>
    /// <returns>Instructions for implementing Android background services</returns>
    public static string GetAndroidBackgroundInstructions()
    {
        return Helpers.AndroidBackgroundServiceHelper.GetImplementationInstructions();
    }

    /// <summary>
    /// Checks if implementing a background service is recommended for this Android device.
    /// </summary>
    public static bool IsBackgroundServiceRecommended()
    {
        return Helpers.AndroidBackgroundServiceHelper.IsBackgroundServiceRecommended();
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
