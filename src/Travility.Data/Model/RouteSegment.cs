namespace Travility.Data.Model
{
    public partial class RouteSegment
    {
        public int RouteSegmentId { get; set; }
        public int RoutePlanId { get; set; }
        public int SequenceNumber { get; set; }
        public int FromPlaceId { get; set; }
        public int ToPlaceId { get; set; }
        public double DistanceKm { get; set; }
        public int TravelTimeMinutes { get; set; }
        public decimal TransportCost { get; set; }

        public virtual RoutePlan RoutePlan { get; set; }
        public virtual Place FromPlace { get; set; }
        public virtual Place ToPlace { get; set; }
    }
}
