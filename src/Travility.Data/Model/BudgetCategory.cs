using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class BudgetCategory
    {
        public BudgetCategory()
        {
            BudgetAllocations = new HashSet<BudgetAllocation>();
            Expenses = new HashSet<Expense>();
        }

        public int BudgetCategoryId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<BudgetAllocation> BudgetAllocations { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
