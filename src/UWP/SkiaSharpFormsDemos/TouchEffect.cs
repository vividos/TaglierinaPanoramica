using Microsoft.Maui.Controls;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Linq;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchTracking.UWP.TouchEffect), "TouchEffect")]

namespace TouchTracking.UWP
{
    /// <summary>
    /// Touch effect implementation for UWP
    /// </summary>
    public class TouchEffect : PlatformEffect
    {
        private FrameworkElement frameworkElement;
        private TouchTracking.TouchEffect effect;
        private Action<Element, TouchActionEventArgs> onTouchAction;

        protected override void OnAttached()
        {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            this.frameworkElement = this.Control == null ? this.Container : this.Control;

            // Get access to the TouchEffect class in the .NET Standard library
            this.effect = (TouchTracking.TouchEffect)this.Element.Effects.
                        FirstOrDefault(e => e is TouchTracking.TouchEffect);

            if (this.effect != null && this.frameworkElement != null)
            {
                // Save the method to call on touch events
                this.onTouchAction = this.effect.OnTouchAction;

                // Set event handlers on FrameworkElement
                this.frameworkElement.PointerEntered += this.OnPointerEntered;
                this.frameworkElement.PointerPressed += this.OnPointerPressed;
                this.frameworkElement.PointerMoved += this.OnPointerMoved;
                this.frameworkElement.PointerReleased += this.OnPointerReleased;
                this.frameworkElement.PointerExited += this.OnPointerExited;
                this.frameworkElement.PointerCanceled += this.OnPointerCancelled;
            }
        }

        protected override void OnDetached()
        {
            if (this.onTouchAction != null)
            {
                // Release event handlers on FrameworkElement
                this.frameworkElement.PointerEntered -= this.OnPointerEntered;
                this.frameworkElement.PointerPressed -= this.OnPointerPressed;
                this.frameworkElement.PointerMoved -= this.OnPointerMoved;
                this.frameworkElement.PointerReleased -= this.OnPointerReleased;
                this.frameworkElement.PointerExited -= this.OnPointerEntered;
                this.frameworkElement.PointerCanceled -= this.OnPointerCancelled;
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            this.CommonHandler(sender, TouchActionType.Entered, args);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            this.CommonHandler(sender, TouchActionType.Pressed, args);

            // Check setting of Capture property
            if (this.effect.Capture)
            {
                (sender as FrameworkElement).CapturePointer(args.Pointer);
            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            this.CommonHandler(sender, TouchActionType.Moved, args);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            this.CommonHandler(sender, TouchActionType.Released, args);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            this.CommonHandler(sender, TouchActionType.Exited, args);
        }

        private void OnPointerCancelled(object sender, PointerRoutedEventArgs args)
        {
            this.CommonHandler(sender, TouchActionType.Cancelled, args);
        }

        private void CommonHandler(object sender, TouchActionType touchActionType, PointerRoutedEventArgs args)
        {
            PointerPoint pointerPoint = args.GetCurrentPoint(sender as UIElement);
            Windows.Foundation.Point windowsPoint = pointerPoint.Position;

            this.onTouchAction(
                this.Element,
                new TouchActionEventArgs(
                    args.Pointer.PointerId,
                    touchActionType,
                    new Point(windowsPoint.X, windowsPoint.Y),
                    args.Pointer.IsInContact));
        }
    }
}
