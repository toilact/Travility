using System;

namespace Travility.Core.Events
{
    public class ItineraryGeneratedEventArgs : EventArgs
    {
        public int TripId { get; set; }
        public int ItineraryId { get; set; }

        public ItineraryGeneratedEventArgs(int tripId, int itineraryId)
        {
            TripId = tripId;
            ItineraryId = itineraryId;
        }
    }
}
