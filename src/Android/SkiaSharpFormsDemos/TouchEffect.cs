using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

#pragma warning disable SA1516 // Elements should be separated by blank line
#pragma warning disable SA1600 // Elements should be documented

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchTracking.Droid.TouchEffect), "TouchEffect")]

namespace TouchTracking.Droid
{
    /// <summary>
    /// Touch effect implementation for Android
    /// </summary>
    public class TouchEffect : PlatformEffect
    {
        private readonly int[] twoIntArray = new int[2];
        private Android.Views.View view;
        private Element formsElement;
        private TouchTracking.TouchEffect libTouchEffect;
        private bool capture;
        private Func<double, double> fromPixels;
        private static readonly Dictionary<Android.Views.View, TouchEffect> ViewDictionary =
            new Dictionary<Android.Views.View, TouchEffect>();
        private static readonly Dictionary<int, TouchEffect> IdToEffectDictionary =
            new Dictionary<int, TouchEffect>();

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            this.view = this.Control ?? this.Container;

            // Get access to the TouchEffect class in the .NET Standard library
            TouchTracking.TouchEffect touchEffect =
                (TouchTracking.TouchEffect)this.Element.Effects.
                    FirstOrDefault(e => e is TouchTracking.TouchEffect);

            if (touchEffect != null && this.view != null)
            {
                ViewDictionary.Add(this.view, this);

                this.formsElement = this.Element;

                this.libTouchEffect = touchEffect;

                // Save fromPixels function
                this.fromPixels = this.view.Context.FromPixels;

                // Set event handler on View
                this.view.Touch += this.OnTouch;
            }
        }

        protected override void OnDetached()
        {
            if (ViewDictionary.ContainsKey(this.view))
            {
                ViewDictionary.Remove(this.view);
                this.view.Touch -= this.OnTouch;
            }
        }

        private void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            // Two object common to all the events
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            // Get the pointer index
            int pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            int id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(this.twoIntArray);
            Point screenPointerCoords = new Point(
                this.twoIntArray[0] + motionEvent.GetX(pointerIndex),
                this.twoIntArray[1] + motionEvent.GetY(pointerIndex));

            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    this.FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);

                    IdToEffectDictionary.Add(id, this);

                    this.capture = this.libTouchEffect.Capture;
                    break;

                case MotionEventActions.Move:
                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (this.capture)
                        {
                            senderView.GetLocationOnScreen(this.twoIntArray);

                            screenPointerCoords = new Point(
                                this.twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                this.twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            this.FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            this.CheckForBoundaryHop(id, screenPointerCoords);

                            if (IdToEffectDictionary[id] != null)
                            {
                                this.FireEvent(IdToEffectDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                            }
                        }
                    }

                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (this.capture)
                    {
                        this.FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        this.CheckForBoundaryHop(id, screenPointerCoords);

                        if (IdToEffectDictionary[id] != null)
                        {
                            this.FireEvent(IdToEffectDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                        }
                    }

                    IdToEffectDictionary.Remove(id);
                    break;

                case MotionEventActions.Cancel:
                    if (this.capture)
                    {
                        this.FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (IdToEffectDictionary[id] != null)
                        {
                            this.FireEvent(IdToEffectDictionary[id], id, TouchActionType.Cancelled, screenPointerCoords, false);
                        }
                    }

                    IdToEffectDictionary.Remove(id);
                    break;
            }
        }

        private void CheckForBoundaryHop(int id, Point pointerLocation)
        {
            TouchEffect touchEffectHit = null;

            foreach (Android.Views.View viewToCheck in ViewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    viewToCheck.GetLocationOnScreen(this.twoIntArray);
                }
                catch
                {
                    // System.ObjectDisposedException: Cannot access a disposed object.
                    continue;
                }

                Rectangle viewRect = new Rectangle(this.twoIntArray[0], this.twoIntArray[1], viewToCheck.Width, viewToCheck.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = ViewDictionary[viewToCheck];
                }
            }

            if (touchEffectHit != IdToEffectDictionary[id])
            {
                if (IdToEffectDictionary[id] != null)
                {
                    this.FireEvent(IdToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }

                if (touchEffectHit != null)
                {
                    this.FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }

                IdToEffectDictionary[id] = touchEffectHit;
            }
        }

        private void FireEvent(TouchEffect touchEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect.libTouchEffect.OnTouchAction;

            // Get the location of the pointer within the view
            touchEffect.view.GetLocationOnScreen(this.twoIntArray);
            double x = pointerLocation.X - this.twoIntArray[0];
            double y = pointerLocation.Y - this.twoIntArray[1];
            Point point = new Point(this.fromPixels(x), this.fromPixels(y));

            // Call the method
            onTouchAction(
                touchEffect.formsElement,
                new TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}
