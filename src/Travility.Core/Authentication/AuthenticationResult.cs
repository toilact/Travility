namespace Travility.Core.Authentication
{
    public sealed class AuthenticationResult
    {
        private AuthenticationResult(bool succeeded, AuthenticationErrorCode errorCode, AuthenticatedUser user)
        {
            Succeeded = succeeded;
            ErrorCode = errorCode;
            User = user;
        }

        public bool Succeeded { get; }
        public AuthenticationErrorCode ErrorCode { get; }
        public AuthenticatedUser User { get; }

        public static AuthenticationResult Success(AuthenticatedUser user)
        {
            return new AuthenticationResult(true, AuthenticationErrorCode.None, user);
        }

        public static AuthenticationResult Failure(AuthenticationErrorCode errorCode)
        {
            return new AuthenticationResult(false, errorCode, null);
        }
    }
}
