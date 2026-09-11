# CONTEXT — Mô hình miền Travility

> Từ điển thuật ngữ của dự án. Khi viết code, tên issue, tên test, tên biến hay
> câu trong báo cáo có nhắc tới một khái niệm nghiệp vụ, **dùng đúng từ ở đây**.
>
> Đây không phải tài liệu giới thiệu dự án — xem `docs/00-project-overview.md`.

---

## Vì sao file này tồn tại

Bốn người viết code song song trong sáu tuần. Nếu một người gọi là `Budget`,
người khác gọi là `TotalCost`, người thứ ba gọi là `Chi phí dự kiến`, thì đến
tuần 5 ghép lại sẽ không ai chắc ba thứ đó có phải một hay không.

Từ điển này giải quyết đúng vấn đề đó. Nó cũng là file mà các skill engineering
đọc trước khi khám phá codebase.

---

## 1. Miền cốt lõi

### Place

Một **địa điểm** có thật, lưu trong bảng `Places`: khách sạn, nhà hàng, bảo tàng,
bãi biển, bến xe.

- Luôn có `Latitude`/`Longitude`, `CategoryId`, `VisitDuration`, `PreferenceTags`.
- Trong **code và CSDL** luôn gọi là `Place`.
- Từ **POI** (Point of Interest) chỉ dùng khi nói về *dataset* ở mức tài liệu
  — "dataset gồm 150 POI". Không đặt tên lớp hay bảng là `POI`.

### PreferenceTags

Một tới ba nhãn gắn vào `Place`, lấy từ **đúng bảy nhóm sở thích**:

```
Biển · Ẩm thực · Lịch sử · Văn hóa · Giải trí · Mua sắm · Thiên nhiên
```

Bảy nhóm này xuất hiện ở ba nơi và **phải giống hệt nhau** ở cả ba: thanh trượt
sở thích trong Trip Wizard, `PreferenceTags` của `Place`, và `PreferenceScore`
trong Recommendation Engine.

### VisitDuration

Thời gian **tham quan trung bình** tại một `Place`, tính bằng phút. Gán tay theo
category, không lấy từ API nào.

Không nhầm với **thời gian di chuyển** giữa hai địa điểm — cái đó là `TravelTime`.

### Trip

Một **chuyến đi** do người dùng tạo: điểm đến, ngày đi, ngày về, số người, ngân sách,
sở thích. `Trip` là cái *ý định*, chưa phải kế hoạch chi tiết.

Một `Trip` chứa nhiều `Itinerary`.

### Itinerary

Một **phương án lịch trình** cho một `Trip`. Hệ thống sinh ba phương án tương ứng
ba `TravelStyle`; người dùng chọn một làm phương án chính thức.

```
Trip (Đà Nẵng, 4 ngày, 8 triệu)
 ├── Itinerary A   (TravelStyle = Budget)
 ├── Itinerary B   (TravelStyle = Balanced)   ← người dùng chọn
 └── Itinerary C   (TravelStyle = Experience)
```

`Itinerary` gồm nhiều `ItineraryDay`, mỗi ngày gồm nhiều `ItineraryItem`.

### RoutePlan

**Thứ tự di chuyển trong một ngày** của một `Itinerary`, kèm tổng quãng đường,
tổng thời gian, tổng chi phí di chuyển. Gồm các `RouteSegment` nối hai điểm liên tiếp.

`Itinerary` trả lời *"đi đâu, ngày nào"*. `RoutePlan` trả lời *"trong ngày đó đi
theo thứ tự nào"*. Hai khái niệm khác nhau, đừng gộp.

### TravelStyle

Một trong **ba preset trọng số** dùng cho hàm mục tiêu: `Budget`, `Balanced`,
`Experience`. Không phải "mức độ ưu tiên", không phải "chế độ tối ưu".

Mỗi `TravelStyle` là một bộ sáu trọng số cố định — xem `docs/07-engines.md`.

---

## 2. Miền tiền bạc

Đây là nhóm hay bị dùng lẫn lộn nhất. **Ba con số, ba nguồn, ba ý nghĩa khác nhau.**

### Budget

**Hạn mức người dùng tự đặt** cho một `Trip`. Do người dùng nhập, hệ thống không
bao giờ tự sửa.

### PlannedCost

**Chi phí dự kiến do hệ thống tự tính** từ `Itinerary` đang chọn: vé phương tiện
+ khách sạn + vé tham quan + ăn uống + di chuyển nội thành.

Thay đổi mỗi khi `Itinerary` thay đổi. Người dùng không nhập tay.

### ActualSpent

**Số tiền đã tiêu thật**, cộng dồn từ các bản ghi `Expense` do người dùng nhập
trong lúc đi.

```
Budget        8.000.000   ← người dùng đặt
PlannedCost   7.350.000   ← hệ thống tính
ActualSpent   6.100.000   ← người dùng ghi nhận
```

Ba con số này **không bao giờ được gộp** thành một trên giao diện hay trong code.

### TravelWallet

**Tổng tiền người dùng dành cho du lịch**, ở cấp tài khoản chứ không phải cấp chuyến.

Là **hạn mức**, không phải két giữ tiền:

- Tạo `Trip` **không** trừ `TravelWallet`.
- Chỉ `Expense` mới làm giảm số dư.
- Tổng `Budget` các `Trip` đang hoạt động vượt số dư ví → **cảnh báo, không chặn**.

### Expense

Một lần chi tiêu **có thật** do người dùng ghi nhận. Thuộc về một `Trip` và một
`BudgetCategory`.

Không nhầm với `PlannedCost` — `Expense` là quá khứ, `PlannedCost` là tương lai.

### PricingUnit

Đơn vị nhân của một khoản chi phí. Ba giá trị:

| Giá trị | Nhân theo | Ví dụ |
|---|---|---|
| `PerPerson` | số người | Vé tham quan, ăn uống, vé máy bay |
| `PerRoom` | số phòng × số đêm | Khách sạn |
| `PerTrip` | số chuyến | Taxi, xăng xe |

Đây là thứ khiến chuyến 2 người **không** bị nhân đôi tiền khách sạn.

### BudgetCategory

Nhóm ngân sách. Đúng tám giá trị:

```
Transportation · Accommodation · Food · Activities
LocalTransport · Shopping · Reserve · Other
```

Nhóm dự phòng gọi là **`Reserve`**. Không dùng `Emergency` — đặc tả gốc có chỗ
viết `Emergency`, đó là bản cũ.

---

## 3. Miền trải nghiệm

### CheckIn

Ghi nhận người dùng **có mặt tại** một `Place`, khi khoảng cách tới địa điểm nhỏ
hơn `CheckInRadius` (150 m).

Không nhầm với `Booking` — `Booking` là đặt trước, `CheckIn` là đã tới nơi.

Mỗi `CheckIn` có cờ `IsSimulated` phân biệt vị trí thật và vị trí giả lập.

### SimulatedLocation

Vị trí do người dùng **click lên bản đồ** thay vì lấy từ API định vị. Cần thiết
vì ứng dụng desktop không định vị chính xác được, và buổi bảo vệ diễn ra trong
phòng học cách mọi `Place` hàng km.

Giao diện luôn hiển thị nhãn `SIMULATED` khi đang ở chế độ này. Xem
`docs/adr/0006-simulated-location-provider.md`.

### Achievement

Định nghĩa một **huy hiệu** và điều kiện đạt được, lưu trong bảng `Achievements`.
Điều kiện đọc từ CSDL, không hardcode trong C#.

`UserAchievement` là bản ghi một người dùng **đã đạt** một `Achievement`.

### TravelPassport

Hồ sơ tổng hợp cấp tài khoản: số thành phố đã đến, số `Place` đã `CheckIn`, tổng
quãng đường, số `Trip`, số huy hiệu.

Là **khung nhìn tổng hợp**, không phải bảng lưu trữ số liệu trùng lặp — tính bằng
LINQ từ `CheckIns` và `Trips`.

### Booking

Ghi nhận đặt chỗ cho phương tiện hoặc khách sạn.

- `BookingType`: `Flight` · `Bus` · `Train` · `Hotel`
- `BookingStatus`: `Pending` · `Reserved` · `Confirmed` · `Cancelled`

**Là mô phỏng, không phải đặt vé thật.** Không có API thương mại nào được gọi.

---

## 4. Miền thuật toán

### RecommendationScore

Điểm tổng hợp của một `Place` với một `Trip` cụ thể, là tổ hợp tuyến tính sáu
thành phần có tổng trọng số bằng 1. Đầu ra của Recommendation Engine.

### WeightedRating

Rating đã hiệu chỉnh theo **Bayesian**, để địa điểm ít review không vượt mặt địa
điểm nhiều review chỉ vì trung bình cao hơn.

```
WeightedRating = (v/(v+m))·R + (m/(v+m))·C
```

### DistanceMatrix

Bảng khoảng cách giữa mọi cặp `Place`, tính sẵn và lưu trong CSDL, truy vấn bằng
stored procedure qua ADO.NET.

Khoảng cách trong engine là **Haversine × 1.3**, không phải khoảng cách đường
thật. Xem `docs/adr/0004-*.md`.

### TravelTime

Thời gian **di chuyển** giữa hai `Place`. Khác `VisitDuration` (thời gian ở lại).

---

## 5. Thuật ngữ cấm dùng

Ba cụm từ dưới đây **không được xuất hiện** trong code, tài liệu, báo cáo hay
slide. Chúng mô tả những thứ dự án không thực sự triển khai, và dùng chúng là
cách nhanh nhất để bị hội đồng hỏi vặn.

| Cấm | Dùng thay | Vì sao |
|---|---|---|
| **Pareto Front** / Pareto Optimal | "ba preset trọng số theo `TravelStyle`" | Không lọc dominance trên tập nghiệm lớn |
| **TSP with Time Windows** | "heuristic Nearest Neighbor + 2-opt có kiểm tra khung giờ" | Không giải mô hình TSPTW |
| **Orienteering Problem** | "chọn tham lam theo tỉ lệ điểm/chi phí" | Không giải mô hình OP |

Tương tự, tránh nói **"tích hợp Google Places"** (dataset tự xây) và
**"đặt vé"** (là mô phỏng).

---

## 6. Quy ước đặt tên

| Loại | Quy ước | Ví dụ |
|---|---|---|
| Bảng CSDL | PascalCase, số nhiều | `Places`, `ItineraryItems` |
| Cột | PascalCase, số ít | `VisitDuration`, `PricingUnit` |
| Khoá chính | `<Tênbảngsốít>Id` | `PlaceId`, `TripId` |
| Lớp C# | PascalCase, số ít | `Place`, `ItineraryDay` |
| Interface | `I` + danh từ | `IRoutingEngine`, `ILocationProvider` |
| Service | `<Miền>Service` | `TripService`, `BudgetService` |
| Engine | `<Vai trò>Engine` | `RecommendationEngine` |
| Event | Quá khứ phân từ | `PlaceAdded`, `BudgetChanged` |
| Tiền tệ | **luôn** `decimal` | không bao giờ `float`/`double` |

---

## 7. Sự kiện hệ thống

Các module liên kết qua event, không gọi thẳng nhau:

```
PlaceAdded · PlaceRemoved · HotelChanged · BookingConfirmed
BudgetChanged · ExpenseRecorded · CheckInCompleted · ItineraryGenerated
```

Tên event ở thì **quá khứ** — nó thông báo việc đã xảy ra, không phải mệnh lệnh.

---

## 8. Khái niệm cố ý KHÔNG có trong miền này

Ghi lại để không ai vô tình thêm vào:

- **Thanh toán** — không có `Payment`, `Transaction`, `Invoice`. `Booking` dừng ở
  `Confirmed`.
- **Người dùng chia sẻ chuyến đi** — `TripMembers` có trong CSDL nhưng phạm vi đồ
  án chỉ một người dùng trên một chuyến.
- **Đa tiền tệ** — mọi số tiền là VNĐ.
- **Đa thành phố trong một chuyến** — một `Trip` có đúng một điểm đến.
