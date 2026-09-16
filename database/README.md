# Database Foundation

Hiện tại có năm bảng Identity: `Roles`, `Users`, `UserProfiles`,
`UserPreferences`, `TravelWallets`. Các nhóm bảng còn lại, danh mục tham chiếu
và stored procedure được bổ sung ở Task 4 của kế hoạch Foundation.

| File | Nội dung |
|---|---|
| `reset-dev.sql` | Xóa và tạo lại duy nhất database `TravilityDev` sau kiểm tra tên chính xác |
| `schema.sql` | Bảng, khóa ngoại, check constraint và index; chạy một lần trên database trống |
| `seed_reference.sql` | Thêm hai role `Admin`, `Traveler` còn thiếu |
| `seed_demo.sql` | Thêm tài khoản demo và profile, preferences, wallet còn thiếu |
| `smoke_test.sql` | Kiểm tra schema, seed và metadata bằng truy vấn chỉ đọc |

## Chuẩn bị trên Windows

Cài SQL Server Express 2022 với instance `.\SQLEXPRESS` và công cụ `sqlcmd`.
Dùng Windows Authentication; tài khoản chạy reset cần quyền tạo/xóa database.
Chạy các lệnh dưới từ thư mục gốc repository trong PowerShell. Tất cả script
có `:ON ERROR EXIT`; nếu chạy bằng SSMS, phải bật **SQLCMD Mode**.

`reset-dev.sql` **xóa toàn bộ dữ liệu trong `TravilityDev` và ngắt kết nối đang
dùng database đó**. Chỉ chạy khi chủ động dựng lại môi trường dev; không chạy
reset để bổ sung seed. Script không nhận tên database từ biến hoặc tham số.

```powershell
sqlcmd -S .\SQLEXPRESS -E -b -f 65001 -i database\reset-dev.sql
if ($LASTEXITCODE -ne 0) { throw 'Reset failed' }
sqlcmd -S .\SQLEXPRESS -E -b -f 65001 -i database\schema.sql
if ($LASTEXITCODE -ne 0) { throw 'Schema failed' }
sqlcmd -S .\SQLEXPRESS -E -b -f 65001 -i database\seed_reference.sql
if ($LASTEXITCODE -ne 0) { throw 'Reference seed failed' }
sqlcmd -S .\SQLEXPRESS -E -b -f 65001 -i database\seed_demo.sql
if ($LASTEXITCODE -ne 0) { throw 'Demo seed failed' }
sqlcmd -S .\SQLEXPRESS -E -b -f 65001 -i database\smoke_test.sql
if ($LASTEXITCODE -ne 0) { throw 'Smoke test failed' }
```

`-b` trả exit code khác 0 khi SQL lỗi; `-f 65001` giữ đúng tiếng Việt trong
các file UTF-8. Schema dùng transaction để không để lại DDL dở dang khi lỗi.

Kiểm tra seed có thể chạy lại bằng cách chạy lại hai lệnh seed và smoke ở trên,
không chạy lại reset/schema. Seed chỉ thêm bản ghi còn thiếu: không reset mật
khẩu, kích hoạt lại tài khoản, đổi profile/preferences hay ghi đè số dư ví đã
chỉnh sửa. Nếu identifier demo thuộc một tài khoản có email hoặc role khác,
seed dừng và rollback để người phát triển kiểm tra xung đột.

## Tài khoản demo — chỉ dùng trong môi trường nội bộ

| Username | Email | Mật khẩu khởi tạo | Role |
|---|---|---|---|
| `admin` | `admin@travility.local` | `Admin@12345` | Admin |
| `traveler` | `traveler@travility.local` | `Traveler@12345` | Traveler |

Cả hai tài khoản khởi tạo với `IsActive = 1`, `MustChangePassword = 0`, số dư
ví 0 và bảy điểm sở thích bằng 3. SQL chỉ chứa hash/salt xác định, không chứa
mật khẩu plaintext. Hash dùng PBKDF2-HMAC-SHA256, 600.000 vòng, salt 16 byte,
derived key 32 byte, tương thích `Pbkdf2PasswordHasher` trong Core.

Các giá trị cố định này phục vụ demo có thể tái tạo. Tài khoản người dùng thật
được service hash với salt ngẫu nhiên riêng. Mật khẩu demo ở trên chỉ đúng sau
khởi tạo/reset; chạy lại seed không khôi phục mật khẩu cũ.

## Kiểm chứng và quy tắc dữ liệu

Smoke test không tạo, cập nhật hoặc xóa dữ liệu. Nó cần hai role/demo account
còn active, kiểm tra các bản ghi liên kết, format hash, miền điểm sở thích và
các unique index/FK đang hoạt động. Sau khi cố ý vô hiệu hóa tài khoản demo
để thử luồng Auth, cần khôi phục trạng thái đó trước khi chạy smoke.

Mac hiện chưa có SQL Server/`sqlcmd`: kiểm tra cú pháp tĩnh và hash độc lập
không thay thế việc chạy toàn bộ chuỗi lệnh trên SQL Server. Kết quả thực thi
SQL RED/GREEN và kiểm tra seed chạy lặp đang chờ môi trường Windows.

- **Chỉ Thành (A) sửa `schema.sql` trong Cổng 1.** Ai cần thêm bảng/cột thì báo A.
- Foreign key dùng `NO ACTION`; không cascade-delete dữ liệu Identity.
- `UserId` là unique ở profile, preferences và wallet; mỗi bảng vẫn có PK
  `int IDENTITY` riêng để EF Database First sinh model đúng quy ước.
- Thời điểm audit dùng UTC `datetime2(0)`. Default chỉ gán lúc insert; service
  phải cập nhật `UpdatedAtUtc` khi sửa dữ liệu. Không dùng trigger.
- Số dư dùng `decimal(18,2)`; service cập nhật cùng transaction ghi Expense
  theo BR-04. DDL chưa áp quy tắc số dư không âm vì spec chưa chốt quy tắc đó.
- Mọi seed nằm trong Git. Sau khi đổi schema, báo nhóm và regenerate EDMX
  trên Visual Studio 2022 theo quy trình Database First.

Nguồn sự thật: [Foundation spec §6](../docs/superpowers/specs/2026-09-16-foundation-walking-skeleton-design.md)
và [thiết kế CSDL](../docs/03-database-design.md).
