using System;
using System.Windows.Forms;
using Travility.Core.Authentication;
using Travility.Core.Contracts;

namespace Travility.WinForms.Auth
{
    public partial class RegisterForm : Form
    {
        private readonly IAuthenticationService _authService;

        public event Action<int> RegistrationSucceeded;
        public event Action BackToLoginRequested;

        public RegisterForm(IAuthenticationService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            InitializeComponent();

            btnRegister.Click += BtnRegister_Click;
            lnkBackToLogin.LinkClicked += LnkBackToLogin_LinkClicked;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            lblMessage.Text = string.Empty;

            string username = txtUsername.Text != null ? txtUsername.Text.Trim() : string.Empty;
            string email = txtEmail.Text != null ? txtEmail.Text.Trim() : string.Empty;
            string displayName = txtDisplayName.Text != null ? txtDisplayName.Text.Trim() : string.Empty;
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            bool hasError = false;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorProvider.SetError(txtUsername, "Vui lòng nhập tên đăng nhập.");
                hasError = true;
            }
            else if (username.Contains("@"))
            {
                errorProvider.SetError(txtUsername, "Tên đăng nhập không được chứa ký tự '@'.");
                hasError = true;
            }
            else if (username.Length < 3 || username.Length > 50)
            {
                errorProvider.SetError(txtUsername, "Tên đăng nhập phải từ 3 đến 50 ký tự.");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                errorProvider.SetError(txtEmail, "Vui lòng nhập email.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(password))
            {
                errorProvider.SetError(txtPassword, "Vui lòng nhập mật khẩu.");
                hasError = true;
            }
            else if (password.Length < 8 || password.Length > 128)
            {
                errorProvider.SetError(txtPassword, "Mật khẩu phải từ 8 đến 128 ký tự.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                errorProvider.SetError(txtConfirmPassword, "Vui lòng xác nhận mật khẩu.");
                hasError = true;
            }
            else if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                errorProvider.SetError(txtConfirmPassword, "Mật khẩu xác nhận không khớp.");
                hasError = true;
            }

            if (hasError)
            {
                return;
            }

            btnRegister.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                var request = new RegistrationRequest
                {
                    Username = username,
                    Email = email,
                    DisplayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName,
                    Password = password
                };

                var result = _authService.Register(request);
                if (result.Succeeded)
                {
                    RegistrationSucceeded?.Invoke(result.UserId);
                }
                else
                {
                    lblMessage.Text = GetErrorMessage(result.ErrorCode);
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Đã xảy ra lỗi khi tạo tài khoản. Vui lòng thử lại.";
            }
            finally
            {
                btnRegister.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void LnkBackToLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            BackToLoginRequested?.Invoke();
        }

        private static string GetErrorMessage(AuthenticationErrorCode errorCode)
        {
            switch (errorCode)
            {
                case AuthenticationErrorCode.UsernameAlreadyExists:
                    return "Tên đăng nhập đã được sử dụng.";
                case AuthenticationErrorCode.EmailAlreadyExists:
                    return "Email đã được sử dụng.";
                case AuthenticationErrorCode.InvalidUsername:
                    return "Tên đăng nhập không hợp lệ (3–50 ký tự, không chứa '@').";
                case AuthenticationErrorCode.InvalidEmail:
                    return "Địa chỉ email không đúng định dạng.";
                case AuthenticationErrorCode.WeakPassword:
                    return "Mật khẩu quá yếu (tối thiểu 8 ký tự).";
                case AuthenticationErrorCode.Forbidden:
                    return "Bạn không có quyền thực hiện thao tác này.";
                default:
                    return "Đăng ký không thành công. Vui lòng kiểm tra lại.";
            }
        }
    }
}
