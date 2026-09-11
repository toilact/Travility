# Quy tắc truy cập dữ liệu

Database First. Chỉ một người sửa `database/schema.sql`.

## Phân vai — không trộn trong cùng một service

| Dùng | Cho |
|---|---|
| **Entity Framework** | Toàn bộ CRUD nghiệp vụ |
| **ADO.NET** | `DistanceMatrix`, báo cáo thống kê Admin, stored procedure cần hiệu năng |

## Bắt buộc

- Truy vấn ADO.NET **luôn dùng tham số**, không nối chuỗi SQL.
- Form không gọi `DbContext`. Service gọi Repository, Repository gọi EF/ADO.NET.
- Lọc theo `UserId` ở tầng Repository — người này không được đọc dữ liệu người kia.
- Mọi seed là file `.sql` trong git, không seed tay trong SSMS.
- Đổi schema: báo cả nhóm → regenerate EF model → merge ngay.

Chi tiết bảng và hợp đồng trường: `docs/03-database-design.md`.
