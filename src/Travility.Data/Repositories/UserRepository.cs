using System;
using System.Data.Entity;
using System.Linq;
using Travility.Data.Contracts;
using Travility.Data.Model;

namespace Travility.Data.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly TravilityEntities _context;

        public UserRepository(TravilityEntities context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public User GetById(int id)
        {
            return _context.Users.SingleOrDefault(x => x.UserId == id);
        }

        public User GetByNormalizedUsername(string value)
        {
            return _context.Users.SingleOrDefault(x => x.NormalizedUsername == value);
        }

        public User GetByNormalizedEmail(string value)
        {
            return _context.Users.SingleOrDefault(x => x.NormalizedEmail == value);
        }

        public bool UsernameExists(string value)
        {
            return _context.Users.Any(x => x.NormalizedUsername == value);
        }

        public bool EmailExists(string value)
        {
            return _context.Users.Any(x => x.NormalizedEmail == value);
        }

        public Role GetRoleByName(string value)
        {
            return _context.Roles.SingleOrDefault(x => x.Name == value);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public void AddProfile(UserProfile profile)
        {
            _context.UserProfiles.Add(profile);
        }

        public void AddWallet(TravelWallet wallet)
        {
            _context.TravelWallets.Add(wallet);
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
