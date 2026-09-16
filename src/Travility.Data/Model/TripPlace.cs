using System;

namespace Travility.Data.Model
{
    public partial class TripPlace
    {
        public int TripPlaceId { get; set; }
        public int TripId { get; set; }
        public int PlaceId { get; set; }
        public int AddedByUserId { get; set; }
        public DateTime AddedAtUtc { get; set; }
        public int? PreferredDay { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual Place Place { get; set; }
        public virtual User AddedByUser { get; set; }
    }
}
