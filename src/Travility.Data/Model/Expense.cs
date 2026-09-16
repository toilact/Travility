using System;

namespace Travility.Data.Model
{
    public partial class Expense
    {
        public int ExpenseId { get; set; }
        public int TripId { get; set; }
        public int UserId { get; set; }
        public int BudgetCategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime SpentAtUtc { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual User User { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }
    }
}
