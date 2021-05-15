using System;
using Xamarin.Forms;

namespace TouchTracking
{
    /// <summary>
    /// Event arguments for touch action
    /// </summary>
    public class TouchActionEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new touch action
        /// </summary>
        /// <param name="id">touch ID</param>
        /// <param name="type">touch action type</param>
        /// <param name="location">touch location</param>
        /// <param name="isInContact">true when in contact</param>
        public TouchActionEventArgs(long id, TouchActionType type, Point location, bool isInContact)
        {
            this.Id = id;
            this.Type = type;
            this.Location = location;
            this.IsInContact = isInContact;
        }

        /// <summary>
        /// Touch ID
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Touch action type
        /// </summary>
        public TouchActionType Type { get; private set; }

        /// <summary>
        /// Touch location
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// True when in contact
        /// </summary>
        public bool IsInContact { get; private set; }
    }
}
