using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Uri = Android.Net.Uri;

namespace TaglierinaPanoramica.Droid
{
    /// <summary>
    /// Android main activity
    /// </summary>
    [Activity(
        Label = "Taglierina Panoramica",
        Theme = "@style/Maui.SplashTheme",
        ScreenOrientation = ScreenOrientation.Landscape,
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(
        [Intent.ActionView, Intent.ActionSend],
        Categories = [Intent.CategoryDefault, Intent.CategoryBrowsable],
        DataMimeTypes = ["image/*"],
        Icon = "@mipmap/appicon")]
    public class MainActivity : MauiAppCompatActivity
    {
        /// <summary>
        /// Called when a new intent is sent to the activity
        /// </summary>
        /// <param name="intent">intent sent to this activity</param>
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);

            this.Intent = intent;
        }

        /// <summary>
        /// Called when activity is about to be resumed; this is called after OnCreate or
        /// OnNewIntent in order to check for a new file to open.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            if (this.Intent != null)
            {
                this.ProcessIntent(this.Intent);
                this.Intent = null;
            }
        }

        /// <summary>
        /// Processes intent that the app was started with, e.g. by sending an image to the app
        /// See: https://stackoverflow.com/questions/3760276/android-intent-filter-associate-app-with-file-extension
        /// </summary>
        /// <param name="intent">intent that was passed to the app</param>
        private void ProcessIntent(Intent intent)
        {
            if (intent.Type == null ||
                !intent.Type.StartsWith("image/"))
            {
                return;
            }

            var uri = GetUriFromIntent(intent);
            if (uri == null)
            {
                return;
            }

            string? contentName = this.GetContentNameFromContentUri(uri);
            using var stream = this.ContentResolver?.OpenInputStream(uri);

            if (stream != null &&
                App.Current?.MainPage is ImageCropPage page)
            {
                page.LoadImage(stream, contentName);
            }
        }

        /// <summary>
        /// Retrieves an Uri from given intent
        /// </summary>
        /// <param name="intent">intent to use</param>
        /// <returns>Uri object, or null when it couldn't be retrieved</returns>
        private static Uri? GetUriFromIntent(Intent intent)
        {
            if (intent.Data != null)
            {
                return intent.Data;
            }

#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1422 // Validate platform compatibility
            return
                Build.VERSION.SdkInt > BuildVersionCodes.Tiramisu
                ? intent.Extras?.GetParcelable(Intent.ExtraStream, Java.Lang.Class.FromType(typeof(Uri))) as Uri
                : intent.Extras?.GetParcelable(Intent.ExtraStream) as Uri;
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
        }

        /// <summary>
        /// Retrieves content filename from content:// URI
        /// </summary>
        /// <param name="uri">content URI</param>
        /// <returns>filename-only part</returns>
        private string? GetContentNameFromContentUri(Uri uri)
        {
            ICursor? cursor = this.ContentResolver?.Query(uri, null, null, null, null);
            if (cursor == null)
            {
                return null;
            }

            cursor.MoveToFirst();

            int nameIndex = cursor.GetColumnIndex(
                Android.Provider.MediaStore.IMediaColumns.DisplayName);

            return nameIndex >= 0
                ? cursor.GetString(nameIndex)
                : null;
        }
    }
}
