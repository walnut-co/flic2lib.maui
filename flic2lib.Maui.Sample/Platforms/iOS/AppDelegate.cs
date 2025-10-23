using Foundation;
using flic2lib.Maui;
using flic2lib.Maui.Sample.Services;
using Microsoft.Extensions.DependencyInjection;
using UIKit;
using CoreFoundation;

namespace flic2lib.Maui.Sample
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        private FlicEventHandler? _flicEventHandler;
        private MauiApp? _mauiApp;

        protected override MauiApp CreateMauiApp()
        {
            _mauiApp = MauiProgram.CreateMauiApp();
            return _mauiApp;
        }

        public override bool FinishedLaunching(UIKit.UIApplication application, Foundation.NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            // Keep Flic2 buttons connected when app is in background
            Configure.KeepFlicButtonsConnected();

            // Initialize FlicEventHandler for background events
            InitializeFlicEventHandler();

            // Request background app refresh permission
            RequestBackgroundAppRefresh();

            return result;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            base.DidEnterBackground(application);

            // iOS allows limited background execution (30 seconds - 3 minutes)
            // Ensure FlicEventHandler is active for background button events
            EnsureBackgroundEventHandling();

            System.Diagnostics.Debug.WriteLine("iOS: App entered background - Flic events will work for limited time");
        }

        public override void WillEnterForeground(UIApplication application)
        {
            base.WillEnterForeground(application);

            // Re-initialize connections when returning to foreground
            Configure.KeepFlicButtonsConnected();
            EnsureFlicEventHandlerActive();

            System.Diagnostics.Debug.WriteLine("iOS: App entering foreground - Flic connectivity refreshed");
        }

        public override void OnActivated(UIApplication application)
        {
            base.OnActivated(application);

            // Ensure event handler is active when app becomes active
            EnsureFlicEventHandlerActive();
            System.Diagnostics.Debug.WriteLine("iOS: App became active");
        }

        private void InitializeFlicEventHandler()
        {
            try
            {
                // Get the FlicEventHandler from the stored MauiApp service provider
                if (_mauiApp?.Services != null)
                {
                    _flicEventHandler = _mauiApp.Services.GetService<FlicEventHandler>();
                    if (_flicEventHandler != null)
                    {
                        System.Diagnostics.Debug.WriteLine("iOS: FlicEventHandler initialized successfully");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("iOS: Warning - FlicEventHandler not found in service provider");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("iOS: Warning - MauiApp or Services not available");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"iOS: Error initializing FlicEventHandler: {ex.Message}");
            }
        }

        private void EnsureFlicEventHandlerActive()
        {
            if (_flicEventHandler == null)
            {
                InitializeFlicEventHandler();
            }
        }

        private void EnsureBackgroundEventHandling()
        {
            // iOS background execution is limited, but we can ensure our handler is ready
            EnsureFlicEventHandlerActive();

            // Request background task to extend execution time for critical operations
            var taskId = UIApplication.SharedApplication.BeginBackgroundTask("FlicEventHandling", () =>
            {
                System.Diagnostics.Debug.WriteLine("iOS: Background task expired");
            });

            // The background task gives us extra time to handle button events
            if (taskId != UIApplication.BackgroundTaskInvalid)
            {
                System.Diagnostics.Debug.WriteLine("iOS: Background task started for Flic event handling");

                // End the task after a reasonable time (iOS will eventually end it anyway)
                DispatchQueue.MainQueue.DispatchAfter(
                    new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(25)),
                    () =>
                    {
                        try
                        {
                            UIApplication.SharedApplication.EndBackgroundTask(taskId);
                            System.Diagnostics.Debug.WriteLine("iOS: Background task ended");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"iOS: Error ending background task: {ex.Message}");
                        }
                    });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("iOS: Failed to start background task");
            }
        }

        private void RequestBackgroundAppRefresh()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                try
                {
                    UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
                    System.Diagnostics.Debug.WriteLine("iOS: Background app refresh configured");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"iOS: Error configuring background refresh: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("iOS: Background app refresh requires iOS 13+");
            }
        }

        // Handle background fetch for iOS 13+
        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            System.Diagnostics.Debug.WriteLine("iOS: Background fetch triggered - refreshing Flic connections");

            try
            {
                // Refresh Flic connections during background fetch
                Configure.KeepFlicButtonsConnected();
                EnsureFlicEventHandlerActive();

                // Give iOS feedback that we successfully refreshed
                completionHandler(UIBackgroundFetchResult.NewData);
                System.Diagnostics.Debug.WriteLine("iOS: Background fetch completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"iOS: Background fetch error: {ex.Message}");
                completionHandler(UIBackgroundFetchResult.Failed);
            }
        }

        public override void OnResignActivation(UIApplication application)
        {
            base.OnResignActivation(application);
            System.Diagnostics.Debug.WriteLine("iOS: App resigned activation");
        }
    }
}