using System;

namespace Travility.Core.Models
{
    public class BookingDraft
    {
        public int TripId { get; set; }
        public int? PlaceId { get; set; }
        public string ServiceType { get; set; }
        public string BookingReference { get; set; }
        public decimal TotalCost { get; set; }
        public string Currency { get; set; } = "VND";
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string Notes { get; set; }
    }
}
