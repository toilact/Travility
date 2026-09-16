using System;

namespace Travility.Data.Model
{
    public partial class UserProfile
    {
        public int UserProfileId { get; set; }
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string AvatarPath { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual User User { get; set; }
    }
}
