# 15 — Lộ trình và tài liệu phải nộp

> **Đọc trước:** [`00-project-overview.md`](./00-project-overview.md)
> **11/09/2026 → 23/10/2026 · đúng 6 tuần · 4 người**

---

## 1. Lộ trình

| Tuần | Mốc | Nội dung |
|---|---|---|
| **1**<br>11–17/09 | Nền móng + **chốt interface** | Thành: schema + EF model + Auth · Tùng: quy tắc Map và dữ liệu `Place` · Nhật: WebView2+Leaflet marker tĩnh + khung Trip Wizard · Quân: Haversine + unit test |
| **2**<br>18–24/09 | Dataset đủ 120–150 `Place` | Thành: Repository + nghiệp vụ lõi · Tùng: Map Layers + filter · Nhật: `PlaceDetail` + cầu WebView2 ↔ C# · Quân: nearest-neighbor + 2-opt |
| **3**<br>25/09–01/10 | Nghiệp vụ lõi xong | Thành: Budget + Expense + `PricingUnit` + Recommendation/Itinerary · Tùng: nghiệm thu Map · Nhật: vẽ `RoutePlan`/`Itinerary` · Quân: tích hợp Routing Engine + test |
| **4**<br>02–08/10 | 🚩 **End-to-end chạy được** | Thành: tích hợp Trip → Itinerary → `CheckIn` · Tùng: nghiệm thu Map · Nhật: Itinerary UI + so sánh 3 phương án · Quân: Admin + `CheckIn`/`TravelPassport` UI |
| **5**<br>09–15/10 | Phần nâng cao | Thành: Chatbot + tool use + replay nếu đủ điều kiện · Tùng: chốt dữ liệu demo · Nhật: xuất báo cáo và UI polish · Quân: thống kê Admin, QA và **diễn tập trên máy trắng** |
| **6**<br>16–23/10 | 🔒 **Đóng băng tính năng 16/10** | Thành: chỉ sửa lỗi, Release và tích hợp · Tùng: dữ liệu demo đẹp · Nhật: tài liệu, slide và UI · Quân: tile offline, smoke test, tập demo ≥ 3 lần và video dự phòng |

### Hai điểm dừng khẩn cấp

- Hết tuần 4 **chưa chạy end-to-end** → **bỏ chatbot**, dồn tuần 5 vào lõi.
- Hết tuần 5 lõi còn lỗi nặng → **bỏ huy hiệu nâng cao**, giữ CheckIn + Passport cơ bản.

---

## 2. Định nghĩa "xong" theo mốc

### Hết tuần 1

- [ ] 3 người còn lại clone repo, build được, chạy được, đăng nhập được
- [ ] `schema.sql` chạy sạch trên máy trắng
- [ ] Toàn bộ interface trong `Travility.Core` đã chốt và merge
- [ ] Bản đồ hiện marker từ JSON tĩnh
- [ ] Engine chạy trên `List<Place>` giả, có ít nhất 3 unit test xanh

### Hết tuần 3

- [ ] Dataset đủ 120–150 `Place` với đủ hợp đồng trường
- [ ] Bật/tắt lớp bản đồ hoạt động, click marker mở `PlaceDetailForm`
- [ ] Tạo `Trip` hoàn chỉnh, lưu CSDL, validation chặn đầu vào sai
- [ ] Dashboard hiện đúng ba con số tiền, chuyến 2 người tính đúng
- [ ] Pipeline ba tầng sinh được `Itinerary`

### Hết tuần 4 — mốc quan trọng nhất

- [ ] **Chạy được liền mạch: đăng nhập → tạo chuyến → đề xuất → tối ưu →
      so sánh 3 phương án → check-in → passport**
- [ ] `Itinerary` vẽ được lên bản đồ, mỗi ngày một màu
- [ ] Check-in được ở cả hai chế độ, có nhãn `SIMULATED`

### Hết tuần 5

- [ ] Chatbot gọi đúng tool, thao tác thật sự thay đổi CSDL
- [ ] Nút phát lại diễn được một phiên hoàn chỉnh khi **ngắt mạng**
- [ ] Xuất được lịch trình ra PDF/Excel
- [ ] Đã chạy thử trên một máy chưa từng cài gì

### Trước 23/10

- [ ] ≥ 15 unit test xanh
- [ ] Build Release chạy được
- [ ] Video dự phòng đã quay
- [ ] Tập demo ≥ 3 lần
- [ ] Đủ 7 tài liệu ở mục 3

---

## 3. Tài liệu phải nộp

Đặc tả gốc liệt kê **18 loại tài liệu** — quá nhiều cho 6 tuần, giấy tờ sẽ lấn át
code. Ưu tiên bảy cái:

| # | Tài liệu | Ai | Khi nào | Nguồn có sẵn |
|---|---|---|---|---|
| 1 | Use Case Diagram + đặc tả | cả nhóm | Tuần 1–2 | [`01-requirements.md`](./01-requirements.md) |
| 2 | ERD + Data Dictionary | Thành | Tuần 1–2 | [`03-database-design.md`](./03-database-design.md) |
| 3 | Business Rules | Thành (lõi), Tùng (Map) | Tuần 3 | [`08-trip-booking-budget.md`](./08-trip-booking-budget.md) mục 5 |
| 4 | Kiến trúc lớp + sơ đồ project | Thành | Tuần 3 | [`02-system-architecture.md`](./02-system-architecture.md) |
| 5 | Đặc tả 3 Engine | Thành (Recommendation/Itinerary), Quân (Routing) | Tuần 4 | [`07-engines.md`](./07-engines.md) |
| 6 | Thiết kế giao diện | Nhật, phối hợp Tùng | Tuần 4 | [`05-ui-guidelines.md`](./05-ui-guidelines.md) |
| 7 | Test Case | Quân điều phối, cả nhóm thực hiện | Tuần 5 | [`12-testing-strategy.md`](./12-testing-strategy.md) |

Phần lớn nội dung **đã có sẵn** trong bộ tài liệu này — việc còn lại chủ yếu là
vẽ sơ đồ và định dạng theo mẫu của trường.

### Gộp hoặc bỏ

- Activity Diagram → gộp vào đặc tả use case.
- Sequence Diagram → **chỉ vẽ 3 cái**: tạo chuyến đi, sinh lịch trình, chatbot
  gọi tool.
- Kế hoạch Sprint → dùng mục 1 của file này.

---

## 4. Slide — ba trang bắt buộc

Ngoài các trang giới thiệu thông thường, ba trang này trả lời trực tiếp thứ hội
đồng muốn biết:

**Trang "Ánh xạ 9 bài học"**

| Bài | Thể hiện ở đâu |
|---|---|
| 1. .NET Framework | Kiến trúc 4 project, phụ thuộc một chiều |
| 2. Ngôn ngữ C# | Toàn bộ |
| 3. Lớp và giao diện | `IPlaceProvider`, `ILocationProvider`, `IChatProvider`, 3 interface engine |
| 4. Delegate và Event | Event giữa các engine; cầu WebView2 ↔ C# |
| 5. Windows Forms | ~24 form, shell, wizard nhiều bước |
| 6. WinForms Control | `DataGridView`, `Chart`, `ErrorProvider`, custom control |
| 7. LINQ | Top-K, lọc Map Layers, tổng hợp Passport |
| 8. ADO.NET | `DistanceMatrix` + thống kê qua stored procedure |
| 9. Entity Framework | Toàn bộ CRUD nghiệp vụ (DB First) |

**Trang "Ba engine"** — sơ đồ pipeline ba tầng, vì đây là thứ phân biệt đồ án
này với một phần mềm CRUD.

**Trang "Quyết định kỹ thuật"** — 3–4 quyết định từ [`adr/`](./adr/) kèm lý do.
Cho thấy nhóm cân nhắc chứ không chọn bừa.

---

## 5. Theo dõi công việc

GitHub Issues trên `toilact/Travility` — xem
[`agents/issue-tracker.md`](./agents/issue-tracker.md).

Lịch sử Issues + Pull Request **chính là nhật ký tiến độ**. Không cần file
`TASKS.md` hay `PROGRESS.md` riêng để bốn người conflict trên đó.
