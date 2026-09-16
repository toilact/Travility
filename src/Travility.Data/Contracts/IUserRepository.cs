using Travility.Data.Model;

namespace Travility.Data.Contracts
{
    public interface IUserRepository
    {
        User GetById(int userId);
        User GetByNormalizedUsername(string normalizedUsername);
        User GetByNormalizedEmail(string normalizedEmail);
        bool UsernameExists(string normalizedUsername);
        bool EmailExists(string normalizedEmail);
        Role GetRoleByName(string roleName);
        void Add(User user);
        void AddProfile(UserProfile profile);
        void AddWallet(TravelWallet wallet);
        void Update(User user);
    }
}
