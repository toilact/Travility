# ADR-0003 — Bản đồ dùng Leaflet + tile OpenStreetMap trong WebView2

- **Trạng thái:** Chấp nhận
- **Ngày:** 2026-09-11

## Bối cảnh

Smart Map là trung tâm của trải nghiệm khám phá. WinForms không có control bản đồ
tương tác đủ tốt, nên cần nhúng bản đồ web qua WebView2.

Dataset địa điểm do nhóm tự xây, nên hệ thống **không cần** dữ liệu địa điểm của
nhà cung cấp bên ngoài — chỉ cần nền bản đồ để vẽ marker lên.

Buổi bảo vệ diễn ra trên laptop cá nhân, mạng phòng học không chắc chắn.

## Quyết định

**Leaflet.js + tile OpenStreetMap**, nhúng trong WebView2. Thư viện `.js`/`.css`
để trong thư mục local, không load từ CDN. Tile khu vực Đà Nẵng tải sẵn về máy.

## Hệ quả

- Không cần API key, không cần thẻ tín dụng, không có rủi ro phát sinh phí.
- Mất mạng lúc demo vẫn hiện được bản đồ — đây là bảo hiểm rẻ nhất của cả đồ án.
- Có sẵn `Leaflet.markercluster` cho yêu cầu gom marker.
- Buộc dataset của nhóm thành nguồn sự thật duy nhất, tránh cảnh bản đồ hiện một
  kiểu còn CSDL nói một kiểu.
- Cầu nối C# ↔ JavaScript qua `PostWebMessageAsJson` / `WebMessageReceived` trở
  thành điểm nhấn cho **Bài 4 (Delegate và Event)**.
- **Chi phí:** phải tự vẽ icon marker, không có ảnh/rating sẵn như Google.

## Phương án đã loại

**Google Maps JavaScript API** — đẹp và quen thuộc, nhưng bắt buộc gắn thẻ tín
dụng; quên tắt billing sau đồ án có thể phát sinh phí thật.

**Mapbox GL JS** — giao diện đẹp nhất, free tier rộng, nhưng vẫn cần đăng ký
token và nặng hơn Leaflet.

**GMap.NET** — control WinForms thuần, không cần WebView2 lẫn JavaScript. Đơn
giản hơn nhưng thư viện cũ, ít hiệu ứng, và mất điểm nhấn kỹ thuật WebView2 bridge.
