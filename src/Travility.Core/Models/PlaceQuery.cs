using System.Collections.Generic;

namespace Travility.Core.Models
{
    public sealed class PlaceQuery
    {
        public string Keyword { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? NearLatitude { get; set; }
        public double? NearLongitude { get; set; }
        public double? MaxDistanceKm { get; set; }
        public IList<string> PreferenceTags { get; set; } = new List<string>();
    }
}
