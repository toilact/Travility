using System;

namespace Travility.Data.Contracts
{
    public interface ITravilityDataSession : IDisposable
    {
        IUserRepository Users { get; }
        IPlaceRepository Places { get; }
        ITripRepository Trips { get; }
        IBudgetRepository Budgets { get; }
        void BeginTransaction();
        int SaveChanges();
        void Commit();
        void Rollback();
    }
}
