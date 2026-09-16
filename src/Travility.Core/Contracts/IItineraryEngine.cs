using System.Collections.Generic;
using Travility.Core.Models;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface IItineraryEngine
    {
        IList<Itinerary> Generate(Trip trip);
        Itinerary GenerateFor(Trip trip, TravelStyle style);
    }
}
