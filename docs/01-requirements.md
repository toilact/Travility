# 01 — Yêu cầu, Actor và Use Case

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) · [`00-project-overview.md`](./00-project-overview.md)
> **Nguồn:** rút gọn từ [`archive/Travility_System_Specification.md`](./archive/Travility_System_Specification.md)

---

## 1. Phát biểu bài toán

> Travility là hệ thống hỗ trợ lập kế hoạch du lịch thông minh trên nền Windows,
> cho phép người dùng khám phá địa điểm qua bản đồ đa lớp, tạo và quản lý chuyến
> đi, tìm kiếm phương tiện, quản lý ngân sách, nhận đề xuất dựa trên sở thích và
> đánh giá cộng đồng, tối ưu tuyến đường và xây dựng nhiều lịch trình cân bằng
> giữa chi phí và trải nghiệm.

Bài toán trọng tâm: tối thiểu hoá chi phí, quãng đường, thời gian di chuyển;
đồng thời tối đa hoá mức độ phù hợp, chất lượng địa điểm và trải nghiệm — trong
ràng buộc ngân sách, thời gian và giờ hoạt động.

---

## 2. Actor

| Actor | Quyền |
|---|---|
| **Traveler** | Tạo/sửa `Trip`, xem đề xuất, sinh `Itinerary`, `CheckIn`, ghi `Expense`, xem `TravelPassport` |
| **Admin** | Quản lý `Place`, người dùng, `Achievement`, `TransportOptions`, xem thống kê |

---

## 3. Danh sách Use Case

### Traveler

| Mã | Use Case | Tài liệu |
|---|---|---|
| UC-01 | Đăng ký / Đăng nhập | `11` |
| UC-02 | Khám phá địa điểm trên bản đồ | `06` |
| UC-03 | Bật/tắt lớp bản đồ | `06` |
| UC-04 | Xem chi tiết địa điểm | `06` |
| UC-05 | Tạo chuyến đi (wizard) | `08` |
| UC-06 | Khai báo sở thích | `08` |
| UC-07 | Tìm và chọn phương tiện | `08` |
| UC-08 | Ghi nhận booking | `08` |
| UC-09 | Nhận Top-K đề xuất khách sạn / nhà hàng / địa điểm | `07` |
| UC-10 | Thêm/xoá địa điểm khỏi chuyến đi | `08` |
| UC-11 | Tối ưu tuyến đường | `07` |
| UC-12 | Sinh 3 phương án lịch trình | `07` |
| UC-13 | So sánh và chọn phương án | `07` |
| UC-14 | Phân bổ ngân sách | `08` |
| UC-15 | Ghi nhận chi tiêu | `08` |
| UC-16 | Xem dashboard ngân sách | `08` |
| UC-17 | Check-in tại địa điểm | `09` |
| UC-18 | Xem huy hiệu | `09` |
| UC-19 | Xem Travel Passport | `09` |
| UC-20 | Trò chuyện với AI Assistant | `10` |
| UC-21 | Xuất lịch trình ra PDF/Excel | `14` |

### Admin

| Mã | Use Case |
|---|---|
| UC-22 | Quản lý địa điểm (CRUD) |
| UC-23 | Quản lý người dùng |
| UC-24 | Quản lý huy hiệu |
| UC-25 | Xem thống kê |

---

## 4. User flow tổng thể

```
ĐĂNG NHẬP
   ▼
DASHBOARD
   ▼
CHỌN ĐIỂM ĐẾN → NGÀY ĐI/VỀ → NGÂN SÁCH → SỞ THÍCH
   ▼
CHỌN PHƯƠNG TIỆN → CHỌN KHÁCH SẠN (Top-K)
   ▼
CHỌN ĐỊA ĐIỂM → TỐI ƯU TUYẾN → SINH LỊCH TRÌNH
   ▼
        ┌───────────┼───────────┐
   Budget      Balanced    Experience
        └───────────┼───────────┘
   ▼
XÁC NHẬN CHUYẾN ĐI
   ▼
TRAVEL MODE ── Map · Budget · CheckIn · AI Assistant
   ▼
TRAVEL PASSPORT
```

---

## 5. Phạm vi

### Trong phạm vi

```
Smart Map + Map Layers          Top-K Recommendation + Bayesian
Trip Creation (wizard)          Route Optimization (NN + 2-opt)
Budget + Expense + Wallet       Itinerary Generation (3 phương án)
Booking (mô phỏng)              CheckIn (2 chế độ vị trí)
Achievements + Passport         AI Assistant (7 tool)
Admin CRUD + thống kê           Xuất báo cáo PDF/Excel
SQL Server · EF · LINQ · ADO.NET
```

### Ngoài phạm vi (nêu rõ trong báo cáo)

```
Thanh toán thật                 Đặt vé thật qua nhà cung cấp
Giá vé máy bay/khách sạn real-time
Dữ liệu giao thông real-time    Đa tiền tệ
Đa thành phố trong một chuyến   Chia sẻ chuyến đi giữa nhiều người dùng
```

---

## 6. Yêu cầu phi chức năng

| | |
|---|---|
| Hiệu năng | Tối ưu lịch trình xong dưới **3 giây** với 150 `Place` |
| Ngoại tuyến | Bản đồ hiện được khi **mất mạng** (tile đã cache) |
| Ngoại tuyến | Chatbot demo được khi mất mạng (chế độ phát lại) |
| Bảo mật | Mật khẩu **hash**, không lưu plaintext |
| Bảo mật | API key **không** nằm trong git |
| Dữ liệu | Mọi số tiền dùng `decimal` |

---

## 7. Trung thực về phạm vi

Những chỗ đặc tả gốc mô tả tham vọng hơn thực tế triển khai. Báo cáo phải dùng
cột bên phải:

| Đừng viết | Hãy viết |
|---|---|
| "Triển khai TSP with Time Windows" | "Heuristic Nearest Neighbor + 2-opt, đối chứng brute-force với n ≤ 8" |
| "Pareto Optimal Solutions" | "Ba preset trọng số theo `TravelStyle`" |
| "Tìm kiếm phương tiện" | "Mô phỏng tìm kiếm trên dữ liệu tự xây" |
| "Tích hợp Google Places" | "Dataset tự xây, 150 `Place` khu vực Đà Nẵng" |
| "Check-in bằng GPS" | "Hai chế độ định vị: API hệ điều hành và giả lập" |
| "Khoảng cách đường thật" | "Xấp xỉ Haversine × 1.3; đường thật ở bước hiển thị" |

Xem mục "Thuật ngữ cấm dùng" trong [`CONTEXT.md`](../CONTEXT.md).
