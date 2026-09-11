# ADR-0004 — Haversine trong engine, đường thật chỉ khi hiển thị

- **Trạng thái:** Chấp nhận
- **Ngày:** 2026-09-11

## Bối cảnh

Routing Engine cần khoảng cách giữa các địa điểm. Có một ràng buộc về **tần suất
gọi** dễ bị bỏ qua: thuật toán 2-opt đánh giá hàng nghìn đến hàng chục nghìn tổ
hợp cho **mỗi lần** người dùng bấm "Tối ưu".

Nếu mỗi lần tính khoảng cách là một HTTP request tới API định tuyến, người dùng
bấm nút rồi ngồi chờ vài phút, và hạn mức API hết ngay trong buổi test đầu tiên.

## Quyết định

Tách đôi theo mục đích:

| Mục đích | Dùng gì |
|---|---|
| Bên trong engine (chạy hàng vạn lần) | **Haversine × 1.3** |
| Vẽ tuyến lên bản đồ (chạy một lần) | OSRM / Leaflet Routing Machine |

Hệ số 1.3 là *detour factor*, bù cho việc đường thật không đi thẳng.

Distance Matrix tính sẵn, lưu vào bảng, truy vấn bằng **stored procedure qua
ADO.NET**.

## Hệ quả

- Tối ưu hoàn tất trong dưới ba giây với 150 địa điểm, hoàn toàn offline và tất định.
- Không phụ thuộc mạng ở phần tính toán — chỉ phần hiển thị mới cần.
- Distance Matrix trở thành chỗ dùng ADO.NET có lý do thật, thay vì gượng ép để
  đủ **Bài 8**.
- **Chi phí:** khoảng cách trong engine là xấp xỉ, không phải đường thật. Phải
  nói rõ điều này trong báo cáo.

## Phương án đã loại

**Tính sẵn toàn bộ ma trận bằng OSRM rồi lưu DB** — cho khoảng cách đường thật
100% mà engine vẫn nhanh, nhưng 150 địa điểm là ~22.500 cặp, tốn công seed và
phụ thuộc vào server công cộng có giới hạn.

**Gọi API trực tiếp mỗi lần tính** — đơn giản nhất về code nhưng chậm và vượt
hạn mức ngay lập tức.

**Chỉ Haversine, vẽ tuyến bằng đoạn thẳng** — đơn giản tuyệt đối, nhưng bản đồ
lúc demo nhìn thô, mất điểm ở phần quan trọng nhất.
