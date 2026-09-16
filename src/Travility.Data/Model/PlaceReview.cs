using System;

namespace Travility.Data.Model
{
    public partial class PlaceReview
    {
        public int PlaceReviewId { get; set; }
        public int PlaceId { get; set; }
        public int UserId { get; set; }
        public decimal Rating { get; set; }
        public string Content { get; set; }
        public DateTime ReviewedAtUtc { get; set; }
        public bool IsActive { get; set; }

        public virtual Place Place { get; set; }
        public virtual User User { get; set; }
    }
}
