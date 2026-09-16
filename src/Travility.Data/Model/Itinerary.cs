using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class Itinerary
    {
        public Itinerary()
        {
            ItineraryDays = new HashSet<ItineraryDay>();
        }

        public int ItineraryId { get; set; }
        public int TripId { get; set; }
        public byte TravelStyle { get; set; }
        public decimal TotalCost { get; set; }
        public double TotalDistanceKm { get; set; }
        public int TotalTravelTimeMinutes { get; set; }
        public decimal ExperienceScore { get; set; }
        public bool IsSelected { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual ICollection<ItineraryDay> ItineraryDays { get; set; }
    }
}
