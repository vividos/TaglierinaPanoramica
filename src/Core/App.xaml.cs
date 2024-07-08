[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TaglierinaPanoramica
{
    /// <summary>
    /// Forms application
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Creates a new app object
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // on WinUI, there must be a navigation page to show toast messages
            this.MainPage = DeviceInfo.Platform == DevicePlatform.WinUI
                ? new NavigationPage(new ImageCropPage())
                : new ImageCropPage();
        }
    }
}
