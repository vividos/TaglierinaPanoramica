namespace TouchTracking
{
    /// <summary>
    /// Type of touch action
    /// </summary>
    public enum TouchActionType
    {
        /// <summary>
        /// Touch entered the view
        /// </summary>
        Entered,

        /// <summary>
        /// Touch press action
        /// </summary>
        Pressed,

        /// <summary>
        /// Touch point moved
        /// </summary>
        Moved,

        /// <summary>
        /// Touch was released
        /// </summary>
        Released,

        /// <summary>
        /// Touch exited the view
        /// </summary>
        Exited,

        /// <summary>
        /// Touch action was cancelled
        /// </summary>
        Cancelled,
    }
}
