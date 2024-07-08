namespace TouchTracking
{
    /// <summary>
    /// Touch effect Forms implementation
    /// </summary>
    public class TouchEffect : RoutingEffect
    {
        /// <summary>
        /// Event handler for touch actions
        /// </summary>
        public event TouchActionEventHandler TouchAction;

        /// <summary>
        /// Creates a new touch effect object
        /// </summary>
        public TouchEffect()
            : base("XamarinDocs.TouchEffect")
        {
        }

        /// <summary>
        /// Indicates if touch actions should capture pointer events
        /// </summary>
        public bool Capture { get; set; }

        /// <summary>
        /// Fires a touch action event
        /// </summary>
        /// <param name="element">element to fire touch action</param>
        /// <param name="args">touch action event args</param>
        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            this.TouchAction?.Invoke(element, args);
        }
    }
}
