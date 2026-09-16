using Travility.Core.Authentication;

namespace Travility.Core.Contracts
{
    public interface IAuthenticationService
    {
        AuthenticationResult Login(string identifier, string password);
        RegistrationResult Register(RegistrationRequest request);
        PasswordResetResult SetTemporaryPassword(int actorUserId, int targetUserId, string temporaryPassword);
        ChangePasswordResult ChangePassword(int userId, string currentPassword, string newPassword);
    }
}
