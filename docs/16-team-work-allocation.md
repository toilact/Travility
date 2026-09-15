# 16 — Phân công công việc nhóm

> Bản phân công đã thống nhất cho 4 thành viên trong 6 tuần thực hiện Travility.
> Phân công theo trách nhiệm đầu ra và mức độ rủi ro, không theo số giờ cố định.

## 1. Nguyên tắc phối hợp

1. Mỗi mảng có một người sở hữu đầu ra cuối cùng.
2. Người sở hữu nghiệp vụ quyết định quy tắc; người sở hữu giao diện quyết định cách hiển thị.
3. Các interface trong `Travility.Core` phải được Thành chốt trước khi các module phụ thuộc triển khai.
4. Task của Quân phải có phạm vi nhỏ, dữ liệu mẫu và tiêu chí nghiệm thu rõ ràng.
5. Chatbot và Achievement nâng cao là phần tùy chọn, chỉ làm sau khi luồng lõi chạy ổn.
6. Tuần 6 không mở tính năng mới; chỉ sửa lỗi, hoàn thiện dữ liệu và luyện demo.

## 2. Trách nhiệm theo thành viên

| Thành viên | Sở hữu chính | Không sở hữu / phối hợp |
|---|---|---|
| **Thành** | Database, interface, Auth, các nghiệp vụ `Trip`/`Booking`/`Budget`/`Expense`, Dashboard, `Recommendation Engine`, `Itinerary` orchestration, `CheckIn` logic, tích hợp và Release | Review `Routing Engine`; hỗ trợ Tùng và Nhật khi contract bị vướng |
| **Tùng** | Nghiệp vụ Map: `Place`, phân loại địa điểm, Map Layers, filter, dữ liệu và tiêu chí nghiệm thu | Không triển khai Leaflet/WebView2; phối hợp với Nhật về dữ liệu đầu vào và hành vi hiển thị |
| **Nhật** | Map UI: Leaflet, WebView2 bridge, marker, popup, `PlaceDetail`; Trip Wizard/Itinerary/Comparison UI | Không tự quyết định quy tắc filter hoặc tính toán nghiệp vụ |
| **Quân** | `Routing Engine` gồm Haversine, nearest-neighbor, 2-opt và unit test; Admin CRUD, Check-in/`TravelPassport` UI, QA | Làm theo contract của Thành; được Thành review các phần engine và tích hợp |

### Ranh giới Map

- Tùng quyết định **bản đồ cần hiển thị gì và theo quy tắc nào**.
- Nhật quyết định **hiển thị bản đồ ra sao** và triển khai cầu Leaflet/WebView2.
- Thành chốt dữ liệu, interface và điểm ghép giữa hai phần.

## 3. Lộ trình theo tuần

| Tuần | Thành | Tùng | Nhật | Quân |
|---|---|---|---|---|
| **1** | Entity, interface, schema, EF model, Auth | Quy tắc Map, nhóm `Place`, filter và cấu trúc dữ liệu | WebView2 + Leaflet, marker tĩnh, khung Trip Wizard | Haversine và unit test đầu tiên |
| **2** | Repository và khung `Trip`/`Booking`/`Budget`/`Expense` | Dataset 120–150 `Place`, Map Layers và filter behavior | Marker, popup, bật/tắt layer, `PlaceDetail` | Nearest-neighbor, 2-opt và test trường hợp biên |
| **3** | `Recommendation Engine`, `Itinerary` orchestration và validation | Kiểm tra dữ liệu Map thật, hoàn thiện hành vi chọn `Place` | Vẽ `RoutePlan`/`Itinerary` lên Map | Tích hợp `Routing Engine` vào pipeline và bổ sung test |
| **4** | Ghép luồng end-to-end và `CheckIn` logic | Nghiệm thu nghiệp vụ Map | `Itinerary` UI và so sánh 3 phương án | Admin CRUD, Check-in/`TravelPassport` UI và smoke test |
| **5** | Sửa lỗi lõi; chỉ hỗ trợ Chatbot nếu đủ điều kiện | Chốt dữ liệu demo và các trường hợp Map | Hoàn thiện giao diện, hỗ trợ xuất báo cáo | Test case, hồi quy, ghi nhận và xác minh lỗi |
| **6** | Release build, tích hợp cuối, backup demo | Đóng băng dữ liệu và nội dung trình diễn | Hoàn thiện hình ảnh, luồng thao tác và slide UI | Smoke test trên máy trắng, hỗ trợ diễn tập |

## 4. Tiêu chí hoàn thành theo mốc

### Hết tuần 1

- Cả nhóm clone, build, chạy và đăng nhập được.
- `schema.sql` chạy sạch trên máy trắng.
- Interface trong `Travility.Core` đã chốt và merge.
- Map hiển thị marker từ dữ liệu tĩnh.
- `Routing Engine` chạy trên `List<Place>` giả với ít nhất 3 unit test xanh.

### Hết tuần 3

- Có 120–150 `Place` với đủ trường dữ liệu và `PreferenceTags` hợp lệ.
- Map bật/tắt layer, lọc dữ liệu và mở `PlaceDetail` được.
- Tạo `Trip`, lưu CSDL và validation đầu vào hoạt động.
- `Budget`, `PlannedCost`, `ActualSpent` được tính tách biệt và đúng `PricingUnit`.
- Pipeline ba tầng sinh được `Itinerary` và có `RoutePlan`.

### Hết tuần 4

- Chạy liền mạch: đăng nhập → tạo Trip → đề xuất → tối ưu → so sánh 3 `Itinerary` → Check-in → `TravelPassport`.
- `Itinerary` hiển thị được trên Map, mỗi ngày một màu.
- Check-in hoạt động ở cả chế độ thật và `SimulatedLocation`, có nhãn `SIMULATED`.

### Hết tuần 5 và trước ngày bảo vệ

- Chatbot chỉ được giữ lại nếu gọi tool đúng và không làm hỏng luồng lõi.
- Có ít nhất 15 unit test xanh, Release build chạy được.
- Đã thử trên máy trắng, quay video dự phòng và diễn tập ít nhất 3 lần.

## 5. Quy tắc giảm rủi ro

- Nếu Quân bị chậm, Thành tiếp quản phần tích hợp `Routing Engine`; Admin nâng cao có thể giản lược.
- Nếu hết tuần 4 chưa chạy end-to-end, bỏ Chatbot và dồn lực cho lõi.
- Nếu hết tuần 5 lõi còn lỗi nặng, giữ Check-in + `TravelPassport` cơ bản và bỏ Achievement nâng cao.
