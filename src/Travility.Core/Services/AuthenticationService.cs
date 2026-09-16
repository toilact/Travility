using System;
using Travility.Core.Authentication;
using Travility.Core.Contracts;
using Travility.Core.Security;
using Travility.Data.Contracts;
using Travility.Data.Model;

namespace Travility.Core.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ITravilityDataSessionFactory _sessionFactory;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAppLogger _logger;

        public AuthenticationService(
            ITravilityDataSessionFactory sessionFactory,
            IPasswordHasher passwordHasher,
            IAppLogger logger = null)
        {
            _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _logger = logger;
        }

        public RegistrationResult Register(RegistrationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string normalizedUsername;
            try
            {
                normalizedUsername = LoginIdentifierNormalizer.NormalizeUsername(request.Username);
            }
            catch
            {
                return RegistrationResult.Failure(AuthenticationErrorCode.InvalidUsername);
            }

            string normalizedEmail;
            try
            {
                normalizedEmail = LoginIdentifierNormalizer.NormalizeEmail(request.Email);
            }
            catch
            {
                return RegistrationResult.Failure(AuthenticationErrorCode.InvalidEmail);
            }

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8 || request.Password.Length > 128)
            {
                return RegistrationResult.Failure(AuthenticationErrorCode.WeakPassword);
            }

            using (var session = _sessionFactory.Create())
            {
                if (session.Users.UsernameExists(normalizedUsername))
                {
                    return RegistrationResult.Failure(AuthenticationErrorCode.UsernameAlreadyExists);
                }

                if (session.Users.EmailExists(normalizedEmail))
                {
                    return RegistrationResult.Failure(AuthenticationErrorCode.EmailAlreadyExists);
                }

                session.BeginTransaction();
                try
                {
                    var hash = _passwordHasher.Hash(request.Password);
                    var travelerRole = session.Users.GetRoleByName("Traveler");
                    var now = DateTime.UtcNow;

                    var user = new User
                    {
                        Username = request.Username.Trim(),
                        NormalizedUsername = normalizedUsername,
                        Email = request.Email.Trim(),
                        NormalizedEmail = normalizedEmail,
                        DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? request.Username.Trim() : request.DisplayName.Trim(),
                        RoleId = travelerRole != null ? travelerRole.RoleId : 2,
                        Role = travelerRole,
                        PasswordHash = hash.HashBytes,
                        PasswordSalt = hash.SaltBytes,
                        PasswordIterations = hash.Iterations,
                        PasswordAlgorithm = hash.Algorithm,
                        IsActive = true,
                        MustChangePassword = false,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    };

                    session.Users.Add(user);

                    var profile = new UserProfile
                    {
                        UserId = user.UserId,
                        User = user,
                        FullName = user.DisplayName,
                        CreatedAtUtc = now
                    };
                    session.Users.AddProfile(profile);

                    var wallet = new TravelWallet
                    {
                        UserId = user.UserId,
                        User = user,
                        Balance = 0m,
                        Currency = "VND",
                        UpdatedAtUtc = now
                    };
                    session.Users.AddWallet(wallet);

                    session.SaveChanges();
                    session.Commit();

                    _logger?.Info("AuthenticationService", string.Format("User {0} registered successfully.", user.Username));
                    return RegistrationResult.Success(user.UserId);
                }
                catch
                {
                    session.Rollback();
                    throw;
                }
            }
        }

        public AuthenticationResult Login(string identifier, string password)
        {
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrEmpty(password))
            {
                return AuthenticationResult.Failure(AuthenticationErrorCode.InvalidCredentials);
            }

            LoginIdentifier loginId;
            try
            {
                loginId = LoginIdentifierNormalizer.Normalize(identifier);
            }
            catch
            {
                return AuthenticationResult.Failure(AuthenticationErrorCode.InvalidCredentials);
            }

            using (var session = _sessionFactory.Create())
            {
                User user = null;
                if (loginId.Kind == IdentifierKind.Email)
                {
                    user = session.Users.GetByNormalizedEmail(loginId.NormalizedValue);
                }
                else
                {
                    user = session.Users.GetByNormalizedUsername(loginId.NormalizedValue);
                }

                if (user == null)
                {
                    return AuthenticationResult.Failure(AuthenticationErrorCode.InvalidCredentials);
                }

                if (!user.IsActive)
                {
                    return AuthenticationResult.Failure(AuthenticationErrorCode.AccountDisabled);
                }

                var storedHash = new PasswordHash(
                    user.PasswordHash,
                    user.PasswordSalt,
                    user.PasswordIterations,
                    user.PasswordAlgorithm);

                if (!_passwordHasher.Verify(password, storedHash))
                {
                    return AuthenticationResult.Failure(AuthenticationErrorCode.InvalidCredentials);
                }

                var authUser = new AuthenticatedUser(
                    user.UserId,
                    user.Username,
                    user.DisplayName,
                    user.Role != null ? user.Role.Name : string.Empty,
                    user.MustChangePassword);

                return AuthenticationResult.Success(authUser);
            }
        }

        public PasswordResetResult SetTemporaryPassword(int actorUserId, int targetUserId, string temporaryPassword)
        {
            if (string.IsNullOrEmpty(temporaryPassword) || temporaryPassword.Length < 8 || temporaryPassword.Length > 128)
            {
                return PasswordResetResult.Failure(AuthenticationErrorCode.WeakPassword);
            }

            using (var session = _sessionFactory.Create())
            {
                session.BeginTransaction();
                try
                {
                    var actor = session.Users.GetById(actorUserId);
                    if (actor == null || actor.Role == null || actor.Role.Name != "Admin")
                    {
                        session.Rollback();
                        return PasswordResetResult.Failure(AuthenticationErrorCode.Forbidden);
                    }

                    var target = session.Users.GetById(targetUserId);
                    if (target == null)
                    {
                        session.Rollback();
                        return PasswordResetResult.Failure(AuthenticationErrorCode.InvalidCredentials);
                    }

                    var hash = _passwordHasher.Hash(temporaryPassword);
                    target.PasswordHash = hash.HashBytes;
                    target.PasswordSalt = hash.SaltBytes;
                    target.PasswordIterations = hash.Iterations;
                    target.PasswordAlgorithm = hash.Algorithm;
                    target.MustChangePassword = true;
                    target.UpdatedAtUtc = DateTime.UtcNow;

                    session.Users.Update(target);
                    session.SaveChanges();
                    session.Commit();

                    _logger?.Info("AuthenticationService", string.Format("Admin {0} set temporary password for target user {1}.", actorUserId, targetUserId));
                    return PasswordResetResult.Success();
                }
                catch
                {
                    session.Rollback();
                    throw;
                }
            }
        }

        public ChangePasswordResult ChangePassword(int userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(currentPassword))
            {
                return ChangePasswordResult.Failure(AuthenticationErrorCode.InvalidCredentials);
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8 || newPassword.Length > 128)
            {
                return ChangePasswordResult.Failure(AuthenticationErrorCode.WeakPassword);
            }

            using (var session = _sessionFactory.Create())
            {
                session.BeginTransaction();
                try
                {
                    var user = session.Users.GetById(userId);
                    if (user == null)
                    {
                        session.Rollback();
                        return ChangePasswordResult.Failure(AuthenticationErrorCode.InvalidCredentials);
                    }

                    var storedHash = new PasswordHash(
                        user.PasswordHash,
                        user.PasswordSalt,
                        user.PasswordIterations,
                        user.PasswordAlgorithm);

                    if (!_passwordHasher.Verify(currentPassword, storedHash))
                    {
                        session.Rollback();
                        return ChangePasswordResult.Failure(AuthenticationErrorCode.InvalidCredentials);
                    }

                    var newHash = _passwordHasher.Hash(newPassword);
                    user.PasswordHash = newHash.HashBytes;
                    user.PasswordSalt = newHash.SaltBytes;
                    user.PasswordIterations = newHash.Iterations;
                    user.PasswordAlgorithm = newHash.Algorithm;
                    user.MustChangePassword = false;
                    user.UpdatedAtUtc = DateTime.UtcNow;

                    session.Users.Update(user);
                    session.SaveChanges();
                    session.Commit();

                    _logger?.Info("AuthenticationService", string.Format("User {0} changed password successfully.", userId));
                    return ChangePasswordResult.Success();
                }
                catch
                {
                    session.Rollback();
                    throw;
                }
            }
        }
    }
}
