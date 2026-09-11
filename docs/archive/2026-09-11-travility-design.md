# TRAVILITY — Bản thiết kế chốt (Design v1)

> **Ngày:** 11/09/2026
> **Hạn thuyết trình:** 23/10/2026 (6 tuần)
> **Nhóm:** 4 người, cả 4 đều code được
> **Môn:** Lập trình trên môi trường Windows
> **Tài liệu gốc:** `docs/Travility_System_Specification.md`

Tài liệu này **không thay thế** bản đặc tả gốc. Nó chốt những quyết định mà
bản gốc còn để mở, và ghi lại lý do. Khi hai tài liệu mâu thuẫn, tài liệu này
thắng.

---

## 1. Bối cảnh

Bản đặc tả gốc mô tả đầy đủ *cái gì* (24 form, 3 engine, chatbot, bản đồ,
gamification) nhưng để mở phần lớn *làm thế nào*: lấy dữ liệu ở đâu, thuật toán
nào, thư viện bản đồ gì, 4 người chia việc ra sao.

Nhóm đã quyết định **giữ nguyên phạm vi lớn**. Vì vậy mục tiêu của bản thiết kế
này không phải cắt giảm, mà là **loại bỏ mọi điểm chờ đợi và mọi rủi ro demo**,
để phạm vi đó thực sự về đích trong 6 tuần.

Ràng buộc gốc: 4 người × 6 tuần ≈ **380–480 giờ-người**. Không có chỗ cho thời
gian chết.

---

## 2. Bảng quyết định

| # | Vấn đề | Quyết định | Lý do cốt lõi |
|---|---|---|---|
| 1 | Chiến lược CSDL | **Database First** | Một `schema.sql` trong git, một người sở hữu. Hợp với ADO.NET + stored procedure (Bài 8) và với ERD phải nộp. Tránh xung đột migration khi 4 người song song. |
| 2 | Chia việc | **Module dọc (vertical slice)** | `.Designer.cs` merge rất tệ → mỗi Form đúng một người chạm. Chia theo tầng sẽ khiến người làm UI phải chờ. |
| 3 | Giờ mở cửa | **1 khung giờ/ngày, giống nhau cả tuần** | Giữ được ràng buộc `OpeningHourConstraint` mà dataset không phình gấp 7. |
| 4 | Bản đồ | **Leaflet + tile OpenStreetMap** | Không API key, không thẻ tín dụng. Tải trước tile Đà Nẵng → demo được khi mất mạng. |
| 5 | Khoảng cách | **Haversine × 1.3 trong engine · OSRM chỉ khi vẽ tuyến** | Engine gọi hàng vạn lần nên phải offline và ~0ms; đường thật chỉ cần ở bước hiển thị. |
| 6 | Itinerary Engine | **Pipeline tham lam 3 tầng** | Mỗi tầng test độc lập được; chắc chắn xong trong 6 tuần; giải thích được khi bảo vệ. |
| 7 | Check-in | **`ILocationProvider` 2 nhánh: thật + giả lập** | Demo diễn ra trong phòng học, cách mọi POI hàng km → định vị thật luôn thất bại. |
| 8 | Chatbot | **Gemini / OpenAI API + tool use + replay** | Nhóm đã có key. Bảng `ToolExecutions` cho phép phát lại phiên đã ghi khi mất mạng. |
| 9 | Ngân sách | **3 con số + `PricingUnit`** | Budget / PlannedCost / ActualSpent là ba đại lượng khác nhau. Khách sạn tính theo phòng, không theo người. |
| 10 | Solution | **4 project: Data / Core / WinForms / Tests** | `Core` không phụ thuộc UI → D chạy engine độc lập từ ngày 1. `Tests` là điểm cộng thật khi bảo vệ. |
| 11 | Merge | **Xong chức năng thì merge, không ràng buộc lịch** | Quyết định của nhóm. Rủi ro trôi giao diện được vá ở tầng thiết kế (mục 4). |

---

## 3. Kiến trúc solution

```
Travility.sln
├── Travility.Data      → EF model (DB First), Repository            [A]
├── Travility.Core      → Entities, Services, 3 Engine, Events       [C, D]
│     └── Engines/        ← không tham chiếu WinForms, không cần DB
├── Travility.WinForms  → Forms, chia thư mục theo module            [A B C D]
│     ├── Auth/    [A]     ├── Trip/    [C]     ├── Map/       [B]
│     └── Admin/   [A]     ├── Budget/  [C]     └── Itinerary/ [D]
└── Travility.Tests     → Unit test cho 3 Engine                     [D]
```

Quy tắc phụ thuộc một chiều: `WinForms → Core → Data`. `Core` **không được**
tham chiếu `System.Windows.Forms`.

### Phân công

| Người | Sở hữu | Rủi ro chính |
|---|---|---|
| **A** (lead) | `schema.sql`, seed, DbContext, Repository, Auth, MainForm, cụm Admin | Nút thắt tuần 1 (~3 ngày) |
| **B** | Smart Map, Map Layers, PlaceDetail, cầu WebView2 ↔ C# | Kỹ thuật lạ nhất, dễ sa lầy |
| **C** | Trip Wizard, Transport, Booking, Budget, Expense | Nhiều form nhất, nghiệp vụ tiền |
| **D** | 3 Engine, Itinerary, Comparison, Top-K, unit test | Thuần logic, test rời được |

Check-in / Gamification / Passport: ghép tuần 4 cho ai xong trước.

---

## 4. Chống trôi giao diện (thay cho kỷ luật merge)

Vì nhóm chọn merge tự do, biện pháp phòng vệ nằm ở thiết kế chứ không ở quy trình.

**Ngày 2–3 tuần 1: chốt toàn bộ interface trong `Travility.Core`, merge một lần
duy nhất, sau đó không ai sửa chữ ký nếu chưa báo cả nhóm.**

```csharp
ITripService, IBudgetService, IPlaceService,
IRecommendationEngine, IRoutingEngine, IItineraryEngine,
ILocationProvider, IChatProvider
```

Chỉ chữ ký hàm + DTO, thân hàm `throw new NotImplementedException()`. Sau bước
này, bốn người code cùng một bản hợp đồng nên sự trôi lệch không còn chỗ xảy ra,
và ai merge lúc nào cũng được.

**Hai việc bắt buộc ngày đầu:**
1. `.gitignore` chuẩn Visual Studio — bỏ `bin/`, `obj/`, `packages/`, `.vs/`, `*.suo`.
2. Không ai mở Form của người khác trong Designer, kể cả chỉ để xem.

Khi conflict trên `.csproj`: **giữ cả hai dòng** `<Compile Include=...>`, không
bao giờ xoá dòng của người khác.

---

## 5. Dữ liệu

### 5.1. Quy mô

**120–150 POI, một thành phố (Đà Nẵng)**, nhập tay bằng Excel → script import.
Đủ để Top-K có ý nghĩa và routing có không gian nghiệm. Báo cáo ghi rõ: *dataset
tự xây dựng phục vụ mục đích học thuật*.

### 5.2. Hợp đồng trường dữ liệu

Suy ngược từ thứ engine **bắt buộc** phải có:

| Trường | Phục vụ | Ghi chú |
|---|---|---|
| `Latitude`, `Longitude` | Map, Routing, Check-in | Bản gốc §35 thiếu — **phải bổ sung** |
| `Rating`, `ReviewCount` | Recommendation | Đầu vào Bayesian |
| `Price` / `TicketPrice` | Recommendation, Budget | |
| `CategoryId` | Map Layers, lọc | |
| `OpenTime`, `CloseTime` | Itinerary | Gộp vào `Places`, **bỏ bảng `PlaceOpeningHours`** |
| **`VisitDuration`** (phút) | Itinerary, Routing | Gán tay theo category: bảo tàng 90, biển 180, quán ăn 60, cà phê 45 |
| **`PreferenceTags`** (1–3 tag) | Recommendation | Nối với 7 nhóm sở thích ở §5.2. **Bản gốc thiếu** — không có nó thì `PreferenceScore` không tính được |
| `IsMealPlace` | Itinerary | Để chèn giờ ăn |
| `PricingUnit` | Budget | PerPerson / PerRoom / PerTrip |

### 5.3. Thay đổi so với đặc tả gốc

- **Bỏ** bảng `PlaceOpeningHours` (gộp vào `Places`).
- **Thêm** `Latitude`, `Longitude`, `VisitDuration`, `PreferenceTags`,
  `IsMealPlace`, `PricingUnit` vào `Places`.
- Làm rõ: `PlaceRatings` là điểm tổng hợp (cache), `PlaceReviews` là từng review.
- Thống nhất tên nhóm ngân sách dự phòng: dùng **`Reserve`** (§9 đang lẫn lộn
  `Reserve` và `Emergency`).

---

## 6. Ba Engine

### 6.1. Recommendation Engine

Giữ nguyên công thức §16. Điểm nhấn học thuật là **Bayesian Rating** (§17) —
giữ bằng mọi giá, rẻ về công sức và ấn tượng khi bảo vệ:

```
WeightedRating = (v/(v+m)) × R + (m/(v+m)) × C
```

Top-K lấy bằng LINQ (`OrderByDescending(...).Take(k)`) — đúng **Bài 7**.

### 6.2. Routing Engine

- **Trong engine:** Haversine × 1.3 (detour factor). Offline, ~0ms, tất định.
- **Khi vẽ tuyến:** OSRM / Leaflet Routing Machine, gọi **một lần** sau khi đã
  chốt thứ tự.
- **Distance Matrix** tính sẵn, lưu bảng, truy vấn bằng **stored procedure qua
  ADO.NET** → đúng chỗ thể hiện **Bài 8** một cách có lý do.
- Thuật toán: **Nearest Neighbor + 2-opt**. Với n ≤ 8 có so sánh đối chứng
  brute-force để đánh giá chất lượng nghiệm.

> **Lưu ý khi viết báo cáo:** không tuyên bố triển khai "TSP with Time Windows"
> hay "Orienteering Problem". Viết đúng những gì code làm.

### 6.3. Itinerary Engine — pipeline tham lam 3 tầng

```
Với mỗi TravelStyle (Budget / Balanced / Experience):

  Tầng 1 — CHỌN
    Xếp hạng toàn bộ POI theo RecommendationScore
    Tham lam nhặt theo tỉ lệ (Score / Chi phí)
    đến khi hết ngân sách hoặc hết quỹ thời gian

  Tầng 2 — CHIA NGÀY
    Gom các điểm đã chọn thành N cụm địa lý (N = số ngày)
    → mỗi ngày một vùng, không chạy lòng vòng

  Tầng 3 — XẾP GIỜ
    Mỗi ngày: Nearest Neighbor + 2-opt
    Chèn giờ ăn, kiểm tra OpenTime/CloseTime
    Tích luỹ VisitDuration + thời gian di chuyển

→ 3 lịch trình để so sánh (§23)
```

**Tầng 2 là tầng quyết định lịch trình trông có thông minh hay không.** Bỏ nó đi
sẽ sinh ra lịch trình vô lý kiểu *sáng lên Bà Nà, trưa xuống biển, chiều lại lên
núi*. Đây cũng là thứ **nhìn thấy được trên bản đồ lúc demo**.

Mỗi tầng test độc lập. Nếu tầng 3 chậm, vẫn demo được tầng 1+2.

> **Thuật ngữ:** ba phương án A/B/C là **3 preset trọng số**, không phải Pareto
> Front. Báo cáo gọi đúng tên để tránh bị hỏi vặn.

---

## 7. Bản đồ

```
WinForms → WebView2 → Leaflet.js + tile OpenStreetMap
```

- `Leaflet.markercluster` cho §4 (gom marker khi zoom xa).
- **Tải trước tile khu vực Đà Nẵng về máy** — bảo hiểm demo rẻ nhất.
- Cầu nối hai chiều:
  - C# → JS: `webView.CoreWebView2.PostWebMessageAsJson()`
  - JS → C#: sự kiện `WebMessageReceived`

Cầu nối này ăn thẳng vào **Bài 4 — Delegate và Event**; nhấn mạnh khi thuyết trình.

---

## 8. Check-in

```csharp
interface ILocationProvider { Coordinate GetCurrentLocation(); double Accuracy { get; } }
```

| Implementation | Dùng khi |
|---|---|
| `WindowsLocationProvider` | Chế độ thật — hiện toạ độ kèm độ chính xác ±Xm |
| `SimulatedLocationProvider` | Chế độ demo — click lên bản đồ để đặt vị trí |

Form hiển thị nhãn **SIMULATED** rõ ràng khi ở chế độ giả lập.

**Lý do bắt buộc phải có chế độ giả lập:** lúc bảo vệ nhóm ngồi trong phòng học,
cách mọi POI hàng km, nên điều kiện `≤ 150m` (§11.1) sẽ luôn thất bại bất kể
phần cứng. Ngoài ra đại đa số laptop Windows không có chip GPS — API định vị
thực chất dùng Wi-Fi/IP với sai số từ ~100m tới vài km.

Đây là **Bài 3 — Lớp và giao diện** được dùng đúng chỗ, chi phí ~nửa ngày công.

---

## 9. Ngân sách

### 9.1. Ba con số cho mỗi chuyến

| Con số | Nguồn | Ý nghĩa |
|---|---|---|
| `Budget` | Người dùng nhập | Hạn mức tự đặt |
| `PlannedCost` | Hệ thống tự tính từ itinerary | Dự kiến sẽ tiêu |
| `ActualSpent` | Người dùng ghi nhận khi đi | Đã tiêu thật |

Dashboard §10 vẽ **hai thanh chồng nhau**, không gộp làm một.

### 9.2. Travel Wallet là hạn mức, không phải két giữ tiền

- Tạo chuyến đi **không** trừ ví.
- Chỉ `Expense` thật mới làm giảm số dư ví.
- Tổng `Budget` các chuyến đang hoạt động vượt số dư ví → **cảnh báo, không chặn**.

### 9.3. PricingUnit

```
Vé tham quan, ăn uống  → PerPerson  → × số người
Khách sạn              → PerRoom    → × số phòng × số đêm
Vé máy bay/tàu/xe      → PerPerson  → × số người
Taxi, xăng xe          → PerTrip    → × số chuyến
```

Thiếu cờ này thì chuyến 2 người sẽ tính sai ngân sách khách sạn gấp đôi — lỗi lộ
ngay khi thầy bấm thử.

---

## 10. Chatbot

- Nhà cung cấp: **Gemini API / OpenAI API** (nhóm đã có key), bọc sau
  `IChatProvider` với hai implementation → hết hạn mức bên này đổi sang bên kia
  ngay giữa buổi demo. Đúng nguyên tắc §49.1 mà bản gốc đã tự đặt ra.
- Gọi thẳng `HttpClient` + `Newtonsoft.Json`, **không dùng SDK chính thức** (phần
  lớn nhắm .NET hiện đại, hay vướng trên .NET Framework).
- **Rút từ 16 tool xuống 7** — đủ chứng minh luận điểm §31:

```
find_places(category, maxPrice, near)    get_trip_budget()
add_place_to_itinerary(placeId, day)     remove_place_from_itinerary(itemId)
optimize_route(day)                      generate_itinerary(style)
record_expense(category, amount)
```

**Hai điều bắt buộc:**
1. **Chế độ phát lại.** Mọi lệnh gọi ghi vào `ToolExecutions` (§40.1); thêm nút
   "Phát lại phiên đã ghi" đọc lại hội thoại thành công từ DB và diễn lại trên
   UI. Mất mạng lúc bảo vệ vẫn demo được. Công dụng thứ hai, gần như miễn phí,
   của một bảng vốn đã phải làm.
2. **API key đọc từ file config ngoài git.** Không hardcode.

Lưu ý kỹ thuật:

```csharp
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
```

Một số phiên bản .NET Framework không bật TLS 1.2 mặc định → gọi HTTPS lỗi khó hiểu.

---

## 11. Bổ sung so với đặc tả gốc (ăn rubric)

Bản gốc thiếu, nhưng rubric môn WinForms thường có:

- **Hash mật khẩu** — lưu plaintext là lỗi bị trừ điểm chắc chắn.
- **Xuất báo cáo** — in lịch trình ra PDF/Excel.
- **Validation đầu vào** — `ErrorProvider`, `try-catch` lỗi kết nối DB.
- **Thống kê Admin** — biểu đồ `Chart`: địa điểm được thêm nhiều nhất, chi tiêu
  trung bình. Rẻ và ăn điểm.
- **Gỡ "LEARN USER PREFERENCE"** khỏi sơ đồ §47 (không có thiết kế tương ứng),
  hoặc thêm cơ chế đơn giản: sau mỗi check-in tăng nhẹ trọng số preference của
  category đó.

---

## 12. Lộ trình 6 tuần

| Tuần | Mốc | Nội dung |
|---|---|---|
| **1**<br>11–17/09 | Nền móng + **chốt interface** | A: `schema.sql` + seed 30 POI mẫu + EF model + Login/MainForm · B: WebView2+Leaflet vẽ marker từ JSON tĩnh · C: khung TripWizard · D: Haversine + NN/2-opt + unit test trên dữ liệu giả |
| **2**<br>18–24/09 | Dataset đủ 120–150 POI | A: Repository + Auth + hash mật khẩu · B: Map Layers + PlaceDetail + cầu 2 chiều · C: Trip CRUD + Transport + Booking · D: Bayesian + RecommendationScore + Top-K |
| **3**<br>25/09–01/10 | Nghiệp vụ lõi xong | C: Budget + Expense + PricingUnit + Dashboard · D: pipeline 3 tầng chạy được · B: cluster marker + vẽ itinerary lên map · A: Admin forms |
| **4**<br>02–08/10 | 🚩 **End-to-end chạy được** | Itinerary UI + so sánh 3 phương án · Check-in + `ILocationProvider` · Gamification + Passport |
| **5**<br>09–15/10 | Phần nâng cao | Chatbot + tool use + replay · Xuất báo cáo · Thống kê Admin |
| **6**<br>16–23/10 | 🔒 **Đóng băng tính năng 16/10** | Chỉ sửa lỗi · dữ liệu demo đẹp · tải tile offline · tài liệu + slide · tập demo ≥ 3 lần · quay video dự phòng |

### Hai điểm dừng khẩn cấp (quyết định trước)

- Hết tuần 4 **chưa chạy end-to-end** → **bỏ chatbot**, dồn tuần 5 vào lõi.
- Hết tuần 5 lõi còn lỗi nặng → **bỏ huy hiệu nâng cao**, giữ check-in + passport
  cơ bản.

**Tuần 6 không viết tính năng mới.** Đồ án điểm thấp hiếm khi vì thiếu chức năng
— thường vì demo vỡ, vì lỗi khi thầy bấm thử, vì không ai tập nói trước.

---

## 13. Ánh xạ tới 9 bài của học phần

Dùng bảng này khi viết báo cáo và làm slide.

| Bài | Thể hiện ở đâu |
|---|---|
| 1. .NET Framework | Kiến trúc 4 project, quy tắc phụ thuộc một chiều |
| 2. Ngôn ngữ C# | Toàn bộ |
| 3. Lớp và giao diện | `IPlaceProvider`, `ILocationProvider`, `IChatProvider`, 3 interface engine |
| 4. Delegate và Event | Event-driven giữa các engine (§32); cầu WebView2 ↔ C# |
| 5. Windows Forms | ~24 form, MDI/shell, wizard nhiều bước |
| 6. WinForms Control | `DataGridView`, `Chart`, `ErrorProvider`, custom control cho thẻ địa điểm |
| 7. LINQ | Top-K ranking, lọc Map Layers, tổng hợp Passport |
| 8. ADO.NET | Distance Matrix + báo cáo thống kê qua stored procedure |
| 9. Entity Framework | Toàn bộ CRUD nghiệp vụ (DB First) |

Ranh giới **Bài 8 vs Bài 9** phải ghi rõ trong báo cáo: *EF cho CRUD nghiệp vụ;
ADO.NET cho Distance Matrix, báo cáo thống kê và các stored procedure cần hiệu
năng.* Không phân vai rõ sẽ thành hai phong cách truy cập DB lẫn lộn.

---

## 14. Việc cần làm ngay (ngày 1–3)

1. **A:** tạo solution 4 project + `.gitignore` chuẩn VS + đẩy repo.
2. **Cả nhóm:** họp chốt toàn bộ interface trong `Travility.Core`, merge một lần.
3. **A:** viết `schema.sql` v1 + seed 30 POI mẫu để mọi người có dữ liệu chạy thử.
4. **Người làm dataset:** nhận hợp đồng trường ở mục 5.2 **trước khi** bắt đầu
   thu thập — tránh mất một tuần làm lại.
5. **B:** dựng WebView2 + Leaflet hiện marker từ file JSON tĩnh (chưa cần DB).
6. **D:** viết Haversine + NN/2-opt + unit test trên `List<Place>` giả (chưa cần DB).
