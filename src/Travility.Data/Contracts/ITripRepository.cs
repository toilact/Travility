using System.Collections.Generic;
using Travility.Data.Model;

namespace Travility.Data.Contracts
{
    public interface ITripRepository
    {
        Trip GetById(int tripId);
        IList<Trip> GetByUser(int userId);
        void Add(Trip trip);
        void Update(Trip trip);
    }
}
