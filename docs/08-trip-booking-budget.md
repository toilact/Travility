# 08 — Trip Builder, Booking, Ngân sách

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) (từ điển thuật ngữ) · [`00-project-overview.md`](./00-project-overview.md)
> **Người phụ trách:** C
> **Module nhiều form nhất, và là nơi duy nhất đụng tới tiền — nghiệp vụ phải chặt.**

---

## Mục tiêu

Cho phép người dùng tạo chuyến đi qua wizard, chọn phương tiện, ghi nhận booking,
phân bổ ngân sách và theo dõi chi tiêu.

---

## Sở hữu

```
Travility.WinForms/Trip/
├── TripWizardForm.cs            ← wizard nhiều bước
├── TransportSearchForm.cs
└── BookingForm.cs

Travility.WinForms/Budget/
├── BudgetForm.cs                ← phân bổ ngân sách
├── BudgetDashboardForm.cs
└── ExpenseForm.cs

Travility.Core/Services/
├── TripService.cs
└── BudgetService.cs
```

---

## Phụ thuộc

- Cần `schema.sql` + Repository của A (tuần 1–2).
- Cần `IItineraryEngine` của D để có `PlannedCost` (tuần 3).
- B gọi sang C qua nút *Thêm vào chuyến đi* ở `PlaceDetailForm`.

Tuần 1 làm được khung wizard mà chưa cần DB — dùng dữ liệu giả trong bộ nhớ.

---

## 1. Trip Wizard

Các bước theo đặc tả §5:

```
Bước 1  Điểm xuất phát · Điểm đến · Ngày đi · Ngày về · Số người · Ngân sách
Bước 2  Sở thích — 7 thanh trượt 1..5
        Biển · Ẩm thực · Lịch sử · Văn hóa · Giải trí · Mua sắm · Thiên nhiên
Bước 3  Chọn phương tiện
Bước 4  Chọn khách sạn (Top-K từ engine của D)
Bước 5  Xem lại và xác nhận
```

Bảy giá trị sở thích ở bước 2 là đầu vào của `PreferenceScore` trong
Recommendation Engine — chúng khớp với `PreferenceTags` của bảng `Places`.

**Validation bắt buộc** (`ErrorProvider`, ăn Bài 6):
ngày về > ngày đi · số người ≥ 1 · ngân sách > 0 · điểm đến khác điểm xuất phát.

---

## 2. Phương tiện và Booking

**Đây là dữ liệu mô phỏng, không phải API thật.** Không có API vé máy bay/tàu
miễn phí. Bảng `TransportOptions` do nhóm tự seed.

Gọi đúng tên trong UI và trong báo cáo: **"Mô phỏng tìm kiếm phương tiện"**.
Nói rõ ngay từ đầu, đừng để hội đồng tự phát hiện.

Luồng hỗ trợ đầy đủ: **Tìm kiếm → So sánh → Lựa chọn → Ghi nhận booking → Quản lý trạng thái**.
Không làm thanh toán thật.

```
BookingType    : Flight · Bus · Train · Hotel
BookingStatus  : Pending · Reserved · Confirmed · Cancelled
```

Chi phí phương tiện đi thẳng vào tổng ngân sách chuyến đi.

---

## 3. Ngân sách — quy tắc nghiệp vụ

### 3.1. Ba con số cho mỗi chuyến (KHÔNG phải một)

| Con số | Nguồn | Ý nghĩa |
|---|---|---|
| `Budget` | Người dùng nhập | Hạn mức tự đặt |
| `PlannedCost` | **Hệ thống tự tính** từ itinerary | Dự kiến sẽ tiêu |
| `ActualSpent` | Người dùng ghi nhận khi đi | Đã tiêu thật |

Dashboard vẽ **hai thanh chồng nhau** (Planned và Actual trên nền Budget),
không gộp làm một như đặc tả §10 đang mô tả.

### 3.2. Travel Wallet là hạn mức, không phải két giữ tiền

- Tạo chuyến đi **không** trừ ví.
- Chỉ `Expense` thật mới làm giảm số dư ví.
- Tổng `Budget` các chuyến đang hoạt động vượt số dư ví → **cảnh báo, không chặn**.
  Chặn cứng làm demo khó chịu mà không chứng minh thêm điều gì.

### 3.3. PricingUnit — chỗ dễ sai nhất

Không phải chi phí nào cũng nhân theo số người:

```
Vé tham quan, ăn uống  → PerPerson  → x số người
Khách sạn              → PerRoom    → x số phòng x số đêm
Vé máy bay/tàu/xe      → PerPerson  → x số người
Taxi, xăng xe          → PerTrip    → x số chuyến
```

Thiếu cờ này thì chuyến 2 người sẽ tính sai ngân sách khách sạn gấp đôi — lỗi lộ
ngay khi thầy bấm thử.

### 3.4. Phân bổ theo nhóm (đặc tả §9)

```
Transportation 25%  ·  Accommodation 30%  ·  Food 20%
Activities     15%  ·  LocalTransport  5%  ·  Reserve  5%
```

Đây là tỉ lệ **gợi ý mặc định**, người dùng sửa được. Tổng phải bằng 100%.

---

## 4. Event phải phát ra

Module này là nguồn của nhiều event (Bài 4):

```
BudgetChanged      ← khi đổi ngân sách hoặc phân bổ
ExpenseRecorded    ← khi ghi nhận chi tiêu
BookingConfirmed   ← khi xác nhận booking
HotelChanged       ← khi đổi khách sạn  → D tính lại lịch trình
```

Và phải **lắng nghe** `ItineraryGenerated` từ D để cập nhật `PlannedCost`.

---

## 5. Business Rules — đánh số để trích dẫn trong báo cáo

Đây là mục hội đồng soi kỹ nhất. Trích dẫn bằng mã `BR-NN` trong test case và báo cáo.

| Mã | Quy tắc |
|---|---|
| **BR-01** | Ngày về phải sau ngày đi |
| **BR-02** | Số người ≥ 1 và ngân sách > 0 |
| **BR-03** | Tạo `Trip` **không** trừ `TravelWallet` |
| **BR-04** | Chỉ `Expense` mới làm giảm số dư `TravelWallet` |
| **BR-05** | Tổng `Budget` các chuyến đang hoạt động vượt số dư ví → **cảnh báo, không chặn** |
| **BR-06** | Chi phí nhân theo `PricingUnit`: `PerPerson` / `PerRoom` / `PerTrip` |
| **BR-07** | `Booking` chuyển sang `Cancelled` thì trừ khỏi `PlannedCost` |
| **BR-08** | Tổng tỉ lệ phân bổ ngân sách phải bằng 100% |
| **BR-09** | Check-in hợp lệ khi `Distance ≤ CheckInRadius` (150 m) |
| **BR-10** | Mỗi `Achievement` chỉ trao một lần cho một người dùng |
| **BR-11** | `Itinerary` sinh ra phải có `TotalCost ≤ Budget` |
| **BR-12** | Không xếp hoạt động ngoài `[OpenTime, CloseTime]` |

BR-09 tới BR-12 được thực thi ở module khác (`09-checkin-gamification.md`,
`07-engines.md`) nhưng liệt kê ở đây để có một chỗ tra cứu duy nhất.

Quyết định nền cho BR-03 tới BR-06: [ADR-0008](./adr/0008-three-money-numbers-and-pricing-unit.md).

---

## Định nghĩa "xong"

- [ ] Wizard tạo được chuyến đi hoàn chỉnh, lưu vào DB
- [ ] Validation chặn được mọi đầu vào sai
- [ ] Chọn phương tiện và ghi nhận booking, đổi được trạng thái
- [ ] Dashboard hiện đúng cả ba con số, hai thanh tiến độ
- [ ] Chuyến 2 người tính đúng: vé x2, khách sạn không x2
- [ ] Ghi chi tiêu làm giảm số dư ví
- [ ] Vượt ngân sách thì cảnh báo, không crash

---

## Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Gộp `PlannedCost` và `ActualSpent` làm một | Tách từ đầu, đừng để refactor giữa chừng |
| Nhân mọi chi phí theo số người | Dùng `PricingUnit` |
| Dùng `float`/`double` cho tiền | Dùng `decimal`. Luôn. |
| Tính lại ngân sách bằng cách gọi trực tiếp engine | Dùng event, tránh phụ thuộc vòng |
| Wizard lưu DB ở từng bước → bỏ giữa chừng để lại rác | Giữ trong bộ nhớ, chỉ lưu ở bước cuối |
| Quên xử lý huỷ booking → tiền không hoàn | Quy tắc: `Cancelled` thì trừ khỏi `PlannedCost` |
