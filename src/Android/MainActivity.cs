using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

namespace TaglierinaPanoramica.Droid
{
    /// <summary>
    /// Android main activity
    /// </summary>
    [Activity(
        Label = "Taglierina Panoramica",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// Called when activity is about to be created
        /// </summary>
        /// <param name="savedInstanceState">saved instance state</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppCenter.Start(
                "83722e40-26a1-40b3-bd01-10bcf629c46b",
                typeof(Distribute),
                typeof(Crashes));

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            this.LoadApplication(new App());
        }

        /// <summary>
        /// Called when there's a result for a requested permission
        /// </summary>
        /// <param name="requestCode">request code</param>
        /// <param name="permissions">list of permissions</param>
        /// <param name="grantResults">permission grant results</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
