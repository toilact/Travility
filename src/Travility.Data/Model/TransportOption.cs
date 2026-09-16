using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class TransportOption
    {
        public TransportOption()
        {
            Bookings = new HashSet<Booking>();
        }

        public int TransportOptionId { get; set; }
        public byte BookingType { get; set; }
        public string ProviderName { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartAt { get; set; }
        public DateTime ArriveAt { get; set; }
        public decimal UnitPrice { get; set; }
        public byte PricingUnit { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
