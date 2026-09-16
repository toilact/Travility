using System;

namespace Travility.Data.Model
{
    public partial class ItineraryItem
    {
        public int ItineraryItemId { get; set; }
        public int ItineraryDayId { get; set; }
        public int PlaceId { get; set; }
        public int SequenceNumber { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int VisitDuration { get; set; }
        public decimal PlannedCost { get; set; }

        public virtual ItineraryDay ItineraryDay { get; set; }
        public virtual Place Place { get; set; }
    }
}
