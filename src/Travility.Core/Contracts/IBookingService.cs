using System.Collections.Generic;
using Travility.Core.Models;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface IBookingService
    {
        Booking Create(BookingDraft draft);
        IList<Booking> GetByTrip(int tripId);
        void UpdateStatus(int bookingId, BookingStatus status, int actorUserId);
    }
}
