using System;

namespace Travility.Core.Events
{
    public class PlaceRemovedEventArgs : EventArgs
    {
        public int TripId { get; set; }
        public int PlaceId { get; set; }
        public int? DayNumber { get; set; }

        public PlaceRemovedEventArgs(int tripId, int placeId, int? dayNumber = null)
        {
            TripId = tripId;
            PlaceId = placeId;
            DayNumber = dayNumber;
        }
    }
}
