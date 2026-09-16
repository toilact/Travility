using System;
using Travility.Data.Contracts;

namespace Travility.Tests.TestData
{
    public sealed class FakeTravilityDataSession : ITravilityDataSession
    {
        public FakeUserRepository UsersRepo { get; }

        public IUserRepository Users => UsersRepo;
        public IPlaceRepository Places => null;
        public ITripRepository Trips => null;
        public IBudgetRepository Budgets => null;

        public int BeginCount { get; private set; }
        public int SaveCount { get; private set; }
        public int CommitCount { get; private set; }
        public int RollbackCount { get; private set; }
        public bool IsInTransaction { get; private set; }

        public FakeTravilityDataSession(FakeUserRepository usersRepo)
        {
            UsersRepo = usersRepo ?? throw new ArgumentNullException(nameof(usersRepo));
        }

        public void BeginTransaction()
        {
            if (IsInTransaction)
            {
                throw new InvalidOperationException("Transaction already started.");
            }

            IsInTransaction = true;
            BeginCount++;
        }

        public int SaveChanges()
        {
            SaveCount++;
            return 1;
        }

        public void Commit()
        {
            if (!IsInTransaction)
            {
                throw new InvalidOperationException("No transaction to commit.");
            }

            IsInTransaction = false;
            CommitCount++;
        }

        public void Rollback()
        {
            if (IsInTransaction)
            {
                IsInTransaction = false;
                RollbackCount++;
            }
        }

        public void Dispose()
        {
            if (IsInTransaction)
            {
                Rollback();
            }
        }
    }

    public sealed class FakeTravilityDataSessionFactory : ITravilityDataSessionFactory
    {
        private readonly FakeTravilityDataSession _session;

        public FakeTravilityDataSessionFactory(FakeTravilityDataSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public ITravilityDataSession Create()
        {
            return _session;
        }
    }
}
