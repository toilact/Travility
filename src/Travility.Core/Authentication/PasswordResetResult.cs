namespace Travility.Core.Authentication
{
    public sealed class PasswordResetResult
    {
        private PasswordResetResult(bool succeeded, AuthenticationErrorCode errorCode)
        {
            Succeeded = succeeded;
            ErrorCode = errorCode;
        }

        public bool Succeeded { get; }
        public AuthenticationErrorCode ErrorCode { get; }

        public static PasswordResetResult Success()
        {
            return new PasswordResetResult(true, AuthenticationErrorCode.None);
        }

        public static PasswordResetResult Failure(AuthenticationErrorCode errorCode)
        {
            return new PasswordResetResult(false, errorCode);
        }
    }
}
