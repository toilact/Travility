using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Travility.Data.Contracts;
using Travility.Data.Model;

namespace Travility.Data.Repositories
{
    public sealed class PlaceRepository : IPlaceRepository
    {
        private readonly TravilityEntities _context;

        public PlaceRepository(TravilityEntities context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Place GetById(int placeId)
        {
            return _context.Places.SingleOrDefault(x => x.PlaceId == placeId);
        }

        public IList<Place> GetByCategory(int categoryId)
        {
            return _context.Places
                .Where(x => x.CategoryId == categoryId)
                .ToList();
        }

        public IList<Place> GetByIds(IList<int> placeIds)
        {
            if (placeIds == null || placeIds.Count == 0)
            {
                return new List<Place>();
            }

            return _context.Places
                .Where(x => placeIds.Contains(x.PlaceId))
                .ToList();
        }

        public void Add(Place place)
        {
            _context.Places.Add(place);
        }

        public void Update(Place place)
        {
            _context.Entry(place).State = EntityState.Modified;
        }
    }
}
