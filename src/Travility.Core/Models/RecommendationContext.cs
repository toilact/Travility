using System.Collections.Generic;

namespace Travility.Core.Models
{
    public sealed class RecommendationContext
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public decimal TripBudget { get; set; }
        public int PeopleCount { get; set; } = 1;
        public IDictionary<string, byte> UserPreferences { get; set; } = new Dictionary<string, byte>();
        public TravelStyle Style { get; set; } = TravelStyle.Balanced;
    }
}
