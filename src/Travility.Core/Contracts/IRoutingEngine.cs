using System.Collections.Generic;
using Travility.Core.Models;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface IRoutingEngine
    {
        RoutePlan Optimise(IList<Place> places, RoutingOptions options);
        double Distance(Place a, Place b);
        double[,] BuildMatrix(IList<Place> places);
    }
}
