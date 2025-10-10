using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TaglierinaPanoramica
{
    /// <summary>
    /// Taglierina Panoramica app
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Creates a new app object
        /// </summary>
        public App()
        {
            AppCenter.Start(
                "83722e40-26a1-40b3-bd01-10bcf629c46b",
                typeof(Distribute),
                typeof(Crashes));

            this.InitializeComponent();
        }

        /// <summary>
        /// Called when the app's window is about to be created. Sets the app's title.
        /// </summary>
        /// <param name="activationState">activation state</param>
        /// <returns>window object</returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var titleBar = DeviceInfo.Platform == DevicePlatform.WinUI
                ? new TitleBar
                {
                    Title = "Taglierina Panoramica",
                    BackgroundColor = Color.FromArgb("29ccbf"),
                    ForegroundColor = Colors.White,
                }
                : null;

            return new Window
            {
                Title = "Taglierina Panoramica",
                TitleBar = titleBar,
                Page = new ImageCropPage(),
            };
        }
    }
}
