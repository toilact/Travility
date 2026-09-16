using System;
using System.Windows.Forms;
using Travility.Core.Authentication;
using Travility.Core.Contracts;

namespace Travility.WinForms.Auth
{
    public partial class LoginForm : Form
    {
        private readonly IAuthenticationService _authService;

        public event Action<AuthenticatedUser> LoginSucceeded;
        public event Action OpenRegisterRequested;

        public LoginForm(IAuthenticationService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            InitializeComponent();

            btnLogin.Click += BtnLogin_Click;
            lnkRegister.LinkClicked += LnkRegister_LinkClicked;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            lblMessage.Text = string.Empty;

            string identifier = txtIdentifier.Text != null ? txtIdentifier.Text.Trim() : string.Empty;
            string password = txtPassword.Text;

            bool hasError = false;
            if (string.IsNullOrWhiteSpace(identifier))
            {
                errorProvider.SetError(txtIdentifier, "Vui lòng nhập tên đăng nhập hoặc email.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(password))
            {
                errorProvider.SetError(txtPassword, "Vui lòng nhập mật khẩu.");
                hasError = true;
            }

            if (hasError)
            {
                return;
            }

            btnLogin.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                var result = _authService.Login(identifier, password);
                if (result.Succeeded)
                {
                    if (result.User.MustChangePassword)
                    {
                        using (var changeDlg = new ChangePasswordDialog(_authService, result.User.UserId))
                        {
                            if (changeDlg.ShowDialog(this) == DialogResult.OK)
                            {
                                LoginSucceeded?.Invoke(result.User);
                            }
                            else
                            {
                                txtPassword.Clear();
                                lblMessage.Text = "Bạn phải đổi mật khẩu tạm thời để tiếp tục đăng nhập.";
                            }
                        }
                    }
                    else
                    {
                        LoginSucceeded?.Invoke(result.User);
                    }
                }
                else
                {
                    lblMessage.Text = GetErrorMessage(result.ErrorCode);
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Đã xảy ra lỗi khi xử lý đăng nhập. Vui lòng thử lại.";
            }
            finally
            {
                btnLogin.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void LnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenRegisterRequested?.Invoke();
        }

        private static string GetErrorMessage(AuthenticationErrorCode errorCode)
        {
            switch (errorCode)
            {
                case AuthenticationErrorCode.InvalidCredentials:
                    return "Tên đăng nhập/email hoặc mật khẩu không đúng.";
                case AuthenticationErrorCode.AccountDisabled:
                    return "Tài khoản đã bị vô hiệu hóa. Hãy liên hệ quản trị viên.";
                case AuthenticationErrorCode.Forbidden:
                    return "Bạn không có quyền thực hiện thao tác này.";
                default:
                    return "Đăng nhập không thành công. Vui lòng kiểm tra lại.";
            }
        }
    }
}
