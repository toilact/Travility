using System.Collections.Generic;
using Travility.Core.Models;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface ITripService
    {
        Trip Create(TripDraft draft);
        Trip GetById(int tripId);
        IList<Trip> GetByUser(int userId);
        void AddPlace(int tripId, int placeId);
        void RemovePlace(int tripId, int placeId);
        void Update(Trip trip);
    }
}
