using Foundation;
using flic2lib.Maui;

namespace flic2lib.Maui.Sample
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIKit.UIApplication application, Foundation.NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            // Keep Flic2 buttons connected when app is in background
            Configure.KeepFlicButtonsConnected();

            return result;
        }
    }
}