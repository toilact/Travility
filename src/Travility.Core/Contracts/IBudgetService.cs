using System.Collections.Generic;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface IBudgetService
    {
        decimal GetPlannedCost(int tripId);
        decimal GetActualSpent(int tripId);
        decimal GetWalletBalance(int userId);
        IList<BudgetAllocation> Allocate(int tripId, IDictionary<BudgetCategory, decimal> ratios);
        void RecordExpense(Expense expense);
        bool ExceedsWallet(int userId, decimal additionalBudget);
    }
}
