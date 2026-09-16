using System;

namespace Travility.Data.Model
{
    public partial class DistanceMatrix
    {
        public int DistanceMatrixId { get; set; }
        public int FromPlaceId { get; set; }
        public int ToPlaceId { get; set; }
        public double DistanceKm { get; set; }
        public int EstimatedTravelTimeMinutes { get; set; }
        public DateTime CalculatedAtUtc { get; set; }

        public virtual Place FromPlace { get; set; }
        public virtual Place ToPlace { get; set; }
    }
}
