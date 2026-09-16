using System;

namespace Travility.Data.Model
{
    public partial class ExternalPlaceMapping
    {
        public int ExternalPlaceMappingId { get; set; }
        public int PlaceId { get; set; }
        public string ProviderName { get; set; }
        public string ExternalPlaceId { get; set; }
        public DateTime LastSyncedAtUtc { get; set; }

        public virtual Place Place { get; set; }
    }
}
