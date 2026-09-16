using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class RoutePlan
    {
        public RoutePlan()
        {
            RouteSegments = new HashSet<RouteSegment>();
        }

        public int RoutePlanId { get; set; }
        public int ItineraryDayId { get; set; }
        public double TotalDistanceKm { get; set; }
        public int TotalTravelTimeMinutes { get; set; }
        public decimal TotalTransportCost { get; set; }

        public virtual ItineraryDay ItineraryDay { get; set; }
        public virtual ICollection<RouteSegment> RouteSegments { get; set; }
    }
}
