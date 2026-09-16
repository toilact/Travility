using Travility.Data.Model;

namespace Travility.Core.Models
{
    public sealed class ScoredPlace
    {
        public ScoredPlace()
        {
        }

        public ScoredPlace(Place place, decimal score)
        {
            Place = place;
            Score = score;
        }

        public Place Place { get; set; }
        public decimal Score { get; set; }
    }
}
