using System.Collections.Generic;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface IAchievementService
    {
        IList<Achievement> EvaluateForCheckIn(int checkInId);
        IList<UserAchievement> GetByUser(int userId);
    }
}
