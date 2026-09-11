# Quy tắc bảo mật

- **Mật khẩu phải hash.** PBKDF2 (`Rfc2898DeriveBytes`), salt riêng mỗi người
  dùng, 10.000 vòng. Bảng `Users` không có cột `Password`.
- **Không hardcode API key.** Đọc từ `config/api-keys.json` (nằm trong
  `.gitignore`). Commit `config/api-keys.example.json` làm mẫu.
- **Connection string** trong `App.config`, dùng Windows Authentication
  (`Integrated Security=True`). Không nhúng mật khẩu SQL.
- **Truy vấn ADO.NET luôn tham số hoá.** Không nối chuỗi.
- **Kiểm tra vai trò ở tầng Service**, không chỉ ẩn nút trên giao diện.
- **Lọc theo `UserId`** ở Repository — không ai đọc được dữ liệu người khác.
- Bật TLS 1.2 khi gọi HTTPS trên .NET Framework:
  `ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;`

Chi tiết: `docs/11-security.md`.
