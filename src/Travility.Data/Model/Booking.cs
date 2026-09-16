using System;

namespace Travility.Data.Model
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public int TripId { get; set; }
        public int? TransportOptionId { get; set; }
        public int? PlaceId { get; set; }
        public byte BookingType { get; set; }
        public byte BookingStatus { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }
        public byte PricingUnit { get; set; }
        public string ReferenceCode { get; set; }
        public DateTime BookedAtUtc { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual TransportOption TransportOption { get; set; }
        public virtual Place Place { get; set; }
    }
}
