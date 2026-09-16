using System;
using NUnit.Framework;
using Travility.Core.Security;

namespace Travility.Tests.Security
{
    [TestFixture]
    public sealed class LoginIdentifierNormalizerTests
    {
        [TestCase("Thanh", IdentifierKind.Username, "THANH")]
        [TestCase("Thanh@Example.Com", IdentifierKind.Email, "THANH@EXAMPLE.COM")]
        public void Phai_PhanLoaiVaChuanHoa_Khi_NhanIdentifier(string input, IdentifierKind kind, string normalized)
        {
            var result = LoginIdentifierNormalizer.Normalize(input);
            Assert.That(result.Kind, Is.EqualTo(kind));
            Assert.That(result.NormalizedValue, Is.EqualTo(normalized));
        }

        [Test]
        public void Phai_TuChoi_Khi_UsernameDangKyChuaKyTuAt()
        {
            Assert.Throws<ArgumentException>(() => LoginIdentifierNormalizer.NormalizeUsername("abc@def"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Phai_TuChoi_Khi_IdentifierRong(string input)
        {
            Assert.Throws<ArgumentException>(() => LoginIdentifierNormalizer.Normalize(input));
        }

        [TestCase("ab")]
        [TestCase("a_very_long_username_that_exceeds_fifty_characters_limit_by_far")]
        public void Phai_TuChoi_Khi_DoDaiUsernameKhongHopLe(string username)
        {
            Assert.Throws<ArgumentException>(() => LoginIdentifierNormalizer.NormalizeUsername(username));
        }

        [TestCase("invalid-email")]
        [TestCase("@missing-user.com")]
        public void Phai_TuChoi_Khi_EmailKhongHopLe(string email)
        {
            Assert.Throws<ArgumentException>(() => LoginIdentifierNormalizer.NormalizeEmail(email));
        }
    }
}
