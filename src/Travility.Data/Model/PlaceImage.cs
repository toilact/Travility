using System;

namespace Travility.Data.Model
{
    public partial class PlaceImage
    {
        public int PlaceImageId { get; set; }
        public int PlaceId { get; set; }
        public string ImagePath { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public virtual Place Place { get; set; }
    }
}
