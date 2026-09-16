namespace Travility.Core.Authentication
{
    public sealed class RegistrationRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
    }
}
