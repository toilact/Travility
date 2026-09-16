using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class TripBudget
    {
        public TripBudget()
        {
            BudgetAllocations = new HashSet<BudgetAllocation>();
        }

        public int TripBudgetId { get; set; }
        public int TripId { get; set; }
        public decimal Budget { get; set; }
        public decimal PlannedCost { get; set; }
        public decimal ActualSpent { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual ICollection<BudgetAllocation> BudgetAllocations { get; set; }
    }
}
