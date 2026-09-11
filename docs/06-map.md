# 06 — Smart Map: WebView2 + Leaflet

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) (từ điển thuật ngữ) · [`00-project-overview.md`](./00-project-overview.md)
> **Người phụ trách:** B
> **Đây là module kỹ thuật lạ nhất — dễ sa lầy nhất. Bắt đầu sớm, đừng để tuần 3.**

---

## Mục tiêu

Bản đồ tương tác làm trung tâm cho việc khám phá: hiển thị địa điểm theo lớp,
click marker xem chi tiết, thêm vào chuyến đi, và vẽ lịch trình đã tối ưu lên bản đồ.

---

## Quyết định đã chốt

| | |
|---|---|
| Vật chứa | **WebView2** trong WinForms |
| Thư viện bản đồ | **Leaflet.js** |
| Nền bản đồ | **Tile OpenStreetMap** |
| Gom marker | `Leaflet.markercluster` |
| Vẽ tuyến | `Leaflet Routing Machine` / OSRM — **chỉ gọi một lần** sau khi chốt thứ tự |

**Vì sao Leaflet + OSM:** không API key, không thẻ tín dụng, không rủi ro phát
sinh phí. Dataset do nhóm tự xây nên không cần dữ liệu địa điểm của Google —
chỉ cần nền bản đồ để vẽ marker.

---

## Sở hữu

```
Travility.WinForms/Map/
├── ExploreMapForm.cs
├── PlaceDetailForm.cs
└── MapAssets/              ← HTML + JS + CSS của bản đồ
    ├── map.html
    ├── map.js
    └── lib/                ← leaflet.js, markercluster (nhúng sẵn, không CDN)
```

---

## Phụ thuộc

- **Tuần 1: không phụ thuộc ai.** Vẽ marker từ **file JSON tĩnh**, chưa cần DB.
- Tuần 2 trở đi: cần `IPlaceService` của A để lấy địa điểm thật.
- Tuần 3: cần `IItineraryEngine` của D để vẽ lịch trình lên bản đồ.

---

## Cầu nối C# ↔ JavaScript

Đây là điểm nhấn kỹ thuật của module, và là **Bài 4 — Delegate và Event** được
dùng thật. Nhấn mạnh khi thuyết trình.

```
C# → JS:   webView.CoreWebView2.PostWebMessageAsJson(json)
           trong JS:  window.chrome.webview.addEventListener('message', ...)

JS → C#:   window.chrome.webview.postMessage(JSON.stringify({...}))
           trong C#:  webView.CoreWebView2.WebMessageReceived += OnMessage;
```

Hợp đồng thông điệp đề xuất (chốt sớm, đừng đổi giữa chừng):

```jsonc
// C# → JS
{ "action": "addMarkers",   "places": [ { "id":1, "lat":..., "lng":..., "category":"hotel" } ] }
{ "action": "clearLayer",   "category": "hotel" }
{ "action": "drawRoute",    "points": [ [lat,lng], ... ] }
{ "action": "setUserPin",   "lat":..., "lng":... }

// JS → C#
{ "event": "markerClick",   "placeId": 1 }
{ "event": "mapClick",      "lat":..., "lng":... }   // dùng cho chế độ check-in giả lập
```

`mapClick` là thứ module `E-checkin` sẽ dùng — làm sẵn từ đầu, rẻ.

---

## Việc theo thứ tự

### Tuần 1 — Dựng khung, chưa cần DB

1. Nhúng WebView2 vào một Form, load `map.html` từ thư mục local.
2. Hiện bản đồ Leaflet với tile OSM, center ở Đà Nẵng.
3. Đọc một file `places.json` tĩnh (~30 điểm), vẽ marker.
4. Click marker → gửi `placeId` về C# → hiện `MessageBox` là đủ cho tuần 1.

### Tuần 2 — Nối dữ liệu thật + Layers

1. Thay JSON tĩnh bằng `IPlaceService` của A.
2. **Map Layer System** (đặc tả §4): checkbox bật/tắt từng nhóm — Khách sạn,
   Nhà hàng, Lịch sử, Giải trí, Tham quan, Giao thông, Lịch trình của tôi,
   Đã check-in. Lọc bằng **LINQ** (ăn Bài 7).
3. Icon marker riêng theo loại (đặc tả §3.2).
4. `PlaceDetailForm` — tên, rating, số review, khoảng cách, giá vé, nút
   *Thêm vào chuyến đi* / *Chỉ đường*.

### Tuần 3 — Cluster + vẽ lịch trình

1. `Leaflet.markercluster` — gom marker khi zoom xa (đặc tả §4 muốn tránh rối).
2. Nhận lịch trình từ D, vẽ tuyến lên bản đồ, đánh số thứ tự điểm dừng.
3. Màu khác nhau cho từng ngày — đây là thứ **nhìn thấy được** chứng minh tầng
   chia cụm địa lý của engine hoạt động.

### Tuần 6 — Bảo hiểm demo

**Tải trước tile khu vực Đà Nẵng về máy.** Đây là bảo hiểm demo rẻ nhất của cả
đồ án: mất mạng lúc bảo vệ vẫn hiện được bản đồ.

---

## Định nghĩa "xong"

- [ ] Bản đồ hiện đúng ~150 địa điểm, đúng icon theo loại
- [ ] Bật/tắt từng lớp hoạt động mượt
- [ ] Click marker mở được `PlaceDetailForm` với dữ liệu thật
- [ ] Nút *Thêm vào chuyến đi* gọi được sang module của C
- [ ] Lịch trình 3 ngày hiện lên bản đồ với 3 màu khác nhau
- [ ] **Ngắt mạng, bản đồ vẫn hiện** (tile đã cache)

---

## Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Load Leaflet từ CDN → mất mạng là bản đồ trắng | **Nhúng file `.js`/`.css` vào thư mục local**, đặt `Copy to Output Directory` |
| WebView2 khởi tạo bất đồng bộ, gọi JS quá sớm → im lặng không lỗi | `await webView.EnsureCoreWebView2Async()` trước mọi thao tác |
| Đổi hợp đồng JSON giữa chừng | Chốt ở tuần 1, coi như interface |
| Vẽ lại toàn bộ marker mỗi lần bật/tắt lớp → giật | Tạo sẵn `L.layerGroup()` cho mỗi lớp, chỉ add/remove group |
| Quên `Copy to Output Directory` cho `map.html` → chạy Release là trắng | Kiểm tra thuộc tính file ngay từ đầu |
| Gọi OSRM mỗi lần tính khoảng cách | **Không.** Engine dùng Haversine. OSRM chỉ để vẽ, gọi một lần |

> **Quyết định nền:** [ADR-0003 — Leaflet + OpenStreetMap](./adr/0003-leaflet-openstreetmap.md)
