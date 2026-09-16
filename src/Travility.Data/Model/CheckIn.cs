using System;

namespace Travility.Data.Model
{
    public partial class CheckIn
    {
        public int CheckInId { get; set; }
        public int UserId { get; set; }
        public int? TripId { get; set; }
        public int PlaceId { get; set; }
        public DateTime CheckInTimeUtc { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceFromPlace { get; set; }
        public bool IsSimulated { get; set; }

        public virtual User User { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Place Place { get; set; }
    }
}
