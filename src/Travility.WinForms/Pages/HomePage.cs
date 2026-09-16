using System;
using System.Windows.Forms;
using Travility.Core.Session;

namespace Travility.WinForms.Pages
{
    public partial class HomePage : UserControl
    {
        private readonly UserSession _session;

        public HomePage()
        {
            InitializeComponent();
        }

        public HomePage(UserSession session) : this()
        {
            _session = session;
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (_session == null)
            {
                lblWelcome.Text = "Chào mừng bạn!";
                lblUserName.Text = "Tên: Chưa xác định";
                lblUserRole.Text = "Vai trò: Khách";
                return;
            }

            var displayName = string.IsNullOrWhiteSpace(_session.DisplayName)
                ? _session.Username
                : _session.DisplayName;

            lblWelcome.Text = $"Chào mừng trở lại, {displayName}!";
            lblUserName.Text = $"Tên: {displayName} (@{_session.Username})";
            lblUserRole.Text = $"Vai trò: {(_session.IsAdmin ? "Quản trị viên (Admin)" : "Người dùng du lịch (Traveler)")}";
        }
    }
}
