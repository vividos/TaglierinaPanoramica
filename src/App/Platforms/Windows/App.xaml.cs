namespace TaglierinaPanoramica.WinUI
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
            this.InitializeComponent();
        }

        /// <summary>
        /// Creates a new Windows app object
        /// </summary>
        /// <returns>app object</returns>
        protected override MauiApp CreateMauiApp()
            => MauiProgram.CreateMauiApp();
    }
}
