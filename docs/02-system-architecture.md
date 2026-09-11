# 02 — Kiến trúc hệ thống

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) · [`00-project-overview.md`](./00-project-overview.md)
> **Quyết định nền:** [ADR-0002 — Solution bốn project](./adr/0002-four-project-solution.md)

---

## 1. Cấu trúc solution

```
Travility.sln
├── src/Travility.Data      → EF model (DB First), Repository            [A]
├── src/Travility.Core      → Entities, Services, 3 Engine, Events       [C, D]
│     └── Engines/            ← KHÔNG tham chiếu WinForms, KHÔNG cần DB
├── src/Travility.WinForms  → Forms, chia thư mục theo module            [A B C D]
│     ├── Auth/    [A]     ├── Trip/    [C]     ├── Map/       [B]
│     └── Admin/   [A]     ├── Budget/  [C]     └── Itinerary/ [D]
└── tests/Travility.Tests   → Unit test cho 3 Engine                     [D]
```

### Quy tắc phụ thuộc một chiều

```
WinForms  ──→  Core  ──→  Data
```

`Travility.Core` **không được** tham chiếu `System.Windows.Forms`.

Đây không phải quy tắc hình thức. Nó là điều cho phép người viết engine chạy và
test trên `List<Place>` giả **ngay từ ngày 1**, không chờ CSDL, không chờ giao
diện. Nếu quy tắc này bị phá, cả nhóm mất khả năng làm song song.

Kiểm tra: mở References của `Travility.Core`, không được có `System.Windows.Forms`.

---

## 2. Tầng và trách nhiệm

```
┌─────────────────────────────────────────────┐
│  WINFORMS                                   │
│  Dashboard · SmartMap · TripWizard          │
│  Budget · Itinerary · Chatbot · Admin       │
└──────────────────┬──────────────────────────┘
                   │  chỉ gọi interface, không gọi DbContext
┌──────────────────▼──────────────────────────┐
│  CORE                                       │
│  TripService · BookingService               │
│  BudgetService · CheckInService             │
│  AchievementService                         │
│  ┌───────────┬───────────┬────────────────┐ │
│  │Recommend. │ Routing   │ Itinerary      │ │
│  │Engine     │ Engine    │ Engine         │ │
│  └───────────┴───────────┴────────────────┘ │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│  DATA                                       │
│  Repository · DbContext (EF, DB First)      │
│  Stored procedure qua ADO.NET               │
└──────────────────┬──────────────────────────┘
                   │
             SQL SERVER
```

Nguồn bên ngoài (tile bản đồ, OSRM, LLM) đi qua interface ở tầng `Core`, không
gọi thẳng từ Form.

---

## 3. Ranh giới EF vs ADO.NET

Môn có cả Bài 8 và Bài 9 nên dùng cả hai là hợp lý, **nhưng phải có lý do được
ghi ra**, nếu không sẽ thành hai phong cách truy cập dữ liệu lẫn lộn — lỗi kiến
trúc dễ bị hỏi nhất.

| Dùng | Cho |
|---|---|
| **Entity Framework** | Toàn bộ CRUD nghiệp vụ |
| **ADO.NET** | `DistanceMatrix`, báo cáo thống kê Admin, stored procedure cần hiệu năng |

Không trộn hai cách trong cùng một service.

---

## 4. Sự kiện hệ thống

Các module liên kết qua event, **không gọi thẳng nhau**. Đây là chỗ dùng
**Bài 4 — Delegate và Event** có lý do nghiệp vụ thật, không phải chỉ gán
`button.Click`.

```
PlaceAdded · PlaceRemoved · HotelChanged · BookingConfirmed
BudgetChanged · ExpenseRecorded · CheckInCompleted · ItineraryGenerated
```

Tên event ở thì **quá khứ** — nó thông báo việc đã xảy ra, không phải mệnh lệnh.

### Chuỗi phản ứng mẫu

```
Người dùng thêm khách sạn
        │
        ▼
   HotelChanged
        ├──────► BudgetService    tính lại PlannedCost
        ├──────► RoutingEngine    tính lại RoutePlan
        └──────► ItineraryEngine  sinh lại phương án
```

```
Người dùng xoá một nhà hàng
        │
   PlaceRemoved
        ├──────► PlannedCost giảm
        ├──────► RoutePlan ngắn lại
        ├──────► quỹ thời gian trong ngày dư ra
        └──────► RecommendationEngine gợi ý địa điểm thay thế
```

Lợi ích kiến trúc: `BudgetService` không cần biết `ItineraryEngine` tồn tại.
Thêm module mới chỉ cần đăng ký lắng nghe, không sửa code cũ.

---

## 5. Bốn nguyên tắc không được vi phạm

1. **Không phụ thuộc cứng vào một API.** Mọi nguồn ngoài đi qua interface:
   `IPlaceProvider`, `ILocationProvider`, `IChatProvider`.
2. **Không gộp mọi thứ vào một hàm `OptimizeTrip()`.** Tách
   `RecommendationEngine` / `RoutingEngine` / `ItineraryEngine` / `BudgetEngine`.
3. **AI không truy cập CSDL trực tiếp.**
   `AI → Tool → Business Service → Repository → SQL Server`.
4. **Event cho mọi thay đổi quan trọng**, để module liên quan tự cập nhật.

---

## 6. Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| `Travility.Core` lỡ tham chiếu WinForms | Kiểm tra References mỗi khi thêm project |
| Form gọi thẳng `DbContext` cho nhanh | Luôn đi qua Service ở tầng `Core` |
| Service gọi thẳng Service khác tạo phụ thuộc vòng | Dùng event |
| Trộn EF và ADO.NET trong một service | Tách theo bảng ở mục 3 |
| Đặt engine trong `Travility.WinForms` | Engine thuộc `Core`, nếu không sẽ không test được |
