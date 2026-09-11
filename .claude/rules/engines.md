# Quy tắc cho ba Engine

`Travility.Core` **không được** tham chiếu `System.Windows.Forms`. Đây là điều
cho phép engine chạy và test độc lập, không cần UI, không cần CSDL.

## Bắt buộc

- Engine nhận `List<Place>` và trả kết quả — **không** tự truy vấn CSDL.
- Khoảng cách trong engine: **Haversine × 1.3**. Không gọi API trong vòng lặp
  tối ưu — 2-opt chạy hàng vạn lần mỗi lần bấm nút.
- Đường thật (OSRM) chỉ dùng ở bước **vẽ tuyến**, gọi một lần.
- Không gộp mọi thứ vào một hàm `OptimizeTrip()`. Tách
  `RecommendationEngine` / `RoutingEngine` / `ItineraryEngine` / `BudgetEngine`.
- Itinerary Engine giữ đủ ba tầng: **Chọn → Chia ngày → Xếp giờ**.
  Bỏ tầng chia ngày sẽ sinh lịch trình vô lý.
- Mỗi tầng phải test được độc lập.

## Thuật ngữ

Ba phương án là **ba preset trọng số** theo `TravelStyle`, không phải Pareto
Front. Thuật toán là **Nearest Neighbor + 2-opt**, không phải TSPTW.

Chi tiết: `docs/07-engines.md`, `docs/adr/0004-*.md`, `docs/adr/0005-*.md`.
