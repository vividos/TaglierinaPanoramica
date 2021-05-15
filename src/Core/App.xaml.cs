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
            this.MainPage = new ImageCropPage();
        }
    }
}
