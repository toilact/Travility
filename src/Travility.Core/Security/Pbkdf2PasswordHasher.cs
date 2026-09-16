using System;
using System.Security.Cryptography;

namespace Travility.Core.Security
{
    public sealed class Pbkdf2PasswordHasher : IPasswordHasher
    {
        public const string AlgorithmName = "PBKDF2-HMAC-SHA256";
        private const int SaltLength = 16;
        private const int HashLength = 32;
        private readonly int _iterations;

        public Pbkdf2PasswordHasher(int iterations)
        {
            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            _iterations = iterations;
        }

        public PasswordHash Hash(string password)
        {
            if (!IsValidPassword(password))
                throw new ArgumentException("Mật khẩu phải có từ 8 đến 128 ký tự.", nameof(password));

            var salt = new byte[SaltLength];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            return new PasswordHash(Derive(password, salt, _iterations), salt, _iterations, AlgorithmName);
        }

        public bool Verify(string password, PasswordHash stored)
        {
            if (!IsValidPassword(password) || stored == null ||
                !string.Equals(stored.Algorithm, AlgorithmName, StringComparison.Ordinal) ||
                stored.Iterations <= 0)
                return false;

            var salt = stored.Salt;
            var expected = stored.Hash;
            if (salt == null || salt.Length != SaltLength ||
                expected == null || expected.Length != HashLength)
                return false;

            var actual = Derive(password, salt, stored.Iterations);
            var difference = 0;
            // Luôn duyệt đủ 32 byte; không dừng tại byte khác nhau đầu tiên.
            for (var i = 0; i < HashLength; i++)
                difference |= actual[i] ^ expected[i];

            return difference == 0;
        }

        private static byte[] Derive(string password, byte[] salt, int iterations)
        {
            using (var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                return derive.GetBytes(HashLength);
        }

        private static bool IsValidPassword(string password)
        {
            return password != null && password.Length >= 8 && password.Length <= 128;
        }
    }
}
