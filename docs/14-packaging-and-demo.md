# 14 — Đóng gói và kịch bản demo

> **Đọc trước:** [`00-project-overview.md`](./00-project-overview.md)
> **Làm ở tuần 6, nhưng chuẩn bị từ tuần 4.**
> **Ngày bảo vệ: 23/10/2026.**

---

## 1. Nguyên tắc

> **Đóng băng tính năng ngày 16/10.** Tuần cuối không viết tính năng mới.

Đồ án điểm thấp hiếm khi vì thiếu chức năng — thường vì demo vỡ, vì lỗi khi thầy
bấm thử, vì không ai tập nói trước.

---

## 2. Chạy trên máy trắng

Máy demo có thể không có sẵn những thứ máy phát triển đang có. Kiểm tra đủ bốn
điều kiện:

| Phụ thuộc | Cách xử lý |
|---|---|
| **SQL Server** | Script `database/schema.sql` + `seed_*.sql` chạy sạch trên DB trống |
| **WebView2 Runtime** | Đóng gói Evergreen Bootstrapper; nếu khởi tạo lỗi thì fallback sang `ListView` + ảnh bản đồ tĩnh |
| **.NET Framework** | Phiên bản đích ≤ phiên bản có sẵn trên Windows của máy demo |
| **Tile bản đồ** | Tải sẵn về `MapAssets/tiles/`, **thử với Wi-Fi tắt** |

Diễn tập thật: mượn một laptop khác, cài từ đầu, chạy. Làm việc này ở **tuần 5**,
không phải tối 22/10.

---

## 3. Xuất báo cáo (UC-21)

Ăn rubric, rẻ về công sức:

- Xuất `Itinerary` ra **PDF** hoặc **Excel** — lịch trình theo ngày, kèm chi phí.
- Xuất bảng `Expense` ra Excel.

Dùng thư viện có sẵn qua NuGet. Không cần đẹp, cần chạy.

---

## 4. Bảo hiểm demo — chuẩn bị trước 16/10

- [ ] Tile bản đồ Đà Nẵng đã tải, **đã thử với Wi-Fi tắt**
- [ ] Một phiên chatbot đã ghi trong `ToolExecutions`, **phát lại được offline**
- [ ] **Video demo dự phòng** quay đầy đủ kịch bản mục 5
- [ ] Dữ liệu demo đẹp: tên địa điểm thật, ảnh thật, rating hợp lý
- [ ] Tài khoản demo có sẵn vài `Trip` cũ và vài `Achievement` — `TravelPassport`
      không được trống
- [ ] Bản build **Release** đã chạy thử, không chỉ Debug
- [ ] Tập demo **ít nhất 3 lần**, bấm giờ

---

## 5. Kịch bản demo

Demo là một **câu chuyện liền mạch**, không phải đi qua từng Form.

```
 1. Đăng nhập
 2. Smart Map → bật/tắt vài lớp → click marker → xem chi tiết
 3. Tạo Trip: Đà Nẵng, 4 ngày, 2 người, 8 triệu
 4. Kéo 7 thanh sở thích (Biển 5, Ẩm thực 5, Lịch sử 2)
 5. Top-5 khách sạn → CHỈ RA vì sao khách sạn 4.8★/8200 review
    xếp trên khách sạn 5.0★/4 review          ◄── khoảnh khắc đắt nhất
 6. Sinh 3 phương án → so sánh → chọn Balanced
 7. XEM LỊCH TRÌNH TRÊN BẢN ĐỒ, mỗi ngày một màu, mỗi ngày một cụm
                                              ◄── bằng chứng nhìn thấy được
 8. Bật chế độ giả lập → Check-in → nhận Achievement
 9. Mở Travel Passport
10. Chatbot: "Tôi còn 1 triệu, tìm quán hải sản gần khách sạn dưới 300k"
    → thêm vào lịch trình → ngân sách tự cập nhật
```

**Bước 5 và bước 7 quyết định điểm số.** Bước 5 chứng minh Recommendation Engine
không phải sắp xếp theo rating; bước 7 chứng minh Itinerary Engine thật sự chia
cụm địa lý. Tập kỹ hai bước này, nói chậm ở hai bước này.

---

## 6. Câu hỏi hội đồng hay hỏi

| Câu hỏi | Trả lời |
|---|---|
| "Dữ liệu lấy ở đâu?" | Dataset tự xây, 150 `Place` Đà Nẵng, nhập tay. Kiến trúc `IPlaceProvider` cho phép thay bằng nguồn API thật mà không sửa engine. |
| "Sao không dùng khoảng cách đường thật?" | Engine đánh giá hàng vạn phương án nên dùng Haversine có hệ số hiệu chỉnh; đường thật dùng ở bước hiển thị tuyến. |
| "Sao có chế độ giả lập vị trí?" | Ứng dụng desktop không định vị chính xác, và chúng em đang trong phòng thi cách địa điểm hàng km. Nên tách rõ hai chế độ. |
| "Thuật toán tối ưu là gì?" | Pipeline ba tầng: chọn tham lam theo tỉ lệ điểm/chi phí, chia ngày theo cụm địa lý, xếp thứ tự bằng NN + 2-opt có kiểm tra khung giờ. |
| "Ba phương án có phải Pareto không?" | Không. Đó là ba preset trọng số. Pareto Front cần lọc dominance trên tập nghiệm lớn — nhóm em không triển khai. |
| "Dùng EF rồi sao còn ADO.NET?" | EF cho CRUD nghiệp vụ; ADO.NET cho `DistanceMatrix` và báo cáo thống kê qua stored procedure. |
| "Nhóm có test không?" | Có, N unit test cho ba engine: ràng buộc ngân sách, giờ mở cửa, không trùng giờ, đối chứng 2-opt với brute-force. |
| "Mật khẩu lưu thế nào?" | PBKDF2 với salt riêng cho mỗi người dùng, 10.000 vòng lặp. |

Nguyên tắc trả lời: **nói đúng những gì đã làm.** Trung thực về giới hạn được
đánh giá cao hơn nói quá rồi bị bắt.

---

## 7. Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Chỉ thử trên máy phát triển | Diễn tập trên máy trắng ở tuần 5 |
| Chỉ build Debug | Thử Release trước tuần 6 |
| Không có video dự phòng | Quay trước 16/10 |
| `TravelPassport` trống trơn lúc demo | Seed sẵn dữ liệu cho tài khoản demo |
| Viết tính năng mới ở tuần 6 | Đóng băng 16/10 |
| Không ai tập nói | Tập 3 lần, bấm giờ |
