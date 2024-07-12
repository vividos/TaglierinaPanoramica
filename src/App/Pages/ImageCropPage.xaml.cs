namespace TaglierinaPanoramica
{
    /// <summary>
    /// Image crop page
    /// </summary>
    public partial class ImageCropPage : ContentPage
    {
        /// <summary>
        /// View model for the image crop page
        /// </summary>
        private ImageCropViewModel ViewModel
            => this.BindingContext as ImageCropViewModel
            ?? throw new InvalidOperationException("view model not set as binding context!");

        /// <summary>
        /// Creates a new page object
        /// </summary>
        public ImageCropPage()
        {
            this.InitializeComponent();

            this.ViewModel.GetCroppedImage =
                (width, height) => this.cropView?.GetScaledCroppedBitmap(width, height);
        }

        /// <summary>
        /// Loads image from stream
        /// </summary>
        /// <param name="stream">image stream</param>
        /// <param name="originalFilename">original filename</param>
        public void LoadImage(Stream stream, string? originalFilename)
        {
            this.ViewModel.LoadImage(stream, originalFilename);
        }

        /// <summary>
        /// Called when the square size slider value has changed
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="args">event args</param>
        private void Slider_ValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (sender is not Slider localSquareWidthSlider)
            {
                return;
            }

            const int StepSize = 8;

            var newStep = Math.Round(args.NewValue / StepSize);
            localSquareWidthSlider.Value = newStep * StepSize;
        }
    }
}
