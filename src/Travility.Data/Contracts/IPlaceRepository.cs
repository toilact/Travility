using System.Collections.Generic;
using Travility.Data.Model;

namespace Travility.Data.Contracts
{
    public interface IPlaceRepository
    {
        Place GetById(int placeId);
        IList<Place> GetByCategory(int categoryId);
        IList<Place> GetByIds(IList<int> placeIds);
        void Add(Place place);
        void Update(Place place);
    }
}
