using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace TaglierinaPanoramica.Droid
{
    /// <summary>
    /// Splash screen activity; is started first, and can't be navigated back on.
    /// </summary>
    [Activity(
        MainLauncher = true,
        NoHistory = true,
        Theme = "@style/SplashTheme",
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTop)]
    public class SplashActivity : Activity
    {
        /// <summary>
        /// Called in the activity lifecycle when the activity is about to be created. Just
        /// switches to the main activity once the Android app has been loaded.
        /// </summary>
        /// <param name="savedInstanceState">instance state; unused</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.AddFlags(ActivityFlags.SingleTop);
            this.StartActivity(intent);

            this.Finish();
        }

        /// <summary>
        /// Called when the user presses the back button.
        /// </summary>
        public override void OnBackPressed()
        {
            // prevent user from navigating back by not calling base
        }
    }
}
