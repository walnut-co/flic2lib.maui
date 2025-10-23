using Foundation;
using UIKit;

namespace flic2lib.Maui.Platforms.iOS;

/// <summary>
/// Helper class to manage Flic2 background execution on iOS.
/// Provides easy-to-use methods for enabling background app refresh and maintaining
/// Flic2 button connectivity when the app is backgrounded.
/// </summary>
public static class Flic2BackgroundManager
{
    /// <summary>
    /// Enables background connectivity for Flic2 buttons on iOS.
    /// This configures the necessary background modes and maintains FlicManager state.
    /// </summary>
    public static void EnableBackgroundConnectivity()
    {
        // Ensure FlicManager is initialized
        if (!FlicManager.IsInitialized)
        {
            FlicManager.Init();
        }

        // Register for background task handling
        RegisterBackgroundTasks();
    }

    /// <summary>
    /// Registers background task handlers for iOS to maintain Flic connectivity.
    /// This should be called during app initialization.
    /// </summary>
    private static void RegisterBackgroundTasks()
    {
        // Register for background app refresh notifications
        NSNotificationCenter.DefaultCenter.AddObserver(
            UIApplication.DidEnterBackgroundNotification,
            HandleAppDidEnterBackground);

        NSNotificationCenter.DefaultCenter.AddObserver(
            UIApplication.WillEnterForegroundNotification,
            HandleAppWillEnterForeground);
    }

    /// <summary>
    /// Handles when the app enters background mode.
    /// Requests background time to maintain Flic connections.
    /// </summary>
    private static void HandleAppDidEnterBackground(NSNotification notification)
    {
        var application = UIApplication.SharedApplication;

        // Request background time for maintaining Bluetooth connections
        var backgroundTaskId = application.BeginBackgroundTask("Flic2BackgroundTask", () =>
        {
            // This block is called when the background time is about to expire
            // Clean up any resources if needed
        });

        // Start a task to maintain FlicManager connection
        Task.Run(async () =>
        {
            try
            {
                // Keep the FlicManager active for as long as possible
                // iOS allows limited background time (usually 30 seconds to a few minutes)
                while (application.BackgroundTimeRemaining > 5.0)
                {
                    // Maintain Flic connection state
                    if (FlicManager.IsInitialized)
                    {
                        // The FlicManager should handle maintaining connections internally
                        await Task.Delay(1000);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                // End the background task
                application.EndBackgroundTask(backgroundTaskId);
            }
        });
    }

    /// <summary>
    /// Handles when the app is about to enter foreground mode.
    /// Ensures FlicManager is ready to resume full operation.
    /// </summary>
    private static void HandleAppWillEnterForeground(NSNotification notification)
    {
        // Ensure FlicManager is still initialized and ready
        if (!FlicManager.IsInitialized)
        {
            FlicManager.Init();
        }
    }

    /// <summary>
    /// Checks if background app refresh is enabled for this app.
    /// </summary>
    /// <returns>True if background app refresh is available</returns>
    public static bool IsBackgroundAppRefreshEnabled()
    {
        return UIApplication.SharedApplication.BackgroundRefreshStatus == UIBackgroundRefreshStatus.Available;
    }

    /// <summary>
    /// Gets information about background capabilities for troubleshooting.
    /// </summary>
    /// <returns>String describing background status</returns>
    public static string GetBackgroundStatus()
    {
        var status = UIApplication.SharedApplication.BackgroundRefreshStatus;
        return status switch
        {
            UIBackgroundRefreshStatus.Available => "Background App Refresh is available",
            UIBackgroundRefreshStatus.Denied => "Background App Refresh is denied by user",
            UIBackgroundRefreshStatus.Restricted => "Background App Refresh is restricted",
            _ => "Background App Refresh status unknown"
        };
    }
}