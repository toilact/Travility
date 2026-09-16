using System;

namespace Travility.Core.Events
{
    public class HotelChangedEventArgs : EventArgs
    {
        public int TripId { get; set; }
        public int? OldHotelPlaceId { get; set; }
        public int NewHotelPlaceId { get; set; }

        public HotelChangedEventArgs(int tripId, int? oldHotelPlaceId, int newHotelPlaceId)
        {
            TripId = tripId;
            OldHotelPlaceId = oldHotelPlaceId;
            NewHotelPlaceId = newHotelPlaceId;
        }
    }
}
