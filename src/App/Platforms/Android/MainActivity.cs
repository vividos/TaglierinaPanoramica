using Android.App;
using Android.Content.PM;

namespace TaglierinaPanoramica.Droid
{
    /// <summary>
    /// Android main activity
    /// </summary>
    [Activity(
        Label = "Taglierina Panoramica",
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
