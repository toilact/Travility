# 12 — Chiến lược kiểm thử

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) · [`07-engines.md`](./07-engines.md)
> **Sở hữu:** D
> ***"Nhóm em có 20 unit test cho engine"* là câu rất ít nhóm nói được khi bảo vệ.**

---

## 1. Test cái gì, không test cái gì

| Test | Vì sao |
|---|---|
| ✅ **Ba engine** | Logic thuần, không UI, không DB — dễ test nhất, giá trị cao nhất |
| ✅ **Hàm tính tiền** | `PricingUnit`, phân bổ ngân sách — chỗ dễ sai và dễ bị soi |
| ✅ **Hàm Haversine** | Một hàm, kiểm chứng được bằng số liệu thật |
| ❌ **Form** | Cần chạy UI, tốn công gấp nhiều lần giá trị thu được |
| ❌ **EF/Repository** | Thực chất là test EF chứ không phải test code của nhóm |
| ❌ **Gọi LLM** | Không tất định, tốn token, phụ thuộc mạng |

Sáu tuần không đủ để test mọi thứ. Test đúng chỗ có giá trị nhất.

---

## 2. Công cụ

`tests/Travility.Tests` — **NUnit 3** cài qua NuGet (`NUnit 3.14.0`, `NUnit3TestAdapter 4.6.0`, `Microsoft.NET.Test.Sdk 17.11.1`). Không dùng
framework mock bên ngoài; sử dụng fake in-memory test doubles tự viết. Engine nhận `List<Place>` thuần nên không cần mock gì.

Đặt tên test theo mẫu `Phải_<kỳ vọng>_Khi_<điều kiện>`:

```csharp
[Test]
public void Phai_KhongVuotNganSach_Khi_SinhLichTrinh() { ... }
```

Tên tiếng Việt được — người đọc là nhóm và hội đồng.

---

## 3. Danh sách test bắt buộc

Tối thiểu 15 test xanh trước khi bảo vệ. Tám cái dưới đây là bắt buộc, mỗi cái
gắn với một quy tắc nghiệp vụ trích dẫn được.

| # | Test | Ràng buộc |
|---|---|---|
| 1 | Ngân sách 5 triệu **không bao giờ** sinh `Itinerary` > 5 triệu | BR-11 |
| 2 | Không `Place` nào bị xếp ngoài `[OpenTime, CloseTime]` | BR-12 |
| 3 | Không hai `ItineraryItem` nào trùng giờ trong cùng một ngày | BR-12 |
| 4 | Bayesian: 5.0★/4 review xếp **dưới** 4.8★/8200 review | — |
| 5 | 2-opt cho kết quả **không tệ hơn** Nearest Neighbor | — |
| 6 | Với n ≤ 8, NN+2opt cách brute-force không quá 15% | — |
| 7 | Ba `TravelStyle` cho ba `Itinerary` khác nhau trên cùng dữ liệu | — |
| 8 | Tổng `VisitDuration` + `TravelTime` ≤ số giờ khả dụng trong ngày | — |

Thêm bốn test tiền bạc:

| # | Test | Ràng buộc |
|---|---|---|
| 9 | Khách sạn `PerRoom` **không** nhân theo số người | BR-06 |
| 10 | Vé tham quan `PerPerson` nhân đúng số người | BR-06 |
| 11 | `Booking` chuyển `Cancelled` thì `PlannedCost` giảm | BR-07 |
| 12 | Tổng tỉ lệ phân bổ khác 100% thì bị từ chối | BR-08 |

---

## 4. Dữ liệu test

Engine test trên `List<Place>` **tạo trong code**, không đọc CSDL. Viết một
`TestPlaceBuilder` nhỏ để dựng dữ liệu đọc được:

```csharp
var places = new[]
{
    Place("Bảo tàng Chàm", lat: 16.06, lng: 108.22, rating: 4.5m,
          reviews: 1200, visitMinutes: 90, open: "08:00", close: "17:00"),
    Place("Biển Mỹ Khê",   lat: 16.06, lng: 108.24, rating: 4.7m,
          reviews: 8200, visitMinutes: 180, open: "05:00", close: "19:00"),
};
```

Dữ liệu test phải **đọc được như một câu chuyện**. Test dùng `place1`, `place2`
không ai hiểu khi nó đỏ.

---

## 5. Test thủ công — bảng test case để nộp

Ngoài unit test, cần bảng test case thủ công cho phần giao diện. Tối thiểu 20
dòng, mẫu:

| ID | Chức năng | Bước | Dữ liệu | Kỳ vọng | Kết quả |
|---|---|---|---|---|---|
| TC-01 | Đăng nhập | Nhập sai mật khẩu | `abc`/`wrong` | Báo lỗi, không vào được | ☐ |
| TC-02 | Tạo chuyến | Ngày về trước ngày đi | 20/10 → 18/10 | `ErrorProvider` báo BR-01 | ☐ |
| TC-03 | Ngân sách | Chuyến 2 người, 1 phòng | 2 người, 3 đêm | Khách sạn không nhân đôi | ☐ |
| TC-04 | Bản đồ | Ngắt mạng, mở bản đồ | — | Bản đồ vẫn hiện | ☐ |
| TC-05 | Check-in | Ngoài bán kính | cách 5 km | Từ chối, báo rõ lý do | ☐ |

---

## 6. Kiểm tra trước khi bảo vệ

- [ ] Toàn bộ unit test xanh
- [ ] Build **Release** chạy được, không chỉ Debug
- [ ] Chạy trên máy **chưa từng cài** (SQL Server, WebView2 Runtime)
- [ ] Ngắt mạng: bản đồ vẫn hiện, chatbot phát lại được
- [ ] Bảng test case thủ công đã tick hết

---

## 7. Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Viết test sau khi code xong hết | Viết cùng lúc với engine, tuần 1 đã có test đầu tiên |
| Test đọc CSDL thật | Dùng `List<Place>` trong code |
| Test phụ thuộc thứ tự chạy | Mỗi test tự dựng dữ liệu của nó |
| Chỉ test Debug | Kiểm tra cả Release trước tuần 6 |
| Bỏ test vì "hết thời gian" | Tám test bắt buộc ở mục 3 tốn khoảng một ngày, đổi lại một câu mạnh khi bảo vệ |
