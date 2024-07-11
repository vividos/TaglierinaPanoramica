using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Bitmaps
{
    /// <summary>
    /// Canvas view that allows cropping of photos
    /// https://github.com/dotnet/maui-samples/blob/main/8.0/SkiaSharp/SkiaSharpDemos/SkiaSharpDemos/Bitmaps/PhotoCropperCanvasView.cs
    /// </summary>
    internal class PhotoCropperCanvasView : SKCanvasView
    {
        /// <summary>
        /// Pixel length of cropper corner
        /// </summary>
        private const int CORNER = 50;

        /// <summary>
        /// Pixel radius of touch hit-test
        /// </summary>
        private const int RADIUS = 100;

        /// <summary>
        /// Mapping of touch IDs to touch points
        /// </summary>
        private readonly Dictionary<long, TouchPoint> touchPoints = [];

        /// <summary>
        /// Corner stroke drawing object
        /// </summary>
        private readonly SKPaint cornerStroke = new()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 10
        };

        /// <summary>
        /// Edge stroke drawing object
        /// </summary>
        private readonly SKPaint edgeStroke = new()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 2
        };

        /// <summary>
        /// Divider dashed stroke drawing object
        /// </summary>
        private readonly SKPaint dividerPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([4, 2], 20)
        };

        /// <summary>
        /// Bitmap to crop
        /// </summary>
        private SKBitmap? bitmap;

        /// <summary>
        /// Current cropping rectangle
        /// </summary>
        private CroppingRectangle? croppingRect;

        /// <summary>
        /// Inverse bitmap matrix for touch tracking
        /// </summary>
        private SKMatrix inverseBitmapMatrix;

        /// <summary>
        /// Touch point infos for hit testing
        /// </summary>
        private struct TouchPoint
        {
            /// <summary>
            /// Corner index, from 0 to 3, or -1 when no corner was touched
            /// </summary>
            public int CornerIndex { get; set; }

            /// <summary>
            /// Offset from touch point to corner point
            /// </summary>
            public SKPoint Offset { get; set; }

            /// <summary>
            /// Determines if the touch point is inside of the rectangle, not one of the corner
            /// points
            /// </summary>
            public bool IsInsideRect { get; internal set; }
        }

        #region Bindable properties
        /// <summary>
        /// Specifies the SKBitmap bitmap object that should be cropped
        /// </summary>
        public static readonly BindableProperty BitmapProperty =
            BindableProperty.Create(
                nameof(Bitmap),
                typeof(SKBitmap),
                typeof(PhotoCropperCanvasView),
                null,
                propertyChanged: OnBitmapPropertyChanged);

        /// <summary>
        /// Specifies the rotate angle, in degrees
        /// </summary>
        public static readonly BindableProperty RotateAngleProperty =
            BindableProperty.Create(
                nameof(RotateAngle),
                typeof(double),
                typeof(PhotoCropperCanvasView),
                0.0,
                propertyChanged: OnRotateAnglePropertyChanged);

        /// <summary>
        /// Specifies the number of images that the cropped image should be tiled into.
        /// </summary>
        public static readonly BindableProperty NumberOfImagesProperty =
            BindableProperty.Create(
                nameof(NumberOfImages),
                typeof(int),
                typeof(PhotoCropperCanvasView),
                2,
                propertyChanged: OnNumberOfImagesPropertyChanged);

#pragma warning disable S4275 // Getters and setters should access the expected fields
        /// <summary>
        /// Bitmap to crop
        /// </summary>
        public SKBitmap Bitmap
        {
            get => (SKBitmap)this.GetValue(BitmapProperty);
            set => this.SetValue(BitmapProperty, value);
        }
#pragma warning restore S4275 // Getters and setters should access the expected fields

        /// <summary>
        /// Rotate angle, in degrees
        /// </summary>
        public double RotateAngle
        {
            get => (double)this.GetValue(RotateAngleProperty);
            set => this.SetValue(RotateAngleProperty, value);
        }

        /// <summary>
        /// Number of images to tile the cropped image into
        /// </summary>
        public int NumberOfImages
        {
            get => (int)this.GetValue(NumberOfImagesProperty);
            set => this.SetValue(NumberOfImagesProperty, value);
        }
        #endregion

        /// <summary>
        /// Creates a new photo cropper canvas view
        /// </summary>
        public PhotoCropperCanvasView()
        {
            this.EnableTouchEvents = true;
            this.Touch += this.OnTouch;
            this.IgnorePixelScaling = true;
        }

        /// <summary>
        /// Called when the bitmap property has changed; updates cropping rectangle and forces a
        /// redraw.
        /// </summary>
        /// <param name="bindable">bindable object</param>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        private static void OnBitmapPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PhotoCropperCanvasView view)
            {
                view.bitmap = (SKBitmap)newValue;
                view.UpdateCroppingRectangle();
                view.InvalidateSurface();
            }
        }

        /// <summary>
        /// Updates cropping rectangle by creating a new one from the bitmap size and number of
        /// images
        /// </summary>
        private void UpdateCroppingRectangle()
        {
            if (this.bitmap != null)
            {
                SKRect bitmapRect = new SKRect(0, 0, this.bitmap.Width, this.bitmap.Height);
                float aspectRatio = this.NumberOfImages;
                this.croppingRect = new CroppingRectangle(bitmapRect, aspectRatio);
            }
        }

        /// <summary>
        /// Called when the rotate angle property has changed; forces a redraw.
        /// </summary>
        /// <param name="bindable">bindable object</param>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        private static void OnRotateAnglePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PhotoCropperCanvasView view)
            {
                view.InvalidateSurface();
            }
        }

        /// <summary>
        /// Called when the number of images property has changed; updates cropping rectangle and
        /// forces a redraw.
        /// </summary>
        /// <param name="bindable">bindable object</param>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        private static void OnNumberOfImagesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PhotoCropperCanvasView view)
            {
                view.UpdateCroppingRectangle();
                view.InvalidateSurface();
            }
        }

        /// <summary>
        /// Calculates a scaled, cropped image, based on the current cropping rectangle.
        /// </summary>
        /// <param name="destWidth">destination bitmap width</param>
        /// <param name="destHeight">destination bitmap height</param>
        /// <returns>scaled, cropped bitmap</returns>
        public SKBitmap? GetScaledCroppedBitmap(int destWidth, int destHeight)
        {
            if (this.croppingRect == null)
            {
                return null;
            }

            SKRect cropRect = this.croppingRect.Rect;
            SKBitmap croppedBitmap = new SKBitmap(
                destWidth,
                destHeight);

            SKRect dest = new SKRect(0, 0, destWidth, destHeight);
            SKRect source = new SKRect(
                cropRect.Left,
                cropRect.Top,
                cropRect.Right,
                cropRect.Bottom);

            using (SKCanvas canvas = new SKCanvas(croppedBitmap))
            {
                canvas.DrawBitmap(this.bitmap, source, dest);
            }

            return croppedBitmap;
        }

        /// <summary>
        /// Called when the canvas suface should be repainted.
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.Gray);

            if (this.bitmap == null)
            {
                return;
            }

            // Calculate rectangle for displaying bitmap
            float scale = Math.Min((float)info.Width / this.bitmap.Width, (float)info.Height / this.bitmap.Height);
            float x = (info.Width - (scale * this.bitmap.Width)) / 2;
            float y = (info.Height - (scale * this.bitmap.Height)) / 2;
            var bitmapRect = new SKRect(x, y, x + (scale * this.bitmap.Width), y + (scale * this.bitmap.Height));

            canvas.DrawBitmap(this.bitmap, bitmapRect);

            if (this.croppingRect == null)
            {
                return;
            }

            // Calculate a matrix transform for displaying the cropping rectangle
            SKMatrix bitmapScaleMatrix = SKMatrix.CreateScaleTranslation(scale, scale, x, y);

            // Display rectangle
            SKRect scaledCropRect = bitmapScaleMatrix.MapRect(this.croppingRect.Rect);
            canvas.DrawRect(scaledCropRect, this.edgeStroke);

            // Display heavier corners
            using (SKPath path = new SKPath())
            {
                path.MoveTo(scaledCropRect.Left, scaledCropRect.Top + CORNER);
                path.LineTo(scaledCropRect.Left, scaledCropRect.Top);
                path.LineTo(scaledCropRect.Left + CORNER, scaledCropRect.Top);

                path.MoveTo(scaledCropRect.Right - CORNER, scaledCropRect.Top);
                path.LineTo(scaledCropRect.Right, scaledCropRect.Top);
                path.LineTo(scaledCropRect.Right, scaledCropRect.Top + CORNER);

                path.MoveTo(scaledCropRect.Right, scaledCropRect.Bottom - CORNER);
                path.LineTo(scaledCropRect.Right, scaledCropRect.Bottom);
                path.LineTo(scaledCropRect.Right - CORNER, scaledCropRect.Bottom);

                path.MoveTo(scaledCropRect.Left + CORNER, scaledCropRect.Bottom);
                path.LineTo(scaledCropRect.Left, scaledCropRect.Bottom);
                path.LineTo(scaledCropRect.Left, scaledCropRect.Bottom - CORNER);

                canvas.DrawPath(path, this.cornerStroke);
            }

            // Also draw divider
            float dividerOffset = scaledCropRect.Width / this.NumberOfImages;
            using (SKPath path = new SKPath())
            {
                for (int lineIndex = 1; lineIndex < this.NumberOfImages; lineIndex++)
                {
                    float dividerx = scaledCropRect.Left + (lineIndex * dividerOffset);
                    path.MoveTo(dividerx, scaledCropRect.Top);
                    path.LineTo(dividerx, scaledCropRect.Bottom);
                }

                canvas.DrawPath(path, this.dividerPaint);
            }

            // Invert the transform for touch tracking
            bitmapScaleMatrix.TryInvert(out this.inverseBitmapMatrix);
        }

        /// <summary>
        /// Called when a touch event occurred.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="args">event args</param>
        private void OnTouch(object? sender, SKTouchEventArgs args)
        {
            SKPoint pixelLocation = this.ConvertToPixel(args.Location);
            SKPoint bitmapLocation = this.inverseBitmapMatrix.MapPoint(pixelLocation);

            long touchId = args.Id;

            switch (args.ActionType)
            {
                case SKTouchAction.Pressed:
                    this.OnTouchActionPressed(touchId, bitmapLocation);
                    break;

                case SKTouchAction.Moved:
                    this.OnTouchActionMoved(touchId, bitmapLocation);
                    break;

                case SKTouchAction.Released:
                case SKTouchAction.Cancelled:
                    this.touchPoints.Remove(touchId);
                    break;
            }

            args.Handled = true;
        }

        /// <summary>
        /// Called to handle a Pressed touch action
        /// </summary>
        /// <param name="touchId">touch ID</param>
        /// <param name="bitmapLocation">bitmap location of touch action</param>
        private void OnTouchActionPressed(long touchId, SKPoint bitmapLocation)
        {
            if (this.croppingRect == null ||
                this.touchPoints.ContainsKey(touchId))
            {
                return;
            }

            // Convert radius to bitmap/cropping scale
            float radius = this.inverseBitmapMatrix.ScaleX * RADIUS;

            // Find corner that the finger is touching
            int cornerIndex = this.croppingRect.HitTest(bitmapLocation, radius);

            if (cornerIndex != -1)
            {
                TouchPoint touchPoint = new TouchPoint
                {
                    CornerIndex = cornerIndex,
                    IsInsideRect = false,
                    Offset = bitmapLocation - this.croppingRect.Corners[cornerIndex]
                };

                this.touchPoints.Add(touchId, touchPoint);
            }
            else
            {
                // Touched inside the rectangle
                if (this.croppingRect.Rect.Contains(bitmapLocation))
                {
                    TouchPoint touchPoint = new TouchPoint
                    {
                        CornerIndex = -1,
                        IsInsideRect = true,
                        Offset = bitmapLocation,
                    };

                    this.touchPoints.Add(touchId, touchPoint);
                }
            }
        }

        /// <summary>
        /// Called to handle a Moved touch action
        /// </summary>
        /// <param name="touchId">touch ID</param>
        /// <param name="bitmapLocation">bitmap location of touch action</param>
        private void OnTouchActionMoved(long touchId, SKPoint bitmapLocation)
        {
            if (this.croppingRect == null ||
                !this.touchPoints.TryGetValue(touchId, out TouchPoint touchPoint))
            {
                return;
            }

            SKPoint delta = bitmapLocation - touchPoint.Offset;

            if (touchPoint.IsInsideRect)
            {
                this.croppingRect.MoveRect(delta);

                touchPoint.Offset = bitmapLocation;
                this.touchPoints[touchId] = touchPoint;
            }
            else
            {
                this.croppingRect.MoveCorner(
                    touchPoint.CornerIndex,
                    delta);
            }

            this.InvalidateSurface();
        }

        /// <summary>
        /// Converts a touch point to the pixel value on the bitmap to crop
        /// </summary>
        /// <param name="pt">touch point</param>
        /// <returns>bitmap point</returns>
        private SKPoint ConvertToPixel(SKPoint pt)
        {
            return new SKPoint(
                (float)(this.CanvasSize.Width * pt.X / this.Width),
                (float)(this.CanvasSize.Height * pt.Y / this.Height));
        }
    }
}
