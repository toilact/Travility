using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Travility.Data.Contracts;
using Travility.Data.Model;

namespace Travility.Data.Repositories
{
    public sealed class TripRepository : ITripRepository
    {
        private readonly TravilityEntities _context;

        public TripRepository(TravilityEntities context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Trip GetById(int tripId)
        {
            return _context.Trips.SingleOrDefault(x => x.TripId == tripId);
        }

        public IList<Trip> GetByUser(int userId)
        {
            return _context.Trips
                .Where(x => x.UserId == userId)
                .ToList();
        }

        public void Add(Trip trip)
        {
            _context.Trips.Add(trip);
        }

        public void Update(Trip trip)
        {
            _context.Entry(trip).State = EntityState.Modified;
        }
    }
}
