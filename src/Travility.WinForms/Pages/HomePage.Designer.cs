namespace Travility.WinForms.Pages
{
    partial class HomePage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlCards = new System.Windows.Forms.TableLayoutPanel();
            this.pnlUserInfoCard = new System.Windows.Forms.Panel();
            this.lblUserRole = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblUserCardTitle = new System.Windows.Forms.Label();
            this.pnlAppInfoCard = new System.Windows.Forms.Panel();
            this.lblAppDesc = new System.Windows.Forms.Label();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.pnlNoticeCard = new System.Windows.Forms.Panel();
            this.lblNoticeDesc = new System.Windows.Forms.Label();
            this.lblNoticeTitle = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.pnlCards.SuspendLayout();
            this.pnlUserInfoCard.SuspendLayout();
            this.pnlAppInfoCard.SuspendLayout();
            this.pnlNoticeCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.ColumnCount = 1;
            this.pnlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlMain.Controls.Add(this.lblWelcome, 0, 0);
            this.pnlMain.Controls.Add(this.lblSubtitle, 0, 1);
            this.pnlMain.Controls.Add(this.pnlCards, 0, 2);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(24, 24);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.RowCount = 3;
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlMain.Size = new System.Drawing.Size(960, 680);
            this.pnlMain.TabIndex = 0;
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(111)))), ((int)(((byte)(151)))));
            this.lblWelcome.Location = new System.Drawing.Point(0, 0);
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(0);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(262, 32);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Chào mừng trở lại!";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(110)))), ((int)(((byte)(120)))));
            this.lblSubtitle.Location = new System.Drawing.Point(0, 42);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(325, 19);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Hệ thống quản lý và tối ưu hóa chi phí du lịch cá nhân";
            // 
            // pnlCards
            // 
            this.pnlCards.ColumnCount = 2;
            this.pnlCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlCards.Controls.Add(this.pnlUserInfoCard, 0, 0);
            this.pnlCards.Controls.Add(this.pnlAppInfoCard, 1, 0);
            this.pnlCards.Controls.Add(this.pnlNoticeCard, 0, 1);
            this.pnlCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCards.Location = new System.Drawing.Point(0, 88);
            this.pnlCards.Margin = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this.pnlCards.Name = "pnlCards";
            this.pnlCards.RowCount = 2;
            this.pnlCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.pnlCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlCards.Size = new System.Drawing.Size(960, 592);
            this.pnlCards.TabIndex = 2;
            // 
            // pnlUserInfoCard
            // 
            this.pnlUserInfoCard.BackColor = System.Drawing.Color.White;
            this.pnlUserInfoCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlUserInfoCard.Controls.Add(this.lblUserRole);
            this.pnlUserInfoCard.Controls.Add(this.lblUserName);
            this.pnlUserInfoCard.Controls.Add(this.lblUserCardTitle);
            this.pnlUserInfoCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlUserInfoCard.Location = new System.Drawing.Point(0, 0);
            this.pnlUserInfoCard.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.pnlUserInfoCard.Name = "pnlUserInfoCard";
            this.pnlUserInfoCard.Padding = new System.Windows.Forms.Padding(16);
            this.pnlUserInfoCard.Size = new System.Drawing.Size(468, 168);
            this.pnlUserInfoCard.TabIndex = 0;
            // 
            // lblUserRole
            // 
            this.lblUserRole.AutoSize = true;
            this.lblUserRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUserRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(143)))), ((int)(((byte)(175)))));
            this.lblUserRole.Location = new System.Drawing.Point(16, 85);
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new System.Drawing.Size(123, 19);
            this.lblUserRole.TabIndex = 2;
            this.lblUserRole.Text = "Vai trò: Người dùng";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblUserName.Location = new System.Drawing.Point(16, 50);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(127, 21);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "Tên: Nguyễn Văn";
            // 
            // lblUserCardTitle
            // 
            this.lblUserCardTitle.AutoSize = true;
            this.lblUserCardTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUserCardTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(130)))), ((int)(((byte)(140)))));
            this.lblUserCardTitle.Location = new System.Drawing.Point(16, 16);
            this.lblUserCardTitle.Name = "lblUserCardTitle";
            this.lblUserCardTitle.Size = new System.Drawing.Size(139, 15);
            this.lblUserCardTitle.TabIndex = 0;
            this.lblUserCardTitle.Text = "THÔNG TIN TÀI KHOẢN";
            // 
            // pnlAppInfoCard
            // 
            this.pnlAppInfoCard.BackColor = System.Drawing.Color.White;
            this.pnlAppInfoCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAppInfoCard.Controls.Add(this.lblAppDesc);
            this.pnlAppInfoCard.Controls.Add(this.lblAppTitle);
            this.pnlAppInfoCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAppInfoCard.Location = new System.Drawing.Point(492, 0);
            this.pnlAppInfoCard.Margin = new System.Windows.Forms.Padding(12, 0, 0, 12);
            this.pnlAppInfoCard.Name = "pnlAppInfoCard";
            this.pnlAppInfoCard.Padding = new System.Windows.Forms.Padding(16);
            this.pnlAppInfoCard.Size = new System.Drawing.Size(468, 168);
            this.pnlAppInfoCard.TabIndex = 1;
            // 
            // lblAppDesc
            // 
            this.lblAppDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAppDesc.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblAppDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblAppDesc.Location = new System.Drawing.Point(16, 50);
            this.lblAppDesc.Name = "lblAppDesc";
            this.lblAppDesc.Size = new System.Drawing.Size(434, 95);
            this.lblAppDesc.TabIndex = 1;
            this.lblAppDesc.Text = "Travility hỗ trợ bạn lập kế hoạch chuyến đi, khám phá các địa điểm du lịch, tối ư" +
    "u tuyến đường và kiểm soát ngân sách minh bạch, rõ ràng.";
            // 
            // lblAppTitle
            // 
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(130)))), ((int)(((byte)(140)))));
            this.lblAppTitle.Location = new System.Drawing.Point(16, 16);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(127, 15);
            this.lblAppTitle.TabIndex = 0;
            this.lblAppTitle.Text = "GIỚI THIỆU HỆ THỐNG";
            // 
            // pnlNoticeCard
            // 
            this.pnlNoticeCard.BackColor = System.Drawing.Color.White;
            this.pnlNoticeCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCards.SetColumnSpan(this.pnlNoticeCard, 2);
            this.pnlNoticeCard.Controls.Add(this.lblNoticeDesc);
            this.pnlNoticeCard.Controls.Add(this.lblNoticeTitle);
            this.pnlNoticeCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNoticeCard.Location = new System.Drawing.Point(0, 180);
            this.pnlNoticeCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.pnlNoticeCard.Name = "pnlNoticeCard";
            this.pnlNoticeCard.Padding = new System.Windows.Forms.Padding(16);
            this.pnlNoticeCard.Size = new System.Drawing.Size(960, 400);
            this.pnlNoticeCard.TabIndex = 2;
            // 
            // lblNoticeDesc
            // 
            this.lblNoticeDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNoticeDesc.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblNoticeDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblNoticeDesc.Location = new System.Drawing.Point(16, 50);
            this.lblNoticeDesc.Name = "lblNoticeDesc";
            this.lblNoticeDesc.Size = new System.Drawing.Size(926, 160);
            this.lblNoticeDesc.TabIndex = 1;
            this.lblNoticeDesc.Text = "Hiện tại hệ thống đang vận hành ở phiên bản Foundation Walking Skeleton:\r\n• Đã ho" +
    "àn tất xác thực danh tính, phân quyền vai trò (Admin / Traveler) và quản lý phiên" +
    " đăng nhập.\r\n• Toàn bộ dữ liệu tài chính tuân thủ quy tắc 3 chỉ số riêng biệt (N" +
    "gân sách, Chi phí dự kiến, Thực chi).\r\n• Các phân hệ Bản đồ tương tác, Quản lý " +
    "chuyến đi, Ngân sách chi tiết, Check-in và Gamification đang được kết nối theo l" +
    "ộ trình.";
            // 
            // lblNoticeTitle
            // 
            this.lblNoticeTitle.AutoSize = true;
            this.lblNoticeTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNoticeTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(106)))), ((int)(((byte)(79)))));
            this.lblNoticeTitle.Location = new System.Drawing.Point(16, 16);
            this.lblNoticeTitle.Name = "lblNoticeTitle";
            this.lblNoticeTitle.Size = new System.Drawing.Size(155, 15);
            this.lblNoticeTitle.TabIndex = 0;
            this.lblNoticeTitle.Text = "TRẠNG THÁI PHÁT TRIỂN";
            // 
            // HomePage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "HomePage";
            this.Padding = new System.Windows.Forms.Padding(24);
            this.Size = new System.Drawing.Size(1008, 728);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnlCards.ResumeLayout(false);
            this.pnlUserInfoCard.ResumeLayout(false);
            this.pnlUserInfoCard.PerformLayout();
            this.pnlAppInfoCard.ResumeLayout(false);
            this.pnlAppInfoCard.PerformLayout();
            this.pnlNoticeCard.ResumeLayout(false);
            this.pnlNoticeCard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnlMain;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.TableLayoutPanel pnlCards;
        private System.Windows.Forms.Panel pnlUserInfoCard;
        private System.Windows.Forms.Label lblUserCardTitle;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblUserRole;
        private System.Windows.Forms.Panel pnlAppInfoCard;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Label lblAppDesc;
        private System.Windows.Forms.Panel pnlNoticeCard;
        private System.Windows.Forms.Label lblNoticeTitle;
        private System.Windows.Forms.Label lblNoticeDesc;
    }
}
