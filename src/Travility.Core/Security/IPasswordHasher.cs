namespace Travility.Core.Security
{
    public interface IPasswordHasher
    {
        PasswordHash Hash(string password);
        bool Verify(string password, PasswordHash stored);
    }
}
