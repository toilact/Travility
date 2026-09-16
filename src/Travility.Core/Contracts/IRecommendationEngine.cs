using System.Collections.Generic;
using Travility.Core.Models;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface IRecommendationEngine
    {
        IList<ScoredPlace> TopK(RecommendationContext ctx, int k);
        decimal Score(Place place, RecommendationContext ctx);
    }
}
