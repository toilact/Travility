namespace Travility.Core.Authentication
{
    public sealed class RegistrationResult
    {
        private RegistrationResult(bool succeeded, AuthenticationErrorCode errorCode, int userId)
        {
            Succeeded = succeeded;
            ErrorCode = errorCode;
            UserId = userId;
        }

        public bool Succeeded { get; }
        public AuthenticationErrorCode ErrorCode { get; }
        public int UserId { get; }

        public static RegistrationResult Success(int userId)
        {
            return new RegistrationResult(true, AuthenticationErrorCode.None, userId);
        }

        public static RegistrationResult Failure(AuthenticationErrorCode errorCode)
        {
            return new RegistrationResult(false, errorCode, 0);
        }
    }
}
