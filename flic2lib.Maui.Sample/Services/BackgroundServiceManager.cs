using flic2lib.Maui.Helpers;

namespace flic2lib.Maui.Sample.Services;

/// <summary>
/// Helper service that demonstrates how to manage Flic background connectivity
/// across both Android and iOS platforms.
/// </summary>
public class BackgroundServiceManager
{
    /// <summary>
    /// Gets the current status of background connectivity for Flic buttons.
    /// </summary>
    public string GetBackgroundStatus()
    {
#if ANDROID
        if (AndroidBackgroundServiceHelper.IsBackgroundServiceRecommended())
        {
            return "✅ Android: Background service enabled - unlimited connectivity\n" +
                   "Flic buttons will work even when app is completely backgrounded.";
        }
        else
        {
            return "⚠️ Android: Background service not recommended for this device\n" +
                   "Flic buttons will work only when app is in foreground.";
        }
#elif IOS
        return "✅ iOS: Background connectivity enabled (limited)\n" +
               "Flic buttons work for 30 seconds to 3 minutes in background.\n" +
               "Ensure Background App Refresh is enabled in iOS Settings.";
#else
        return "❌ Background connectivity not implemented for this platform";
#endif
    }

    /// <summary>
    /// Gets platform-specific instructions for optimizing background connectivity.
    /// </summary>
    public string GetOptimizationInstructions()
    {
#if ANDROID
        return "Android Optimization Tips:\n\n" +
               "1. Disable battery optimization for this app\n" +
               "2. Allow app to run in background\n" +
               "3. Pin app in recent apps (some devices)\n" +
               "4. Check notification settings allow background service\n\n" +
               "The background service notification ensures reliable connectivity.";
#elif IOS
        return "iOS Optimization Tips:\n\n" +
               "1. Enable Background App Refresh:\n" +
               "   Settings → General → Background App Refresh → Enable for this app\n\n" +
               "2. Keep app active:\n" +
               "   • Don't force-quit the app\n" +
               "   • Open app periodically to refresh background time\n\n" +
               "3. iOS automatically manages background time to preserve battery.";
#else
        return "No optimization available for this platform.";
#endif
    }

    /// <summary>
    /// Shows implementation details for developers wanting to customize background services.
    /// </summary>
    public string GetImplementationDetails()
    {
#if ANDROID
        return "Android Implementation Details:\n\n" +
               "✅ Local Background Service: FlicBackgroundService.cs\n" +
               "✅ Boot Receiver: FlicBootReceiver.cs\n" +
               "✅ Foreground Service with notification\n" +
               "✅ Auto-restart after device reboot\n" +
               "✅ Survives app updates\n\n" +
               "This implementation avoids Java callable wrapper build errors\n" +
               "by keeping Android-specific components in the main app project.";
#elif IOS
        return "iOS Implementation Details:\n\n" +
               "✅ Background modes enabled in Info.plist\n" +
               "✅ Bluetooth Central background mode\n" +
               "✅ Background processing capabilities\n" +
               "✅ Automatic reconnection on app activation\n\n" +
               "iOS background execution is limited by the system\n" +
               "to preserve battery life and user privacy.";
#else
        return "No implementation details available for this platform.";
#endif
    }

    /// <summary>
    /// Demonstrates how to get template code for implementing background services.
    /// </summary>
    public string GetTemplateCodeInstructions()
    {
#if ANDROID
        return "Template Code Available:\n\n" +
               "Use AndroidBackgroundServiceHelper to get template code:\n\n" +
               "• GetBackgroundServiceTemplate() - Complete service implementation\n" +
               "• GetBootReceiverTemplate() - Boot receiver code\n" +
               "• GetManifestEntries() - Required AndroidManifest.xml entries\n" +
               "• GetMainActivityCode() - Service startup code\n\n" +
               "Copy these templates to your main project to implement background services.";
#elif IOS
        return "iOS Background Setup:\n\n" +
               "1. Add background modes to Info.plist:\n" +
               "   • bluetooth-central\n" +
               "   • background-processing\n" +
               "   • background-fetch\n\n" +
               "2. Call Configure.KeepFlicButtonsConnected() in AppDelegate\n\n" +
               "3. Request Background App Refresh permission from user\n\n" +
               "No additional code templates needed for iOS.";
#else
        return "No template code available for this platform.";
#endif
    }
}