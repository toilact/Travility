using System.Linq;
using NUnit.Framework;
using Travility.Core.Authentication;
using Travility.Tests.TestData;

namespace Travility.Tests.Authentication
{
    [TestFixture]
    public sealed class AuthenticationServiceTests
    {
        [Test]
        public void Phai_DangNhapBangEmail_Khi_EmailVaMatKhauDung()
        {
            var fixture = AuthFixture.CreateTraveler("thanh", "thanh@example.com", "MatKhau@123");
            var result = fixture.Service.Login("THANH@example.com", "MatKhau@123");

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.User.Username, Is.EqualTo("thanh"));
            Assert.That(result.User.DisplayName, Is.EqualTo("thanh"));
            Assert.That(result.User.Role, Is.EqualTo("Traveler"));
        }

        [Test]
        public void Phai_DangNhapBangUsername_Khi_UsernameVaMatKhauDung()
        {
            var fixture = AuthFixture.CreateTraveler("thanh", "thanh@example.com", "MatKhau@123");
            var result = fixture.Service.Login("thanh", "MatKhau@123");

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.User.Username, Is.EqualTo("thanh"));
        }

        [Test]
        public void Phai_TraInvalidCredentials_Khi_MatKhauSai()
        {
            var fixture = AuthFixture.CreateTraveler("thanh", "thanh@example.com", "MatKhau@123");
            var result = fixture.Service.Login("thanh", "SaiMatKhau@123");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(AuthenticationErrorCode.InvalidCredentials));
        }

        [Test]
        public void Phai_TraInvalidCredentials_Khi_UserKhongTonTai()
        {
            var fixture = AuthFixture.Empty();
            var result = fixture.Service.Login("khongtontai", "MatKhau@123");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(AuthenticationErrorCode.InvalidCredentials));
        }

        [Test]
        public void Phai_TaoTravelerVaCommitMotLan_Khi_DangKyHopLe()
        {
            var fixture = AuthFixture.Empty();
            var result = fixture.Service.Register(new RegistrationRequest
            {
                Username = "Thanh",
                Email = "Thanh@Example.com",
                DisplayName = "Đỗ Chí Thành",
                Password = "MatKhau@123"
            });

            Assert.That(result.Succeeded, Is.True);
            Assert.That(fixture.Users.Items.Single().Role.Name, Is.EqualTo("Traveler"));
            Assert.That(fixture.Users.Items.Single().NormalizedUsername, Is.EqualTo("THANH"));
            Assert.That(fixture.Users.Profiles.Count, Is.EqualTo(1));
            Assert.That(fixture.Users.Wallets.Count, Is.EqualTo(1));
            Assert.That(fixture.Session.SaveCount, Is.EqualTo(1));
            Assert.That(fixture.Session.CommitCount, Is.EqualTo(1));
        }

        [TestCase(true, false, AuthenticationErrorCode.UsernameAlreadyExists)]
        [TestCase(false, true, AuthenticationErrorCode.EmailAlreadyExists)]
        public void Phai_KhongCommit_Khi_DinhDanhBiTrung(
            bool duplicateUsername,
            bool duplicateEmail,
            AuthenticationErrorCode expected)
        {
            var fixture = AuthFixture.WithDuplicates(duplicateUsername, duplicateEmail);
            var result = fixture.Service.Register(AuthFixture.ValidRegistration());
            Assert.That(result.ErrorCode, Is.EqualTo(expected));
            Assert.That(fixture.Session.CommitCount, Is.Zero);
        }

        [Test]
        public void Phai_TuChoi_Khi_MatKhauDangKyYeu()
        {
            var fixture = AuthFixture.Empty();
            var req = AuthFixture.ValidRegistration();
            req.Password = "short";

            var result = fixture.Service.Register(req);
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(AuthenticationErrorCode.WeakPassword));
            Assert.That(fixture.Session.CommitCount, Is.Zero);
        }

        [Test]
        public void Phai_TuChoi_Khi_TaiKhoanBiVoHieuHoa()
        {
            var fixture = AuthFixture.CreateTraveler("thanh", "thanh@example.com", "MatKhau@123", isActive: false);
            var result = fixture.Service.Login("thanh", "MatKhau@123");
            Assert.That(result.ErrorCode, Is.EqualTo(AuthenticationErrorCode.AccountDisabled));
        }

        [Test]
        public void Phai_CamTraveler_Khi_DatMatKhauTam()
        {
            var fixture = AuthFixture.WithActorAndTarget(actorRole: "Traveler");
            var result = fixture.Service.SetTemporaryPassword(fixture.ActorId, fixture.TargetId, "TamThoi@123");
            Assert.That(result.ErrorCode, Is.EqualTo(AuthenticationErrorCode.Forbidden));
        }

        [Test]
        public void Phai_BatDoiMatKhau_Khi_AdminDatMatKhauTam()
        {
            var fixture = AuthFixture.WithActorAndTarget(actorRole: "Admin");
            var result = fixture.Service.SetTemporaryPassword(fixture.ActorId, fixture.TargetId, "TamThoi@123");
            Assert.That(result.Succeeded, Is.True);
            Assert.That(fixture.Users.GetById(fixture.TargetId).MustChangePassword, Is.True);
            Assert.That(fixture.Session.CommitCount, Is.EqualTo(1));
        }

        [Test]
        public void Phai_TatCoDoiMatKhau_Khi_DoiMatKhauThanhCong()
        {
            var fixture = AuthFixture.CreateTraveler("thanh", "thanh@example.com", "MatKhau@123", mustChangePassword: true);
            var result = fixture.Service.ChangePassword(fixture.TargetId, "MatKhau@123", "MatKhauMoi@123");
            Assert.That(result.Succeeded, Is.True);
            Assert.That(fixture.Users.GetById(fixture.TargetId).MustChangePassword, Is.False);
            Assert.That(fixture.Session.CommitCount, Is.EqualTo(1));
        }
    }
}
