namespace Travility.Data.Model
{
    public partial class BudgetAllocation
    {
        public int BudgetAllocationId { get; set; }
        public int TripBudgetId { get; set; }
        public int BudgetCategoryId { get; set; }
        public decimal Ratio { get; set; }
        public decimal AllocatedAmount { get; set; }

        public virtual TripBudget TripBudget { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }
    }
}
