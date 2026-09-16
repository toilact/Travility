namespace Travility.Core.Authentication
{
    public sealed class ChangePasswordResult
    {
        private ChangePasswordResult(bool succeeded, AuthenticationErrorCode errorCode)
        {
            Succeeded = succeeded;
            ErrorCode = errorCode;
        }

        public bool Succeeded { get; }
        public AuthenticationErrorCode ErrorCode { get; }

        public static ChangePasswordResult Success()
        {
            return new ChangePasswordResult(true, AuthenticationErrorCode.None);
        }

        public static ChangePasswordResult Failure(AuthenticationErrorCode errorCode)
        {
            return new ChangePasswordResult(false, errorCode);
        }
    }
}
