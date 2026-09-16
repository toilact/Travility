using System;

namespace Travility.Data.Model
{
    public partial class TripMember
    {
        public int TripMemberId { get; set; }
        public int TripId { get; set; }
        public int UserId { get; set; }
        public string MemberRole { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual User User { get; set; }
    }
}
