using System;
using Travility.Core.Authentication;

namespace Travility.Core.Session
{
    public sealed class UserSession
    {
        public int UserId { get; }
        public string Username { get; }
        public string DisplayName { get; }
        public string Role { get; }
        public bool MustChangePassword { get; }
        public bool IsAdmin => string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);

        public UserSession(int userId, string username, string displayName, string role, bool mustChangePassword)
        {
            UserId = userId;
            Username = username;
            DisplayName = displayName;
            Role = role;
            MustChangePassword = mustChangePassword;
        }

        public static UserSession FromAuthenticatedUser(AuthenticatedUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return new UserSession(user.UserId, user.Username, user.DisplayName, user.Role, user.MustChangePassword);
        }
    }
}
