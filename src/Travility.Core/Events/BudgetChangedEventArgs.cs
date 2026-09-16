using System;

namespace Travility.Core.Events
{
    public class BudgetChangedEventArgs : EventArgs
    {
        public int TripId { get; set; }
        public decimal NewTotalBudget { get; set; }

        public BudgetChangedEventArgs(int tripId, decimal newTotalBudget)
        {
            TripId = tripId;
            NewTotalBudget = newTotalBudget;
        }
    }
}
