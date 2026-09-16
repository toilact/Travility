using System;
using System.Collections.Generic;
using System.Linq;
using Travility.Data.Contracts;
using Travility.Data.Model;

namespace Travility.Data.Repositories
{
    public sealed class BudgetRepository : IBudgetRepository
    {
        private readonly TravilityEntities _context;

        public BudgetRepository(TravilityEntities context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TripBudget GetByTrip(int tripId)
        {
            return _context.TripBudgets.SingleOrDefault(x => x.TripId == tripId);
        }

        public TravelWallet GetWalletByUser(int userId)
        {
            return _context.TravelWallets.SingleOrDefault(x => x.UserId == userId);
        }

        public IList<Expense> GetExpensesByTrip(int tripId)
        {
            return _context.Expenses
                .Where(x => x.TripId == tripId)
                .ToList();
        }

        public void AddExpense(Expense expense)
        {
            _context.Expenses.Add(expense);
        }
    }
}
