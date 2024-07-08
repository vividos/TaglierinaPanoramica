using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.UI;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace TaglierinaPanoramica.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>

    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.BackgroundColor =
                titleBar.ButtonBackgroundColor =
                titleBar.InactiveBackgroundColor =
                (Color)Current.Resources["PrimaryColor"];

            titleBar.ForegroundColor =
                titleBar.InactiveForegroundColor =
                titleBar.InactiveForegroundColor =
                Colors.White;
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
