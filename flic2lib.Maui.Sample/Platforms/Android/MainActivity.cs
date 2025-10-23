using Android.App;
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

            // Keep Flic2 buttons connected when app is in background
            Configure.KeepFlicButtonsConnected(this);
        }
    }
}