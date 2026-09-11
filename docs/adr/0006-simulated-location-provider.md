# ADR-0006 — Check-in có hai nguồn vị trí, gồm chế độ giả lập

- **Trạng thái:** Chấp nhận
- **Ngày:** 2026-09-11

## Bối cảnh

Đặc tả yêu cầu check-in khi `Distance(UserLocation, PlaceLocation) ≤ 150 m`.
Có hai sự thật kỹ thuật khiến yêu cầu này không thể thoả trên nền tảng đã chọn:

1. **Laptop hầu như không có chip GPS.** Đại đa số laptop Windows không gắn bộ
   thu GPS; đó là linh kiện của điện thoại và một số máy có module WWAN.
   `Windows.Devices.Geolocation` trên laptop định vị bằng Wi-Fi + IP, sai số
   thường từ ~100 m tới vài km.

2. **Vị trí vật lý lúc demo sai.** Buổi bảo vệ diễn ra trong phòng học, cách mọi
   địa điểm trong dataset hàng km. Kể cả GPS chính xác tuyệt đối, điều kiện
   `≤ 150 m` vẫn **luôn thất bại**.

Điểm thứ hai mới là vấn đề thật, và nó độc lập với chất lượng phần cứng.

## Quyết định

Trừu tượng hoá qua `ILocationProvider` với hai implementation:

| Implementation | Dùng khi |
|---|---|
| `WindowsLocationProvider` | Chế độ thật — hiện toạ độ kèm độ chính xác ±Xm |
| `SimulatedLocationProvider` | Chế độ demo — click lên bản đồ để đặt vị trí |

Giao diện hiển thị nhãn **`SIMULATED`** rõ ràng khi ở chế độ giả lập. Bản ghi
`CheckIn` có cờ `IsSimulated`.

## Hệ quả

- Vẫn **có tích hợp API định vị thật để nói khi thuyết trình**, mà lúc demo vẫn
  bấm check-in thành công.
- Trung thực về giới hạn nền tảng — mạnh hơn nhiều so với bấm thử rồi báo lỗi.
- Là chỗ dùng **Bài 3 (Lớp và giao diện)** đúng mục đích.
- Cờ `IsSimulated` cho phép phân biệt dữ liệu demo khi thống kê.
- **Chi phí:** khoảng nửa ngày công.

## Phương án đã loại

**Chỉ định vị thật** — đúng tinh thần nhất nhưng không demo được, và sẽ phát
hiện điều đó vào đúng hôm bảo vệ.

**Chỉ vị trí giả lập** — rẻ nhất và chắc chắn demo được, nhưng mất điểm nhấn
tích hợp API hệ điều hành.

**Tăng `CheckInRadius` lên 50 km khi demo** — có vẻ "thật" hơn nhưng làm ràng
buộc trở nên vô nghĩa và rất dễ bị hỏi vặn.
