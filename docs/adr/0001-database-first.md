# ADR-0001 — Dùng Entity Framework Database First

- **Trạng thái:** Chấp nhận
- **Ngày:** 2026-09-11

## Bối cảnh

Bốn người viết code song song trong sáu tuần, cả bốn đều cần bảng dữ liệu để làm
việc ngay từ tuần 1. Schema chắc chắn sẽ thay đổi nhiều lần trong suốt dự án.

Môn học yêu cầu cả **Bài 8 (ADO.NET)** lẫn **Bài 9 (Entity Framework)**, và tài
liệu phải nộp gồm ERD — tức là mô hình dữ liệu phải được thiết kế tường minh
trước, không phải suy ra từ code.

## Quyết định

Dùng **Database First**: viết `database/schema.sql` thủ công, một người sở hữu
file đó, rồi reverse-engineer ra EF model.

## Hệ quả

- Một file `.sql` duy nhất trong git — review diff dễ, không có chiến tranh migration.
- `__MigrationHistory` không tồn tại, nên không có chuyện mỗi máy một trạng thái DB.
- Stored procedure viết trực tiếp trong `schema.sql` và gọi bằng ADO.NET một cách
  tự nhiên, không phải uốn EF.
- ERD phải nộp khớp 1-1 với schema thật.
- **Chi phí:** mỗi lần đổi schema phải regenerate EF model và báo cả nhóm.
- **Kỷ luật bắt buộc:** chỉ một người được sửa `schema.sql`.

## Phương án đã loại

**Code First + Migrations** — cho phép bắt đầu code nhanh hơn, nhưng bốn người
tạo migration song song trên .NET Framework rất dễ hỏng khi merge, và mỗi máy
một database local sẽ lệch nhau. Rủi ro này lớn hơn lợi ích tốc độ.

**Code First không Migrations (drop & recreate)** — đơn giản nhất, nhưng mất dữ
liệu test sau mỗi lần đổi schema, mà trong sáu tuần schema sẽ đổi rất nhiều lần.
