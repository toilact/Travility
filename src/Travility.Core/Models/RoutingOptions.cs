using System;

namespace Travility.Core.Models
{
    public sealed class RoutingOptions
    {
        public int DayNumber { get; set; } = 1;
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan EndTime { get; set; } = new TimeSpan(21, 0, 0);
        public int? StartPlaceId { get; set; }
        public int? EndPlaceId { get; set; }
    }
}
