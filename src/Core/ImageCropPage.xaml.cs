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
            => this.BindingContext as ImageCropViewModel;

        /// <summary>
        /// Creates a new page object
        /// </summary>
        public ImageCropPage()
        {
            this.InitializeComponent();

            this.ViewModel.GetCroppedImage =
                (width, height) => this.cropView.GetScaledCroppedBitmap(width, height);
        }

        /// <summary>
        /// Called when the square size slider value has changed
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="args">event args</param>
        private void Slider_ValueChanged(object sender, ValueChangedEventArgs args)
        {
            const int StepSize = 8;

            var newStep = Math.Round(args.NewValue / StepSize);
            this.squareWidthSlider.Value = newStep * StepSize;
        }
    }
}
