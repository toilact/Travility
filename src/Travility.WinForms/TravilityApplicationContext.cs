using System;
using System.Windows.Forms;
using Travility.Core.Authentication;
using Travility.Core.Contracts;
using Travility.Core.Session;
using Travility.WinForms.Auth;
using Travility.WinForms.Shell;

namespace Travility.WinForms
{
    public sealed class TravilityApplicationContext : ApplicationContext
    {
        private readonly IAuthenticationService _authService;
        private readonly IAppLogger _logger;
        private LoginForm _loginForm;
        private RegisterForm _registerForm;
        private MainForm _mainForm;
        private UserSession _currentSession;

        public TravilityApplicationContext(IAuthenticationService authService, IAppLogger logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            ShowLoginForm();
        }

        private void ShowLoginForm()
        {
            DisposeCurrentShell();

            _loginForm = new LoginForm(_authService);
            _loginForm.LoginSucceeded += OnLoginSucceeded;
            _loginForm.OpenRegisterRequested += OnOpenRegisterRequested;
            _loginForm.FormClosed += OnAuthFormClosed;

            MainForm = _loginForm;
            _loginForm.Show();
        }

        private void ShowRegisterForm()
        {
            if (_loginForm != null)
            {
                _loginForm.LoginSucceeded -= OnLoginSucceeded;
                _loginForm.OpenRegisterRequested -= OnOpenRegisterRequested;
                _loginForm.FormClosed -= OnAuthFormClosed;
                _loginForm.Close();
                _loginForm.Dispose();
                _loginForm = null;
            }

            _registerForm = new RegisterForm(_authService);
            _registerForm.RegistrationSucceeded += OnRegistrationSucceeded;
            _registerForm.BackToLoginRequested += OnBackToLoginRequested;
            _registerForm.FormClosed += OnAuthFormClosed;

            MainForm = _registerForm;
            _registerForm.Show();
        }

        private void OnOpenRegisterRequested()
        {
            ShowRegisterForm();
        }

        private void OnBackToLoginRequested()
        {
            if (_registerForm != null)
            {
                _registerForm.RegistrationSucceeded -= OnRegistrationSucceeded;
                _registerForm.BackToLoginRequested -= OnBackToLoginRequested;
                _registerForm.FormClosed -= OnAuthFormClosed;
                _registerForm.Close();
                _registerForm.Dispose();
                _registerForm = null;
            }

            ShowLoginForm();
        }

        private void OnRegistrationSucceeded(int userId)
        {
            _logger.Info("Auth", $"Đăng ký thành công người dùng có ID {userId}. Chuyển về màn hình đăng nhập.");
            MessageBox.Show(
                "Đăng ký tài khoản thành công! Vui lòng đăng nhập bằng thông tin vừa tạo.",
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            OnBackToLoginRequested();
        }

        private void OnLoginSucceeded(AuthenticatedUser user)
        {
            _logger.Info("Auth", $"Đăng nhập thành công: {user.Username} (Role: {user.Role}).");

            if (_loginForm != null)
            {
                _loginForm.LoginSucceeded -= OnLoginSucceeded;
                _loginForm.OpenRegisterRequested -= OnOpenRegisterRequested;
                _loginForm.FormClosed -= OnAuthFormClosed;
                _loginForm.Close();
                _loginForm.Dispose();
                _loginForm = null;
            }

            _currentSession = UserSession.FromAuthenticatedUser(user);
            _mainForm = new MainForm(_currentSession, _logger);
            _mainForm.LogoutRequested += OnLogoutRequested;
            _mainForm.FormClosed += OnMainFormClosed;

            MainForm = _mainForm;
            _mainForm.Show();
        }

        private void OnLogoutRequested(object sender, EventArgs e)
        {
            _logger.Info("Auth", "Thực hiện đăng xuất và khởi tạo lại phiên đăng nhập.");
            DisposeCurrentShell();
            ShowLoginForm();
        }

        private void OnAuthFormClosed(object sender, FormClosedEventArgs e)
        {
            if (_mainForm == null && _loginForm == null && _registerForm == null)
            {
                ExitThread();
            }
            else if (sender == MainForm)
            {
                ExitThread();
            }
        }

        private void OnMainFormClosed(object sender, FormClosedEventArgs e)
        {
            if (_loginForm == null && _registerForm == null)
            {
                ExitThread();
            }
        }

        private void DisposeCurrentShell()
        {
            if (_mainForm != null)
            {
                _mainForm.LogoutRequested -= OnLogoutRequested;
                _mainForm.FormClosed -= OnMainFormClosed;
                _mainForm.Close();
                _mainForm.Dispose();
                _mainForm = null;
            }

            _currentSession = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCurrentShell();

                if (_loginForm != null)
                {
                    _loginForm.Dispose();
                    _loginForm = null;
                }

                if (_registerForm != null)
                {
                    _registerForm.Dispose();
                    _registerForm = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
