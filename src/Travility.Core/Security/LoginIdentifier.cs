namespace Travility.Core.Security
{
    public enum IdentifierKind
    {
        Username,
        Email
    }

    public sealed class LoginIdentifier
    {
        public string RawValue { get; }
        public string NormalizedValue { get; }
        public IdentifierKind Kind { get; }

        public LoginIdentifier(string rawValue, string normalizedValue, IdentifierKind kind)
        {
            RawValue = rawValue;
            NormalizedValue = normalizedValue;
            Kind = kind;
        }
    }
}
