using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class ItineraryDay
    {
        public ItineraryDay()
        {
            ItineraryItems = new HashSet<ItineraryItem>();
        }

        public int ItineraryDayId { get; set; }
        public int ItineraryId { get; set; }
        public int DayNumber { get; set; }
        public DateTime Date { get; set; }

        public virtual Itinerary Itinerary { get; set; }
        public virtual RoutePlan RoutePlan { get; set; }
        public virtual ICollection<ItineraryItem> ItineraryItems { get; set; }
    }
}
