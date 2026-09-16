using System;
using Travility.Core.Authentication;
using Travility.Core.Security;
using Travility.Core.Services;
using Travility.Data.Model;

namespace Travility.Tests.TestData
{
    public sealed class AuthFixture
    {
        public AuthenticationService Service { get; }
        public FakeTravilityDataSession Session { get; }
        public FakeUserRepository Users { get; }
        public IPasswordHasher Hasher { get; }
        public int ActorId { get; set; }
        public int TargetId { get; set; }

        public AuthFixture(FakeUserRepository users, FakeTravilityDataSession session, IPasswordHasher hasher)
        {
            Users = users;
            Session = session;
            Hasher = hasher;
            Service = new AuthenticationService(new FakeTravilityDataSessionFactory(Session), Hasher);
        }

        public static AuthFixture Empty()
        {
            var users = new FakeUserRepository();
            var session = new FakeTravilityDataSession(users);
            var hasher = new Pbkdf2PasswordHasher(1000);
            return new AuthFixture(users, session, hasher);
        }

        public static RegistrationRequest ValidRegistration()
        {
            return new RegistrationRequest
            {
                Username = "thanh",
                Email = "thanh@example.com",
                DisplayName = "Đỗ Chí Thành",
                Password = "MatKhau@123"
            };
        }

        public static AuthFixture CreateTraveler(
            string username,
            string email,
            string password,
            bool isActive = true,
            bool mustChangePassword = false)
        {
            var fixture = Empty();
            var hash = fixture.Hasher.Hash(password);
            var travelerRole = fixture.Users.GetRoleByName("Traveler");

            var user = new User
            {
                UserId = 1,
                Username = username,
                NormalizedUsername = username.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                DisplayName = username,
                RoleId = travelerRole.RoleId,
                Role = travelerRole,
                PasswordHash = hash.HashBytes,
                PasswordSalt = hash.SaltBytes,
                PasswordIterations = hash.Iterations,
                PasswordAlgorithm = hash.Algorithm,
                IsActive = isActive,
                MustChangePassword = mustChangePassword,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            fixture.Users.Add(user);
            fixture.TargetId = user.UserId;
            return fixture;
        }

        public static AuthFixture WithDuplicates(bool duplicateUsername, bool duplicateEmail)
        {
            var fixture = Empty();
            var travelerRole = fixture.Users.GetRoleByName("Traveler");
            var hash = fixture.Hasher.Hash("DummyPass@123");

            if (duplicateUsername)
            {
                fixture.Users.Add(new User
                {
                    UserId = 1,
                    Username = "thanh",
                    NormalizedUsername = "THANH",
                    Email = "other@example.com",
                    NormalizedEmail = "OTHER@EXAMPLE.COM",
                    DisplayName = "Other",
                    RoleId = travelerRole.RoleId,
                    Role = travelerRole,
                    PasswordHash = hash.HashBytes,
                    PasswordSalt = hash.SaltBytes,
                    PasswordIterations = hash.Iterations,
                    PasswordAlgorithm = hash.Algorithm,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                });
            }

            if (duplicateEmail)
            {
                fixture.Users.Add(new User
                {
                    UserId = duplicateUsername ? 2 : 1,
                    Username = "otheruser",
                    NormalizedUsername = "OTHERUSER",
                    Email = "thanh@example.com",
                    NormalizedEmail = "THANH@EXAMPLE.COM",
                    DisplayName = "Other",
                    RoleId = travelerRole.RoleId,
                    Role = travelerRole,
                    PasswordHash = hash.HashBytes,
                    PasswordSalt = hash.SaltBytes,
                    PasswordIterations = hash.Iterations,
                    PasswordAlgorithm = hash.Algorithm,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                });
            }

            return fixture;
        }

        public static AuthFixture WithActorAndTarget(string actorRole)
        {
            var fixture = Empty();
            var role = fixture.Users.GetRoleByName(actorRole);
            var travelerRole = fixture.Users.GetRoleByName("Traveler");
            var hash = fixture.Hasher.Hash("MatKhau@123");

            var actor = new User
            {
                UserId = 10,
                Username = "actor",
                NormalizedUsername = "ACTOR",
                Email = "actor@example.com",
                NormalizedEmail = "ACTOR@EXAMPLE.COM",
                DisplayName = "Actor",
                RoleId = role.RoleId,
                Role = role,
                PasswordHash = hash.HashBytes,
                PasswordSalt = hash.SaltBytes,
                PasswordIterations = hash.Iterations,
                PasswordAlgorithm = hash.Algorithm,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            var target = new User
            {
                UserId = 20,
                Username = "target",
                NormalizedUsername = "TARGET",
                Email = "target@example.com",
                NormalizedEmail = "TARGET@EXAMPLE.COM",
                DisplayName = "Target",
                RoleId = travelerRole.RoleId,
                Role = travelerRole,
                PasswordHash = hash.HashBytes,
                PasswordSalt = hash.SaltBytes,
                PasswordIterations = hash.Iterations,
                PasswordAlgorithm = hash.Algorithm,
                IsActive = true,
                MustChangePassword = false,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            fixture.Users.Add(actor);
            fixture.Users.Add(target);
            fixture.ActorId = actor.UserId;
            fixture.TargetId = target.UserId;
            return fixture;
        }
    }
}
