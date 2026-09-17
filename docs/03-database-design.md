# 03 — Thiết kế cơ sở dữ liệu

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) · [`00-project-overview.md`](./00-project-overview.md)
> **Sở hữu:** A — **chỉ A được sửa `database/schema.sql`**
> **Quyết định nền:** [ADR-0001 — Database First](./adr/0001-database-first.md)

---

## 1. Quy tắc sở hữu

`database/schema.sql` là **một file, một người sở hữu**. Ai cần thêm bảng hoặc
cột thì báo A, A sửa, A thông báo cả nhóm regenerate EF model.

Mọi seed phải là file `.sql` trong git. Không seed bằng tay trong SSMS rồi quên.

---

## 2. Các nhóm bảng

```
User          Users · Roles · UserProfiles · UserPreferences · TravelWallets
Places        Places · PlaceCategories · PlaceTypes · PlaceImages
              PlaceRatings · PlaceReviews · ExternalPlaceMappings
Travel        Trips · TripMembers · TripPreferences · TransportOptions
              Bookings · TripPlaces
Itinerary     Itineraries · ItineraryDays · ItineraryItems
              RoutePlans · RouteSegments · DistanceMatrix
Finance       TravelWallets · TripBudgets · BudgetCategories
              BudgetAllocations · Expenses
Gamification  CheckIns · Achievements · UserAchievements · TravelPassports
AI            ChatSessions · ChatMessages · ToolExecutions
```

---

## 3. Bảng `Places` — hợp đồng trường bắt buộc

Đây là bảng quan trọng nhất: ba engine đều đọc từ nó. Thiếu một trường ở đây thì
một engine không chạy được.

| Trường | Kiểu | Phục vụ | Ghi chú |
|---|---|---|---|
| `PlaceId` | `int` PK | | |
| `Name` | `nvarchar(200)` | | |
| `Latitude`, `Longitude` | `float` | Map, Routing, CheckIn | Đặc tả gốc **thiếu** |
| `CategoryId` | `int` FK | Map Layers, lọc | |
| `Rating` | `decimal(2,1)` | Recommendation | 0.0–5.0 |
| `ReviewCount` | `int` | Recommendation | Đầu vào Bayesian |
| `TicketPrice` | `decimal(18,2)` | Recommendation, Budget | `decimal`, không `float` |
| `OpenTime`, `CloseTime` | `time` | Itinerary | Một khung/ngày, giống nhau cả tuần |
| **`VisitDuration`** | `int` (phút) | Itinerary, Routing | Gán tay theo category |
| **`PreferenceTags`** | `nvarchar(100)` | Recommendation | 1–3 tag, phân tách bằng dấu phẩy |
| `IsMealPlace` | `bit` | Itinerary | Để chèn giờ ăn |
| `PricingUnit` | `tinyint` | Budget | PerPerson / PerRoom / PerTrip |

### `VisitDuration` gợi ý theo category

```
Bảo tàng        90      Bãi biển       180     Quán ăn         60
Cà phê          45      Khu vui chơi   240     Chùa/đền        60
Chợ/mua sắm     90      Điểm ngắm cảnh 45
```

Không có API nào cho con số này — gán tay là cách đúng, và hoàn toàn chấp nhận
được về mặt học thuật nếu ghi rõ trong báo cáo.

### `PreferenceTags`

Lấy từ **đúng bảy nhóm** và không được lệch, vì chúng phải khớp với thanh trượt
sở thích trong Trip Wizard:

```
Biển · Ẩm thực · Lịch sử · Văn hóa · Giải trí · Mua sắm · Thiên nhiên
```

Không có trường này thì `PreferenceScore` trong Recommendation Engine không tính
được — tức là engine mất một trong sáu thành phần.

---

## 4. Thay đổi so với đặc tả gốc

| Thay đổi | Lý do |
|---|---|
| **Bỏ** bảng `PlaceOpeningHours` | Giờ mở cửa là một khung/ngày → gộp vào `Places` |
| **Thêm** `Latitude`, `Longitude` vào `Places` | Đặc tả gốc thiếu, nhưng Map/Routing/CheckIn đều cần |
| **Thêm** `VisitDuration`, `PreferenceTags`, `IsMealPlace`, `PricingUnit` | Ba engine không chạy được nếu thiếu |
| **Thêm** `IsSimulated` vào `CheckIns` | Phân biệt dữ liệu demo — xem [ADR-0006](./adr/0006-simulated-location-provider.md) |
| **Thêm** bảng `DistanceMatrix` | Tính sẵn, truy vấn qua stored procedure |
| Làm rõ `PlaceRatings` vs `PlaceReviews` | `PlaceRatings` = điểm tổng hợp (cache); `PlaceReviews` = từng review |
| Thống nhất **`Reserve`** | Đặc tả gốc lẫn lộn `Reserve` / `Emergency` |

---

## 5. Dataset

**120–150 `Place`, một thành phố (TP.HCM)**, nhập tay bằng Excel → script import.

Đủ để Top-K có ý nghĩa (chọn 5 trong 40 khách sạn là bài toán thật) và routing có
không gian nghiệm, mà một người làm xong trong 5–7 ngày.

Báo cáo ghi rõ: *dataset tự xây dựng phục vụ mục đích học thuật*. Không nói
"tích hợp Google Places".

`ExternalPlaceMappings` giữ lại trong schema để thể hiện kiến trúc không phụ
thuộc một nguồn dữ liệu, dù phiên bản đồ án chưa đồng bộ với nhà cung cấp nào.

---

## 6. Stored procedure (dùng ADO.NET — Bài 8)

Tối thiểu ba cái, để phần ADO.NET có lý do tồn tại thay vì gượng ép:

| Procedure | Mục đích |
|---|---|
| `sp_GetDistanceMatrix` | Lấy ma trận khoảng cách cho một tập `PlaceId` |
| `sp_TopPlacesAddedToTrips` | Thống kê Admin: địa điểm được thêm nhiều nhất |
| `sp_AvgExpenseByCategory` | Thống kê Admin: chi tiêu trung bình theo nhóm |

---

## 7. Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Dùng `float`/`double` cho tiền | **Luôn** `decimal(18,2)` |
| Sửa schema giữa chừng làm hỏng EF model người khác | Báo cả nhóm, regenerate, merge ngay |
| Seed bằng tay trong SSMS, không có script | Mọi seed là file `.sql` trong git |
| Quên `Latitude`/`Longitude` vì đặc tả gốc không có | Kiểm tra lại hợp đồng trường ở mục 3 |
| Người dataset đi thu thập trước khi chốt trường | Đưa mục 3 cho họ **trước** khi bắt đầu |
| Đặt `VisitDuration` bằng 0 cho qua | Itinerary Engine sẽ nhồi 10 địa điểm vào một ngày |
