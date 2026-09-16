namespace Travility.Core.Security
{
    public sealed class PasswordHash
    {
        private readonly byte[] _hash;
        private readonly byte[] _salt;

        public PasswordHash(byte[] hash, byte[] salt, int iterations, string algorithm)
        {
            // Copy cả hai chiều để caller không thể đổi thông tin xác thực sau khi tạo.
            _hash = hash == null ? null : (byte[])hash.Clone();
            _salt = salt == null ? null : (byte[])salt.Clone();
            Iterations = iterations;
            Algorithm = algorithm;
        }

        public byte[] Hash => _hash == null ? null : (byte[])_hash.Clone();
        public byte[] Salt => _salt == null ? null : (byte[])_salt.Clone();
        public int Iterations { get; }
        public string Algorithm { get; }
    }
}
