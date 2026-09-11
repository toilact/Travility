# ADR-0002 — Solution gồm bốn project

- **Trạng thái:** Chấp nhận
- **Ngày:** 2026-09-11

## Bối cảnh

Chuẩn công nghiệp cho ứng dụng .NET nghiệp vụ là 4–6 project theo Clean
Architecture (`Domain` / `Application` / `Infrastructure` / `UI` / `Tests`) kèm
DI container, Repository và Unit of Work.

Nhưng ràng buộc thật của dự án này là: sáu tuần, bốn người, và việc chia dọc theo
module đã được chốt (`ADR` này không quyết định việc đó).

Vấn đề cần giải: người viết engine không được phụ thuộc vào việc UI hay CSDL đã
xong hay chưa.

## Quyết định

Bốn project trong một solution:

```
src/Travility.Data      → EF model (DB First), Repository
src/Travility.Core      → Entities, Services, 3 Engine, Events
src/Travility.WinForms  → Forms, chia thư mục theo module
tests/Travility.Tests   → Unit test cho 3 Engine
```

Phụ thuộc một chiều: `WinForms → Core → Data`.
`Travility.Core` **không được** tham chiếu `System.Windows.Forms`.

## Hệ quả

- Người làm engine chạy và test được trên `List<Place>` giả ngay từ ngày 1, không
  chờ ai. Đây là lý do chính của quyết định này.
- `Travility.Tests` tồn tại được vì engine là logic thuần — dễ test nhất.
- Truyền phụ thuộc qua constructor bằng tay, không dùng DI container.
- **Chi phí:** thêm ba project so với phương án một project, và phải kiểm tra
  reference mỗi khi thêm project mới.

## Phương án đã loại

**Năm project theo Clean Architecture đầy đủ + DI container** — đúng chuẩn ngành
nhất, nhưng tách `Domain` khỏi `Application` chỉ có giá trị khi cần thay tầng hạ
tầng (đổi EF sang Dapper, dùng lại domain cho web API). Trong sáu tuần sẽ không
xảy ra, nên tầng tách thêm chỉ tạo chi phí điều hướng file. DI container giải
quyết bài toán hàng trăm service; ở đây có khoảng tám.

**Một project duy nhất chia thư mục** — ít thao tác dựng nhất, nhưng bốn người
dùng chung một `.csproj` là nguồn conflict lớn nhất, và engine không tách rời để
test được.
