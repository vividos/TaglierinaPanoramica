﻿using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TaglierinaPanoramica
{
    /// <summary>
    /// View model for the image crop page
    /// </summary>
    public class ImageCropViewModel : ObservableObject
    {
        /// <summary>
        /// Backing store for the square width of the image
        /// </summary>
        private int squareWidth = 1080;

        /// <summary>
        /// Backing store of the rotate angle, in degrees
        /// </summary>
        private double rotateAngle = 0.0;

        /// <summary>
        /// Number of images to create
        /// </summary>
        private int numberOfImages = 2;

        /// <summary>
        /// Function to get the cropped image from the view
        /// </summary>
        public Func<int, int, SKBitmap> GetCroppedImage { get; internal set; }

        #region Binding properties

        /// <summary>
        /// Command to execute to open an image
        /// </summary>
        public ICommand OpenImageCommand { get; set; }

        /// <summary>
        /// Command to execute to rotate the image
        /// </summary>
        public ICommand RotateImageCommand { get; set; }

        /// <summary>
        /// Output resolution as text
        /// </summary>
        public string OutputResolution { get; set; }

        /// <summary>
        /// Rotate angle, in degrees
        /// </summary>
        public double RotateAngle
        {
            get => this.rotateAngle;
            set => this.SetProperty(ref this.rotateAngle, value);
        }

        /// <summary>
        /// Sqare size of image
        /// </summary>
        public int SquareWidth
        {
            get => this.squareWidth;
            set => this.SetProperty(
                ref this.squareWidth,
                value,
                onChanged: this.UpdateCropView);
        }

        /// <summary>
        /// Number of images to cut the panorama into
        /// </summary>
        public int NumberOfImages
        {
            get => this.numberOfImages;
            set => this.SetProperty(
                ref this.numberOfImages,
                value,
                onChanged: this.UpdateCropView);
        }

        /// <summary>
        /// Command to execute to save the cropped images
        /// </summary>
        public ICommand SaveImagesCommand { get; set; }

        /// <summary>
        /// Command to execute to show the info popup
        /// </summary>
        public ICommand InfoCommand { get; set; }

        /// <summary>
        /// Indicates if the crop view placeholder is visible
        /// </summary>
        public bool IsCropViewPlaceholderAvailable => !this.IsImageAvailable;

        /// <summary>
        /// Indicates if the image to crop is available
        /// </summary>
        public bool IsImageAvailable => this.OriginalImage != null;

        /// <summary>
        /// Original image filename
        /// </summary>
        private string originalImageFilename;

        /// <summary>
        /// Property containing the original image to crop
        /// </summary>
        public SKBitmap OriginalImage { get; set; }
        #endregion

        /// <summary>
        /// Creates a new view model
        /// </summary>
        public ImageCropViewModel()
        {
            this.OpenImageCommand = new AsyncCommand(this.OpenImageAsync);
            this.RotateImageCommand = new Command(this.RotateImage);
            this.SaveImagesCommand = new AsyncCommand(this.SaveImagesAsync);
            this.InfoCommand = new Command(this.ShowInfo);

            this.UpdateCropView();
        }

        /// <summary>
        /// Called when the open image button is tapped
        /// </summary>
        /// <returns>task to wait on</returns>
        private async Task OpenImageAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(PickOptions.Images);
                if (result == null)
                {
                    return;
                }

                using (var stream = await result.OpenReadAsync())
                {
                    this.originalImageFilename = result.FileName;

                    this.OriginalImage = SKBitmap.Decode(stream);
                    this.OnPropertyChanged(nameof(this.OriginalImage));
                    this.OnPropertyChanged(nameof(this.IsImageAvailable));
                    this.OnPropertyChanged(nameof(this.IsCropViewPlaceholderAvailable));
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error while picking file", ex.Message, "Close");
            }
        }

        /// <summary>
        /// Called when the rotate image button is tapped
        /// </summary>
        private void RotateImage()
        {
            this.RotateAngle = (this.RotateAngle + 90.0) % 360.0;
        }

        /// <summary>
        /// Called when the square size or the number of images have changed
        /// </summary>
        private void UpdateCropView()
        {
            this.OutputResolution =
                $"{this.squareWidth} x {this.squareWidth * this.numberOfImages}";

            this.OnPropertyChanged(nameof(this.OutputResolution));
        }

        /// <summary>
        /// Called when the save images button is tapped
        /// </summary>
        /// <returns>task to wait on</returns>
        private async Task SaveImagesAsync()
        {
            int imageWidth = this.squareWidth * this.numberOfImages;
            SKBitmap bitmap = this.GetCroppedImage?.Invoke(imageWidth, this.squareWidth);

            if (bitmap == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error while sharing images",
                    $"Could not create cropped image",
                    "Close");

                return;
            }

            int numErrorImages = 0;
            for (int imageIndex = 0; imageIndex < this.numberOfImages; imageIndex++)
            {
                SKBitmap subImage = new SKBitmap(this.squareWidth, this.squareWidth, isOpaque: true);

                var sourceRectangle = SKRectI.Create(
                    imageIndex * this.squareWidth,
                    0,
                    this.squareWidth,
                    this.squareWidth);

                bool result = bitmap.ExtractSubset(
                    subImage,
                    sourceRectangle);

                if (!result)
                {
                    numErrorImages++;
                    continue;
                }

                try
                {
                    await this.StoreImageAsync(subImage, imageIndex);
                }
                catch (Exception)
                {
                    numErrorImages++;
                }
            }

            if (numErrorImages > 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error while sharing images",
                    $"{numErrorImages} images were not exported",
                    "Close");

                return;
            }

            await Application.Current.MainPage.DisplayToastAsync("All tiled images were saved.");
        }

        private async Task StoreImageAsync(SKBitmap subImage, int imageIndex)
        {
            string outputFilename =
                Path.GetFileNameWithoutExtension(this.originalImageFilename) +
                $"-tiled-{imageIndex}.jpg";

            using (var outputStream = new MemoryStream())
            {
                subImage.Encode(outputStream, SKEncodedImageFormat.Jpeg, 95);

                var photoLibrary = DependencyService.Get<IPhotoLibrary>();

                byte[] data = outputStream.ToArray();
                await photoLibrary.SavePhotoAsync(data, "TaglierinaPanoramica", outputFilename);
            }
        }

        /// <summary>
        /// Called when the info button is tapped
        /// </summary>
        private void ShowInfo()
        {
            Application.Current.MainPage.Navigation.ShowPopup(new InfoPopup());
        }
    }
}