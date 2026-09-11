# ADR-0008 — Ba con số tiền và cờ `PricingUnit`

- **Trạng thái:** Chấp nhận
- **Ngày:** 2026-09-11

## Bối cảnh

Đặc tả gốc mô tả dashboard ngân sách bằng một thanh tiến độ "Spent / Planned",
gộp hai đại lượng khác bản chất vào một chỗ. Nó cũng định nghĩa `TravelWallet`
và `TripBudget` nhưng không nói rõ khi nào tiền bị trừ.

Ngoài ra, đặc tả có trường `Số người = 2` nhưng không nói chi phí nào nhân theo
số người — mà **không phải chi phí nào cũng nhân**.

Tiền là chỗ hội đồng soi kỹ nhất và cũng là chỗ dễ sai nhất.

## Quyết định

### Ba con số, ba nguồn

| Con số | Nguồn | Ý nghĩa |
|---|---|---|
| `Budget` | Người dùng nhập | Hạn mức tự đặt |
| `PlannedCost` | Hệ thống tính từ `Itinerary` | Dự kiến sẽ tiêu |
| `ActualSpent` | Cộng dồn từ `Expense` | Đã tiêu thật |

Dashboard vẽ **hai thanh chồng nhau**, không gộp làm một.

### Ví là hạn mức, không phải két giữ tiền

- Tạo `Trip` **không** trừ `TravelWallet`.
- Chỉ `Expense` mới làm giảm số dư.
- Tổng `Budget` các chuyến đang hoạt động vượt số dư ví → **cảnh báo, không chặn**.

### Cờ `PricingUnit`

```
PerPerson  → × số người        vé tham quan, ăn uống, vé máy bay
PerRoom    → × số phòng × đêm   khách sạn
PerTrip    → × số chuyến        taxi, xăng xe
```

## Hệ quả

- Chuyến hai người tính đúng: vé nhân đôi, khách sạn không nhân đôi. Thiếu cờ này
  thì ngân sách khách sạn sai gấp đôi — lỗi lộ ngay khi thầy bấm thử.
- Người dùng so sánh được "định làm gì" với "đã làm gì", là giá trị thật của một
  ứng dụng quản lý chi tiêu du lịch.
- Chặn mềm thay vì chặn cứng giúp demo trôi chảy mà vẫn thể hiện được ràng buộc.
- **Ràng buộc code:** mọi kiểu dữ liệu tiền tệ là `decimal`, không bao giờ
  `float` hay `double`.

## Phương án đã loại

**Một con số duy nhất** — đơn giản nhất nhưng không phân biệt được dự kiến với
thực chi, làm mất ý nghĩa của dashboard.

**Chặn cứng khi vượt ví** — nghiêm ngặt hơn về mặt nghiệp vụ, nhưng làm demo khó
chịu mà không chứng minh thêm điều gì.

**Bỏ `PricingUnit`, nhân mọi chi phí theo số người** — nhanh hơn nhưng sai về
nghiệp vụ ở chỗ dễ bị kiểm tra nhất.
