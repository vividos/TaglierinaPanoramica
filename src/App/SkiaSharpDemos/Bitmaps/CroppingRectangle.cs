using SkiaSharp;

namespace SkiaSharpDemos.Bitmaps
{
    /// <summary>
    /// Cropping rectangle data and handling
    /// </summary>
    internal class CroppingRectangle
    {
        /// <summary>
        /// Minimum width or height of cropping rectangle, in pixels
        /// </summary>
        private const float MINIMUM = 10;

        /// <summary>
        /// Aspect ratio of rectangle to keep; may be null
        /// </summary>
        private readonly float? aspectRatio;

        /// <summary>
        /// Maximum rectangle; generally the size of the bitmap
        /// </summary>
        private SKRect maxRect;

        /// <summary>
        /// Creates a new cropping rectangle instance
        /// </summary>
        /// <param name="maxRect">maximum rectangle</param>
        /// <param name="aspectRatio">aspect ratio; may be null</param>
        public CroppingRectangle(SKRect maxRect, float? aspectRatio = null)
        {
            this.maxRect = maxRect;
            this.aspectRatio = aspectRatio;

            // Set initial cropping rectangle
            this.Rect = new SKRect(
                (0.9f * maxRect.Left) + (0.1f * maxRect.Right),
                (0.9f * maxRect.Top) + (0.1f * maxRect.Bottom),
                (0.1f * maxRect.Left) + (0.9f * maxRect.Right),
                (0.1f * maxRect.Top) + (0.9f * maxRect.Bottom));

            // Adjust for aspect ratio
            if (aspectRatio.HasValue)
            {
                SKRect rect = this.Rect;
                float aspect = aspectRatio.Value;

                if (rect.Width > aspect * rect.Height)
                {
                    float width = aspect * rect.Height;
                    rect.Left = (maxRect.Width - width) / 2;
                    rect.Right = rect.Left + width;
                }
                else
                {
                    float height = rect.Width / aspect;
                    rect.Top = (maxRect.Height - height) / 2;
                    rect.Bottom = rect.Top + height;
                }

                this.Rect = rect;
            }
        }

        /// <summary>
        /// Cropping rectangle
        /// </summary>
        public SKRect Rect { get; set; }

        /// <summary>
        /// Corner points array of the rectangle
        /// </summary>
        public SKPoint[] Corners
        {
            get
            {
                return
                [
                    new SKPoint(this.Rect.Left, this.Rect.Top),
                    new SKPoint(this.Rect.Right, this.Rect.Top),
                    new SKPoint(this.Rect.Right, this.Rect.Bottom),
                    new SKPoint(this.Rect.Left, this.Rect.Bottom)
                ];
            }
        }

        /// <summary>
        /// Performs a hit test for the given point and the radius. Returns which part of the
        /// rectangle has been hit.
        /// </summary>
        /// <param name="point">point to check</param>
        /// <param name="radius">point radius</param>
        /// <returns>
        /// -1 if nothing was hit; 0..3 when any of the corners were hit
        /// </returns>
        public int HitTest(SKPoint point, float radius)
        {
            SKPoint[] corners = this.Corners;

            for (int index = 0; index < corners.Length; index++)
            {
                SKPoint diff = point - corners[index];

                if ((float)Math.Sqrt((diff.X * diff.X) + (diff.Y * diff.Y)) < radius)
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Moves the cropping rectangle by a delta value in x and y direction, while keeping the
        /// rectangle inside the maximum rectangle and keeping the aspect ratio.
        /// </summary>
        /// <param name="delta">rectangle delta</param>
        public void MoveRect(SKPoint delta)
        {
            SKRect rect = this.Rect;

            rect.Offset(delta);

            float width = rect.Width;
            float height = rect.Height;

            // limit moving the rectangle
            rect.Left = Math.Max(rect.Left, 0.0f);
            rect.Top = Math.Max(rect.Top, 0.0f);

            if (this.aspectRatio.HasValue)
            {
                rect.Right = rect.Left + width;
                rect.Bottom = rect.Top + height;
            }

            rect.Right = Math.Min(rect.Right, this.maxRect.Right);
            rect.Bottom = Math.Min(rect.Bottom, this.maxRect.Bottom);

            if (this.aspectRatio.HasValue)
            {
                rect.Left = rect.Right - width;
                rect.Top = rect.Bottom - height;
            }

            this.Rect = rect;
        }

        /// <summary>
        /// Moves a single corner of the rectangle by a delta value in x and y direction, while
        /// keeping the rectangle inside the maximum rectangle and keeping the aspect ratio.
        /// </summary>
        /// <param name="index">corner index</param>
        /// <param name="delta">rectangle delta</param>
        public void MoveCorner(int index, SKPoint delta)
        {
            SKRect rect = this.Rect;

            switch (index)
            {
                case 0: // upper-left
                    rect.Left = Math.Min(Math.Max(delta.X, this.maxRect.Left), rect.Right - MINIMUM);
                    rect.Top = Math.Min(Math.Max(delta.Y, this.maxRect.Top), rect.Bottom - MINIMUM);
                    break;

                case 1: // upper-right
                    rect.Right = Math.Max(Math.Min(delta.X, this.maxRect.Right), rect.Left + MINIMUM);
                    rect.Top = Math.Min(Math.Max(delta.Y, this.maxRect.Top), rect.Bottom - MINIMUM);
                    break;

                case 2: // lower-right
                    rect.Right = Math.Max(Math.Min(delta.X, this.maxRect.Right), rect.Left + MINIMUM);
                    rect.Bottom = Math.Max(Math.Min(delta.Y, this.maxRect.Bottom), rect.Top + MINIMUM);
                    break;

                case 3: // lower-left
                    rect.Left = Math.Min(Math.Max(delta.X, this.maxRect.Left), rect.Right - MINIMUM);
                    rect.Bottom = Math.Max(Math.Min(delta.Y, this.maxRect.Bottom), rect.Top + MINIMUM);
                    break;
            }

            rect = this.AdjustRectForAspectRatio(index, rect);

            this.Rect = rect;
        }

        /// <summary>
        /// Adjusts the rectangle to keep the aspect ratio; called while moving a corner of the
        /// rectangle.
        /// </summary>
        /// <param name="index">corner index</param>
        /// <param name="rect">rectangle to modify</param>
        /// <returns>modified rectangle</returns>
        private SKRect AdjustRectForAspectRatio(int index, SKRect rect)
        {
            // Adjust for aspect ratio
            if (this.aspectRatio.HasValue)
            {
                float aspect = this.aspectRatio.Value;

                if (rect.Width > aspect * rect.Height)
                {
                    float width = aspect * rect.Height;

                    switch (index)
                    {
                        case 0:
                        case 3: rect.Left = rect.Right - width; break;
                        case 1:
                        case 2: rect.Right = rect.Left + width; break;
                    }
                }
                else
                {
                    float height = rect.Width / aspect;

                    switch (index)
                    {
                        case 0:
                        case 1: rect.Top = rect.Bottom - height; break;
                        case 2:
                        case 3: rect.Bottom = rect.Top + height; break;
                    }
                }
            }

            return rect;
        }
    }
}
