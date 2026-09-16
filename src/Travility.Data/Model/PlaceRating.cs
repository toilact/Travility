using System;

namespace Travility.Data.Model
{
    public partial class PlaceRating
    {
        public int PlaceRatingId { get; set; }
        public int PlaceId { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime CalculatedAtUtc { get; set; }

        public virtual Place Place { get; set; }
    }
}
