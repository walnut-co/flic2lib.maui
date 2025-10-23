using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using flic2lib.Maui;

namespace flic2lib.Maui.Sample
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Start the local Flic background service for persistent button connectivity
            StartFlicBackgroundService();
        }

        private void StartFlicBackgroundService()
        {
            var serviceIntent = new Intent(this, typeof(Platforms.Android.FlicBackgroundService));

            // Use StartForegroundService for Android 8.0+ (API 26+), StartService for older versions
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(serviceIntent);
            }
            else
            {
                StartService(serviceIntent);
            }
        }
    }
}