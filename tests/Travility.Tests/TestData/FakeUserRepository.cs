using System.Collections.Generic;
using System.Linq;
using Travility.Data.Contracts;
using Travility.Data.Model;

namespace Travility.Tests.TestData
{
    public sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Items { get; } = new List<User>();
        public List<UserProfile> Profiles { get; } = new List<UserProfile>();
        public List<TravelWallet> Wallets { get; } = new List<TravelWallet>();
        public List<Role> Roles { get; } = new List<Role>
        {
            new Role { RoleId = 1, Name = "Admin" },
            new Role { RoleId = 2, Name = "Traveler" }
        };

        public User GetById(int id)
        {
            return Items.SingleOrDefault(x => x.UserId == id);
        }

        public User GetByNormalizedUsername(string value)
        {
            return Items.SingleOrDefault(x => x.NormalizedUsername == value);
        }

        public User GetByNormalizedEmail(string value)
        {
            return Items.SingleOrDefault(x => x.NormalizedEmail == value);
        }

        public bool UsernameExists(string value)
        {
            return Items.Any(x => x.NormalizedUsername == value);
        }

        public bool EmailExists(string value)
        {
            return Items.Any(x => x.NormalizedEmail == value);
        }

        public Role GetRoleByName(string name)
        {
            return Roles.SingleOrDefault(x => x.Name == name);
        }

        public void Add(User user)
        {
            if (user.UserId == 0)
            {
                user.UserId = Items.Count + 1;
            }

            Items.Add(user);
        }

        public void AddProfile(UserProfile profile)
        {
            Profiles.Add(profile);
        }

        public void AddWallet(TravelWallet wallet)
        {
            Wallets.Add(wallet);
        }

        public void Update(User user)
        {
            // In-memory reference is already updated
        }
    }
}
