using System;

namespace Travility.Data.Model
{
    public partial class TripPreference
    {
        public int TripPreferenceId { get; set; }
        public int TripId { get; set; }
        public byte Beach { get; set; }
        public byte Food { get; set; }
        public byte History { get; set; }
        public byte Culture { get; set; }
        public byte Entertainment { get; set; }
        public byte Shopping { get; set; }
        public byte Nature { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual Trip Trip { get; set; }
    }
}
