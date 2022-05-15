using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

            // on UWP, there must be a navigation page to show toast messages
            this.MainPage = DeviceInfo.Platform == DevicePlatform.UWP
                ? new NavigationPage(new ImageCropPage())
                : new ImageCropPage();
        }
    }
}
