namespace Travility.Core.Authentication
{
    public sealed class AuthenticatedUser
    {
        public AuthenticatedUser(int userId, string username, string displayName, string role, bool mustChangePassword)
        {
            UserId = userId;
            Username = username;
            DisplayName = displayName;
            Role = role;
            MustChangePassword = mustChangePassword;
        }

        public int UserId { get; }
        public string Username { get; }
        public string DisplayName { get; }
        public string Role { get; }
        public bool MustChangePassword { get; }
    }
}
