using System.Collections.Generic;

namespace Travility.Core.Models
{
    public class CheckInResult
    {
        public bool Succeeded { get; set; }
        public int? CheckInId { get; set; }
        public string Message { get; set; }
        public double DistanceMeters { get; set; }
        public bool IsWithinRange { get; set; }
        public IList<int> UnlockedAchievementIds { get; set; } = new List<int>();
    }
}
