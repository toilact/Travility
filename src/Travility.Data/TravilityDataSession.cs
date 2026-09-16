using System;
using System.Data.Entity;
using Travility.Data.Contracts;
using Travility.Data.Model;
using Travility.Data.Repositories;

namespace Travility.Data
{
    public sealed class TravilityDataSession : ITravilityDataSession
    {
        private readonly TravilityEntities _context;
        private DbContextTransaction _transaction;
        private bool _disposed;

        private IUserRepository _users;
        private IPlaceRepository _places;
        private ITripRepository _trips;
        private IBudgetRepository _budgets;

        public TravilityDataSession(TravilityEntities context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUserRepository Users
        {
            get { return _users ?? (_users = new UserRepository(_context)); }
        }

        public IPlaceRepository Places
        {
            get { return _places ?? (_places = new PlaceRepository(_context)); }
        }

        public ITripRepository Trips
        {
            get { return _trips ?? (_trips = new TripRepository(_context)); }
        }

        public IBudgetRepository Budgets
        {
            get { return _budgets ?? (_budgets = new BudgetRepository(_context)); }
        }

        public void BeginTransaction()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already active on this data session.");
            }

            _transaction = _context.Database.BeginTransaction();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Commit()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            try
            {
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Rollback()
        {
            if (_transaction == null)
            {
                return;
            }

            try
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_transaction != null)
            {
                try
                {
                    _transaction.Rollback();
                }
                catch
                {
                    // Ignore exception on rollback during disposal
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            _context.Dispose();
            _disposed = true;
        }
    }
}
