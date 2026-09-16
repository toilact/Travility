using System;
using System.Collections.Generic;

namespace Travility.Core.Events
{
    public class CheckInCompletedEventArgs : EventArgs
    {
        public int CheckInId { get; set; }
        public int TripId { get; set; }
        public int PlaceId { get; set; }
        public int UserId { get; set; }
        public IList<int> UnlockedAchievementIds { get; set; }

        public CheckInCompletedEventArgs(int checkInId, int tripId, int placeId, int userId, IList<int> unlockedAchievementIds = null)
        {
            CheckInId = checkInId;
            TripId = tripId;
            PlaceId = placeId;
            UserId = userId;
            UnlockedAchievementIds = unlockedAchievementIds ?? new List<int>();
        }
    }
}
