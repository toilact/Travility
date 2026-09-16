namespace Travility.Core.Authentication
{
    public enum AuthenticationErrorCode
    {
        None,
        InvalidCredentials,
        AccountDisabled,
        UsernameAlreadyExists,
        EmailAlreadyExists,
        InvalidUsername,
        InvalidEmail,
        WeakPassword,
        PasswordChangeRequired,
        Forbidden
    }
}
