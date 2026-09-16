using System;
using System.Windows.Forms;
using Travility.Core.Authentication;
using Travility.Core.Contracts;

namespace Travility.WinForms.Auth
{
    public partial class ChangePasswordDialog : Form
    {
        private readonly IAuthenticationService _authService;
        private readonly int _userId;

        public ChangePasswordDialog(IAuthenticationService authService, int userId)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userId = userId;

            InitializeComponent();

            btnChangePassword.Click += BtnChangePassword_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            lblMessage.Text = string.Empty;

            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            bool hasError = false;

            if (string.IsNullOrEmpty(currentPassword))
            {
                errorProvider.SetError(txtCurrentPassword, "Vui lòng nhập mật khẩu hiện tại.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                errorProvider.SetError(txtNewPassword, "Vui lòng nhập mật khẩu mới.");
                hasError = true;
            }
            else if (newPassword.Length < 8 || newPassword.Length > 128)
            {
                errorProvider.SetError(txtNewPassword, "Mật khẩu mới phải từ 8 đến 128 ký tự.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                errorProvider.SetError(txtConfirmPassword, "Vui lòng xác nhận mật khẩu mới.");
                hasError = true;
            }
            else if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                errorProvider.SetError(txtConfirmPassword, "Mật khẩu xác nhận không khớp.");
                hasError = true;
            }

            if (hasError)
            {
                return;
            }

            btnChangePassword.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                var result = _authService.ChangePassword(_userId, currentPassword, newPassword);
                if (result.Succeeded)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    lblMessage.Text = GetErrorMessage(result.ErrorCode);
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Đã xảy ra lỗi khi đổi mật khẩu. Vui lòng thử lại.";
            }
            finally
            {
                btnChangePassword.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static string GetErrorMessage(AuthenticationErrorCode errorCode)
        {
            switch (errorCode)
            {
                case AuthenticationErrorCode.InvalidCredentials:
                    return "Mật khẩu hiện tại không đúng.";
                case AuthenticationErrorCode.WeakPassword:
                    return "Mật khẩu mới quá yếu (tối thiểu 8 ký tự).";
                case AuthenticationErrorCode.Forbidden:
                    return "Bạn không có quyền thực hiện thao tác này.";
                default:
                    return "Đổi mật khẩu thất bại. Vui lòng kiểm tra lại.";
            }
        }
    }
}
