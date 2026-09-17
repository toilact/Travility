# Daily To-Do Checklist – Travility

> File này dùng để theo dõi tiến độ theo nhóm và theo từng tuần. Focus chính của bạn là phần Map / UI / trải nghiệm người dùng.
> Theo chuẩn dự án: UI không làm logic DB, không làm engine, không truy cập DbContext trực tiếp.

---

## 1. Quy tắc bắt buộc mỗi ngày

- [ ] Đọc lại 1 file chuẩn của nhóm trước khi code: [CONTEXT.md](CONTEXT.md) hoặc [docs/00-project-overview.md](docs/00-project-overview.md)
- [ ] Kiểm tra phần mình đang làm thuộc UI hay logic nghiệp vụ
- [ ] Nếu là Map/UI, đảm bảo không gọi DbContext trực tiếp
- [ ] Nếu là Map bridge, kiểm tra contract C# ↔ JS ổn định
- [ ] Nếu làm validation, dùng `ErrorProvider`, không dùng `MessageBox` cho validation lỗi
- [ ] Nếu thao tác > 1 giây: `WaitCursor` + vô hiệu nút
- [ ] Build / test tối thiểu theo mục mình đang làm

---

## 2. Daily checklist template

### Ngày hôm nay

- [ ] Mục tiêu chính của ngày:
- [ ] Mục tiêu phụ:
- [ ] Blocker / rào cản:
- [ ] Cần hỏi / trao đổi với ai:

### Kiểm tra tiến độ

- [ ] Code chạy được trên máy cá nhân
- [ ] Build không lỗi
- [ ] UI đúng chuẩn naming và layout
- [ ] Không vi phạm kiến trúc WinForms → Core → Data
- [ ] Có check lại với file đặc tả liên quan

### Kết quả trước khi kết thúc ngày

- [ ] Có file / commit / thay đổi rõ ràng
- [ ] Ghi lại gì học được hôm nay
- [ ] Có thứ cần làm tiếp ngày mai

---

## 3. Tuần 1 – Foundation: Map + UI + Routing cơ bản

### Mục tiêu cuối tuần

- [ ] Login / interface nền chạy được
- [ ] Map load được, marker tĩnh hiển thị
- [ ] WebView2 ↔ C# bridge hoạt động
- [ ] Routing Engine cơ bản chạy được
- [ ] Dữ liệu demo có thể hiển thị trên Map

### Checklist công việc

#### UI / form

- [ ] LoginForm / MainForm cơ bản dựng xong
- [ ] Dùng layout phù hợp, không đặt tọa độ tuyệt đối
- [ ] Tạo tên control đúng theo tiêu chuẩn `btn`, `txt`, `lbl`, `cbo`, `dgv`, ...
- [ ] Form có `AutoScaleMode = Dpi`
- [ ] Màu và font theo chuẩn giao diện

#### Map / WebView2

- [ ] WebView2 được nhúng vào Form
- [ ] `map.html` và JS/CSS có trong local project
- [ ] Không dùng CDN cho Leaflet
- [ ] Marker tĩnh hiển thị đúng vị trí
- [ ] Click marker gửi `placeId` về C#
- [ ] C# nhận tín hiệu JS và mở PlaceDetail thử nghiệm
- [ ] Bản đồ center ở TP.HCM

#### Bridge contract

- [ ] Xác nhận payload JSON chuẩn C# → JS
- [ ] Xác nhận payload JS → C# chuẩn
- [ ] Kiểm tra `mapReady`, `markerClick`, `mapClick`
- [ ] Không đổi contract giữa chừng nếu chưa báo nhóm

#### Routing cơ bản

- [ ] Engine cơ bản nhận `List<Place>`
- [ ] Dùng khoảng cách Haversine × 1.3 theo quy định
- [ ] Chạy được tính thứ tự điểm dừng cơ bản
- [ ] Nếu có route, map vẽ được đường đơn giản

### Definition of done – Tuần 1

- [ ] Build được
- [ ] Map hiển thị marker tĩnh
- [ ] Có thể click marker / debug payload
- [ ] Routing engine đơn giản chạy được
- [ ] Không vi phạm ranh giới UI vs Core

---

## 4. Tuần 2 – Trip Wizard + dữ liệu demo + ngân sách cơ bản

### Mục tiêu cuối tuần

- [ ] Có dữ liệu demo đủ dùng
- [ ] Trip Wizard UI hoạt động
- [ ] Validation đúng
- [ ] Tính tiền cơ bản đúng
- [ ] Map thể hiện các địa điểm theo danh sách trip

### Checklist công việc

#### Trip Wizard UI

- [ ] Bước 1: điểm đến, ngày đi/đến, số người, ngân sách
- [ ] Bước 2: 7 thanh sở thích
- [ ] Bước 3: chọn phương tiện
- [ ] Bước 4: chọn khách sạn / địa điểm gợi ý
- [ ] Bước 5: xem lại và xác nhận
- [ ] Validation cho ngày về > ngày đi, số người ≥ 1, ngân sách > 0

#### Ngân sách và pricing

- [ ] Hiểu rõ `Budget`, `PlannedCost`, `ActualSpent`
- [ ] `PricingUnit` đúng: `PerPerson`, `PerRoom`, `PerTrip`
- [ ] Khách sạn không tính theo số người sai
- [ ] Giao diện hiển thị đúng 3 con số

#### Dữ liệu demo

- [ ] Có đủ 120–150 Place nếu có thể
- [ ] Mỗi Place có latitude, longitude, category, rating, visit duration, tags
- [ ] Dữ liệu đọc được như một câu chuyện
- [ ] Không dùng tên `place1`, `place2` trong test hoặc demo

### Definition of done – Tuần 2

- [ ] TripWizard có flow rõ, không lỗi validation rõ ràng
- [ ] Dữ liệu demo hiển thị trên Map hợp lý
- [ ] Tính tiền cơ bản đúng
- [ ] UI đã ăn khớp với dữ liệu thật / giả lập

---

## 5. Tuần 3 – Itinerary + Recommendation + so sánh phương án

### Mục tiêu cuối tuần

- [ ] Itinerary sinh được
- [ ] Có 3 phương án: Budget / Balanced / Experience
- [ ] Map vẽ được lịch trình theo ngày
- [ ] So sánh 3 phương án trên UI
- [ ] Tính tiền và ràng buộc ngân sách chạy đúng

### Checklist công việc

#### UI Itinerary

- [ ] Màn hình so sánh 3 phương án hiển thị rõ
- [ ] Mỗi phương án có cost / distance / experience
- [ ] Chọn phương án chính thức được
- [ ] Thứ tự điểm dừng hiển thị rõ

#### Map route

- [ ] Mỗi ngày có màu riêng
- [ ] Dùng route plan đúng để vẽ tuyến trên bản đồ
- [ ] Không gọi OSRM trong vòng lặp tối ưu
- [ ] Vẽ tuyến chỉ ở bước cuối

#### Business rules

- [ ] `Itinerary` không vượt quá ngân sách
- [ ] Không xếp hoạt động ngoài giờ mở cửa
- [ ] Không có hoạt động trùng giờ trong một ngày
- [ ] Tổng thời lượng ≈ khả dụng trong ngày

### Definition of done – Tuần 3

- [ ] Sinh được 3 Itinerary khác nhau
- [ ] Map render được từng ngày rõ ràng
- [ ] So sánh 3 phương án đúng và dễ hiểu
- [ ] Dữ liệu và UI đồng bộ với engine

---

## 6. Tuần 4 – Full flow + demo

### Mục tiêu cuối tuần

- [ ] Full flow từ Login → Trip → Recommendation → Routing → Itinerary → CheckIn → TravelPassport
- [ ] UI demo mạch lạc, không đứt quãng
- [ ] Check-in hoạt động ở chế độ thật / giả lập
- [ ] Passport hiển thị dữ liệu tích luỹ

### Checklist công việc

#### Full flow

- [ ] Login thành công
- [ ] Tạo Trip xong
- [ ] Map hiển thị Place đề xuất
- [ ] Chọn method / điểm dừng / khách sạn đúng
- [ ] Tối ưu routing
- [ ] Sinh Itinerary
- [ ] Chọn 1 phương án
- [ ] Check-in thành công
- [ ] TravelPassport cập nhật

#### Demo polish

- [ ] Màu sắc rõ ràng, không lộn
- [ ] Không có thông báo lỗi mơ hồ
- [ ] `SIMULATED` hiện rõ khi dùng vị trí giả lập
- [ ] Form không bị treo khi thao tác nặng
- [ ] Build Release chạy được
- [ ] Offline map vẫn hiện được

### Definition of done – Tuần 4

- [ ] Hệ thống chạy end-to-end
- [ ] Demo mạch lạc
- [ ] Không có lỗi nghiêm trọng khi thầy bấm thử

---

## 7. Tuần cuối – sửa lỗi, polish và thuyết trình

### Mục tiêu

- [ ] Chỉ sửa lỗi và hoàn thiện demo
- [ ] Không viết tính năng mới
- [ ] Luyện thuyết trình
- [ ] Chuẩn bị backup khi gặp lỗi

### Checklist

- [ ] Fix bug đã biết từ tuần trước
- [ ] Kiểm tra lại flow demo từng bước
- [ ] Kiểm tra build Release
- [ ] Kiểm tra offline map
- [ ] Kiểm tra chatbot nếu đã ổn
- [ ] Kiểm tra achievement nâng cao nếu còn thời gian
- [ ] Chuẩn bị video dự phòng / backup demo
- [ ] Tập nói 3 lần với thời lượng hợp lý

---

## 8. Checklist “không làm” để tránh sai ranh giới

- [ ] Không gọi `DbContext` từ Form
- [ ] Không xử lý logic DB trong UI
- [ ] Không viết engine trong WinForms
- [ ] Không hardcode API key
- [ ] Không dùng `float` / `double` cho tiền
- [ ] Không đổi contract C# ↔ JS giữa chừng
- [ ] Không commit `bin/`, `obj/`, `packages/`, `.vs/`
- [ ] Không mở Designer của form người khác
- [ ] Không làm chatbot nếu phần lõi chưa ổn

---

## 9. Mẫu note cuối ngày

### Hôm nay làm được

- [ ]
- [ ]
- [ ]

### Hôm nay gặp lỗi

- [ ]
- [ ]
- [ ]

### Cần làm ngày mai

- [ ]
- [ ]
- [ ]

### Ghi chú cho nhóm

-

---

## 10. Gợi ý ưu tiên làm thực tế

Nếu thời gian gấp, ưu tiên theo thứ tự sau:

1. Login + UI nền
2. Map load + marker tĩnh + bridge
3. Trip Wizard UI
4. Route & itinerary trên map
5. So sánh 3 phương án
6. Check-in + passport
7. Demo polish
8. Chatbot và achievement nâng cao nếu còn thời gian

---

## 11. Kết luận

File này là checklist hoạt động cho bạn: làm đúng phần “giao diện và trải nghiệm”, nhưng không vượt qua ranh giới kỹ thuật mà nhóm đã đặt ra. Nếu phần lõi chưa ổn, ưu tiên nền tảng và demo, không làm tính năng phụ.
