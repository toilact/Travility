using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Travility.Core.Security;

namespace Travility.Tests.Security
{
    [TestFixture]
    public sealed class Pbkdf2PasswordHasherTests
    {
        [Test]
        public void Phai_TaoHaiHashKhacNhau_Khi_CungMatKhau()
        {
            var hasher = new Pbkdf2PasswordHasher(600000);
            var first = hasher.Hash("MatKhau@123");
            var second = hasher.Hash("MatKhau@123");

            CollectionAssert.AreNotEqual(first.Salt, second.Salt);
            CollectionAssert.AreNotEqual(first.Hash, second.Hash);
        }

        [TestCase("MatKhau@123", true)]
        [TestCase("SaiMatKhau", false)]
        public void Phai_XacMinhDung_Khi_SoSanhMatKhau(string candidate, bool expected)
        {
            var hasher = new Pbkdf2PasswordHasher(600000);
            var stored = hasher.Hash("MatKhau@123");

            Assert.That(hasher.Verify(candidate, stored), Is.EqualTo(expected));
        }

        [Test]
        public void Phai_LuuThongSoDayDu_Khi_TaoHashBaseline()
        {
            var hasher = new Pbkdf2PasswordHasher(600000);
            var stored = hasher.Hash("MatKhau@123");

            Assert.Multiple(() =>
            {
                Assert.That(stored.Algorithm, Is.EqualTo("PBKDF2-HMAC-SHA256"));
                Assert.That(stored.Iterations, Is.EqualTo(600000));
                Assert.That(stored.Salt, Has.Length.EqualTo(16));
                Assert.That(stored.Hash, Has.Length.EqualTo(32));
            });
        }

        [Test]
        public void Phai_XacMinhHashThamChieuCu_Khi_IterationKhacCauHinhHienTai()
        {
            // Fixture tính độc lập bằng Node.js crypto.pbkdf2Sync (SHA-256).
            var stored = LegacyHash();
            var hasher = new Pbkdf2PasswordHasher(600000);

            Assert.That(hasher.Verify("password", stored), Is.True);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Phai_TuChoiCauHinh_Khi_IterationKhongDuong(int iterations)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pbkdf2PasswordHasher(iterations));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("1234567")]
        public void Phai_TuChoiTaoHash_Khi_MatKhauQuaNgan(string password)
        {
            var hasher = new Pbkdf2PasswordHasher(600000);

            Assert.Throws<ArgumentException>(() => hasher.Hash(password));
        }

        [Test]
        public void Phai_TuChoiTaoHash_Khi_MatKhauQuaDai()
        {
            var hasher = new Pbkdf2PasswordHasher(600000);

            Assert.Throws<ArgumentException>(() => hasher.Hash(new string('a', 129)));
        }

        [TestCase(8)]
        [TestCase(128)]
        public void Phai_ChapNhanMatKhau_Khi_DoDaiONgayBien(int length)
        {
            var hasher = new Pbkdf2PasswordHasher(10000);
            var password = new string('a', length);

            Assert.That(hasher.Verify(password, hasher.Hash(password)), Is.True);
        }

        [Test]
        public void Phai_GiuNguyenKhoangTrangVaUnicode_Khi_HashMatKhau()
        {
            var hasher = new Pbkdf2PasswordHasher(10000);
            var stored = hasher.Hash("  MậtKhẩu@123  ");

            Assert.That(hasher.Verify("  MậtKhẩu@123  ", stored), Is.True);
            Assert.That(hasher.Verify("MậtKhẩu@123", stored), Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("1234567")]
        public void Phai_TraFalse_Khi_XacMinhMatKhauQuaNgan(string password)
        {
            var hasher = new Pbkdf2PasswordHasher(600000);

            Assert.That(hasher.Verify(password, LegacyHash()), Is.False);
        }

        [Test]
        public void Phai_TraFalse_Khi_XacMinhMatKhauQuaDai()
        {
            var hasher = new Pbkdf2PasswordHasher(600000);

            Assert.That(hasher.Verify(new string('a', 129), LegacyHash()), Is.False);
        }

        [TestCaseSource(nameof(MalformedHashes))]
        public void Phai_TraFalse_Khi_HashLuuBiLoi(PasswordHash stored)
        {
            var hasher = new Pbkdf2PasswordHasher(600000);

            Assert.That(hasher.Verify("password", stored), Is.False);
        }

        [TestCase(0)]
        [TestCase(15)]
        [TestCase(31)]
        public void Phai_TraFalse_Khi_MotByteHashKhacNhau(int index)
        {
            var original = LegacyHash();
            var tampered = original.Hash;
            tampered[index] ^= 1;
            var stored = new PasswordHash(tampered, original.Salt, original.Iterations, original.Algorithm);
            var hasher = new Pbkdf2PasswordHasher(600000);

            Assert.That(hasher.Verify("password", stored), Is.False);
        }

        [Test]
        public void Phai_GiuHashBatBien_Khi_SuaMangTruyenVao()
        {
            var original = LegacyHash();
            var hash = original.Hash;
            var salt = original.Salt;
            var stored = new PasswordHash(hash, salt, original.Iterations, original.Algorithm);
            hash[0] ^= 1;
            salt[0] ^= 1;

            Assert.That(new Pbkdf2PasswordHasher(600000).Verify("password", stored), Is.True);
        }

        [Test]
        public void Phai_GiuHashBatBien_Khi_SuaMangDocRa()
        {
            var stored = LegacyHash();
            stored.Hash[0] ^= 1;
            stored.Salt[0] ^= 1;

            Assert.That(new Pbkdf2PasswordHasher(600000).Verify("password", stored), Is.True);
        }

        private static PasswordHash LegacyHash()
        {
            return new PasswordHash(
                Convert.FromBase64String("fvWoYEIJqT7C7d27uj82Ytr0nDi8+Mu5sOUXfNf1EU4="),
                Encoding.UTF8.GetBytes("1234567890abcdef"),
                10000,
                "PBKDF2-HMAC-SHA256");
        }

        private static IEnumerable<TestCaseData> MalformedHashes()
        {
            yield return new TestCaseData(new object[] { null }).SetName("HashNull");
            yield return new TestCaseData(new PasswordHash(null, new byte[16], 10000, "PBKDF2-HMAC-SHA256")).SetName("DerivedKeyNull");
            yield return new TestCaseData(new PasswordHash(new byte[32], null, 10000, "PBKDF2-HMAC-SHA256")).SetName("SaltNull");
            foreach (var length in new[] { 0, 31, 33 })
                yield return new TestCaseData(new PasswordHash(new byte[length], new byte[16], 10000, "PBKDF2-HMAC-SHA256")).SetName("DerivedKeyLength" + length);
            foreach (var length in new[] { 0, 15, 17 })
                yield return new TestCaseData(new PasswordHash(new byte[32], new byte[length], 10000, "PBKDF2-HMAC-SHA256")).SetName("SaltLength" + length);
            foreach (var iterations in new[] { 0, -1 })
                yield return new TestCaseData(new PasswordHash(new byte[32], new byte[16], iterations, "PBKDF2-HMAC-SHA256")).SetName("Iterations" + iterations);
            foreach (var algorithm in new[] { null, "", "PBKDF2-HMAC-SHA1", "pbkdf2-hmac-sha256" })
                yield return new TestCaseData(new PasswordHash(new byte[32], new byte[16], 10000, algorithm)).SetName("Algorithm_" + (algorithm ?? "null"));
        }
    }
}
