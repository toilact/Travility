using System.Collections.Generic;
using Travility.Core.Models;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface IPlaceService
    {
        IList<Place> Search(PlaceQuery query);
        Place GetById(int placeId);
        IList<Place> GetByCategory(int categoryId);
        IList<Place> GetForMapLayers(IEnumerable<int> categoryIds);
    }
}
