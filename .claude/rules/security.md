# Quy tắc bảo mật

- **Mật khẩu phải hash.** PBKDF2-HMAC-SHA256 (`Rfc2898DeriveBytes`), baseline
  600.000 vòng, salt CSPRNG riêng 16 byte, hash 32 byte. Lưu iteration và
  algorithm theo user; benchmark dưới khoảng một giây trên laptop yếu nhất
  trước khi đóng băng cấu hình. Bảng `Users` không có cột `Password`.
- **Mật khẩu dài 8–128 ký tự**, không trim hay normalize; so sánh hash duyệt
  đủ 32 byte để không phụ thuộc vị trí byte khác nhau. `Verify` trả `false`
  với password/hash sai định dạng; `PasswordHash` sao chép mảng để giữ bất biến.
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
