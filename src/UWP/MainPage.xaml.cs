namespace TaglierinaPanoramica.UWP
{
    /// <summary>
    /// UWP main page
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Creates a new main page
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.LoadApplication(new TaglierinaPanoramica.App());
        }
    }
}
