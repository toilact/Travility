using System.Collections.Generic;
using Travility.Data.Model;

namespace Travility.Data.Contracts
{
    public interface IBudgetRepository
    {
        TripBudget GetByTrip(int tripId);
        TravelWallet GetWalletByUser(int userId);
        IList<Expense> GetExpensesByTrip(int tripId);
        void AddExpense(Expense expense);
    }
}
