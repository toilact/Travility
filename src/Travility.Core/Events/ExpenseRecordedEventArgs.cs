using System;

namespace Travility.Core.Events
{
    public class ExpenseRecordedEventArgs : EventArgs
    {
        public int ExpenseId { get; set; }
        public int TripId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }

        public ExpenseRecordedEventArgs(int expenseId, int tripId, int categoryId, decimal amount)
        {
            ExpenseId = expenseId;
            TripId = tripId;
            CategoryId = categoryId;
            Amount = amount;
        }
    }
}
