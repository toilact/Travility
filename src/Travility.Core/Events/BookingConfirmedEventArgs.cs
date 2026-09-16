using System;

namespace Travility.Core.Events
{
    public class BookingConfirmedEventArgs : EventArgs
    {
        public int BookingId { get; set; }
        public int TripId { get; set; }
        public decimal TotalCost { get; set; }

        public BookingConfirmedEventArgs(int bookingId, int tripId, decimal totalCost)
        {
            BookingId = bookingId;
            TripId = tripId;
            TotalCost = totalCost;
        }
    }
}
