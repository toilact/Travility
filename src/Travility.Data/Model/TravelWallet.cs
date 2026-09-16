using System;

namespace Travility.Data.Model
{
    public partial class TravelWallet
    {
        public int TravelWalletId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual User User { get; set; }
    }
}
