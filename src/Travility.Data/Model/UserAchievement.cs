using System;

namespace Travility.Data.Model
{
    public partial class UserAchievement
    {
        public int UserAchievementId { get; set; }
        public int UserId { get; set; }
        public int AchievementId { get; set; }
        public DateTime AwardedAtUtc { get; set; }

        public virtual User User { get; set; }
        public virtual Achievement Achievement { get; set; }
    }
}
