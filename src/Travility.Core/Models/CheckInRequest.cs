using System;

namespace Travility.Core.Models
{
    public class CheckInRequest
    {
        public int TripId { get; set; }
        public int PlaceId { get; set; }
        public int UserId { get; set; }
        public Coordinate Location { get; set; }
        public DateTime CheckInTime { get; set; }
        public string Notes { get; set; }
    }
}
