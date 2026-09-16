using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class Achievement
    {
        public Achievement()
        {
            UserAchievements = new HashSet<UserAchievement>();
        }

        public int AchievementId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ConditionType { get; set; }
        public int Threshold { get; set; }
        public int? CategoryId { get; set; }
        public string BadgeImagePath { get; set; }
        public bool IsActive { get; set; }

        public virtual PlaceCategory PlaceCategory { get; set; }
        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
    }
}
