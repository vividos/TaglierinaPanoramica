using Android.App;
using Android.Runtime;

namespace TaglierinaPanoramica.Droid
{
    /// <summary>
    /// Android application object
    /// </summary>
    [Application]
    public class MainApplication : MauiApplication
    {
        /// <summary>
        /// Creates a new Android application object
        /// </summary>
        /// <param name="handle">Android handle</param>
        /// <param name="ownership">handle ownership</param>
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        /// <summary>
        /// Creates a new Android app object
        /// </summary>
        /// <returns>app object</returns>
        protected override MauiApp CreateMauiApp()
            => MauiProgram.CreateMauiApp();
    }
}
