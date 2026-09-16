using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Travility.Core.Contracts;
using Travility.Core.Session;

namespace Travility.WinForms.Shell
{
    public partial class MainForm : Form
    {
        private readonly UserSession _session;
        private readonly IAppLogger _logger;
        private readonly IPageFactory _pageFactory;
        private readonly Dictionary<string, UserControl> _pages =
            new Dictionary<string, UserControl>(StringComparer.OrdinalIgnoreCase);
        private readonly List<Button> _navButtons = new List<Button>();

        public event EventHandler LogoutRequested;

        public MainForm(UserSession session, IAppLogger logger, IPageFactory pageFactory = null)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pageFactory = pageFactory ?? new PageFactory(session);

            InitializeComponent();
            InitializeShell();
        }

        private void InitializeShell()
        {
            var displayName = string.IsNullOrWhiteSpace(_session.DisplayName)
                ? _session.Username
                : _session.DisplayName;

            var roleText = _session.IsAdmin ? "Admin" : "Traveler";
            lblUserInfo.Text = $"Xin chào, {displayName} ({roleText})";
            lblStatusUserRole.Text = $"Vai trò: {roleText}";

            RenderNavigation();
            NavigateTo("Home");
        }

        private void RenderNavigation()
        {
            pnlNavContainer.SuspendLayout();
            pnlNavContainer.Controls.Clear();
            _navButtons.Clear();

            var navItems = NavigationPolicy.ForRole(_session.Role);

            foreach (var item in navItems)
            {
                var btn = new Button
                {
                    Text = item.Label,
                    Tag = item.Key,
                    Size = new Size(200, 40),
                    Margin = new Padding(0, 0, 0, 6),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(12, 0, 0, 0),
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                    BackColor = Color.Transparent,
                    ForeColor = Color.FromArgb(60, 60, 60)
                };
                btn.FlatAppearance.BorderSize = 0;

                if (string.Equals(item.Key, "Home", StringComparison.OrdinalIgnoreCase))
                {
                    btn.Enabled = true;
                    btn.Cursor = Cursors.Hand;
                    btn.Click += NavButton_Click;
                }
                else
                {
                    btn.Enabled = false;
                    btn.ForeColor = Color.FromArgb(160, 160, 160);
                    toolTipNav.SetToolTip(btn, "Chưa khả dụng trong bản Foundation");
                }

                pnlNavContainer.Controls.Add(btn);
                _navButtons.Add(btn);
            }

            pnlNavContainer.ResumeLayout();
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string key)
            {
                NavigateTo(key);
            }
        }

        public void NavigateTo(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            try
            {
                var page = GetOrCreatePage(key);

                foreach (var btn in _navButtons)
                {
                    var isSelected = string.Equals(btn.Tag as string, key, StringComparison.OrdinalIgnoreCase);
                    btn.BackColor = isSelected ? Color.FromArgb(234, 242, 248) : Color.Transparent;
                    btn.ForeColor = isSelected ? Color.FromArgb(42, 111, 151) : Color.FromArgb(60, 60, 60);
                    btn.Font = new Font("Segoe UI", 9.5F, isSelected ? FontStyle.Bold : FontStyle.Regular);
                }

                pnlContent.SuspendLayout();
                pnlContent.Controls.Clear();
                page.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(page);
                page.BringToFront();
                pnlContent.ResumeLayout();

                lblStatus.Text = $"Đang hiển thị: {key}";
                _logger.Info("Shell", $"Chuyển hướng màn hình tới '{key}'.");
            }
            catch (Exception ex)
            {
                _logger.Error("Shell", "NAV-ERROR", ex);
                MessageBox.Show(
                    $"Không thể tải trang '{key}'. Lỗi: {ex.Message}",
                    "Lỗi điều hướng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private UserControl GetOrCreatePage(string key)
        {
            UserControl page;
            if (_pages.TryGetValue(key, out page)) return page;

            page = _pageFactory.Create(key);
            _pages.Add(key, page);
            return page;
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                _logger.Info("Shell", $"Người dùng '{_session.Username}' xác nhận đăng xuất.");
                LogoutRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var page in _pages.Values)
                {
                    page.Dispose();
                }
                _pages.Clear();

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
