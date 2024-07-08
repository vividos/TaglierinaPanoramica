using Android.App;
using Android.Content.PM;
using Microsoft.Maui;

namespace TaglierinaPanoramica.Droid
{
    /// <summary>
    /// Android main activity
    /// </summary>
    [Activity(
        Label = "Taglierina Panoramica",
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
