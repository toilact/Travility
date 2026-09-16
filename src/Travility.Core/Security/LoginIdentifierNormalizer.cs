using System;
using System.Net.Mail;

namespace Travility.Core.Security
{
    public static class LoginIdentifierNormalizer
    {
        public static LoginIdentifier Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Login identifier cannot be empty.", nameof(input));
            }

            string trimmed = input.Trim();
            if (trimmed.Contains("@"))
            {
                string normalizedEmail = NormalizeEmail(trimmed);
                return new LoginIdentifier(trimmed, normalizedEmail, IdentifierKind.Email);
            }

            string normalizedUsername = NormalizeUsername(trimmed);
            return new LoginIdentifier(trimmed, normalizedUsername, IdentifierKind.Username);
        }

        public static string NormalizeUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty.", nameof(username));
            }

            string trimmed = username.Trim();
            if (trimmed.Contains("@"))
            {
                throw new ArgumentException("Username cannot contain the '@' symbol.", nameof(username));
            }

            if (trimmed.Length < 3 || trimmed.Length > 50)
            {
                throw new ArgumentException("Username must be between 3 and 50 characters.", nameof(username));
            }

            return trimmed.ToUpperInvariant();
        }

        public static string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be empty.", nameof(email));
            }

            string trimmed = email.Trim();
            if (trimmed.Length > 254)
            {
                throw new ArgumentException("Email cannot exceed 254 characters.", nameof(email));
            }

            try
            {
                var mailAddress = new MailAddress(trimmed);
                return mailAddress.Address.ToUpperInvariant();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid email format.", nameof(email), ex);
            }
        }
    }
}
