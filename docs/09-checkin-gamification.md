# 09 — Check-in, Gamification, Travel Passport

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) (từ điển thuật ngữ) · [`00-project-overview.md`](./00-project-overview.md)
> **Người phụ trách:** chưa gán cứng — ghép tuần 4 cho ai xong trước
> **Đây là module "ăn điểm nhìn" — rẻ về công sức, ấn tượng khi demo.**

---

## Mục tiêu

Người dùng check-in tại địa điểm khi ở đủ gần, tích luỹ huy hiệu và xây dựng
hồ sơ du lịch cá nhân.

---

## 1. Vấn đề vị trí — đọc kỹ phần này trước khi code

Có hai sự thật kỹ thuật phải đối mặt:

**Thứ nhất — laptop hầu như không có chip GPS.** Đại đa số laptop Windows không
gắn bộ thu GPS; đó là linh kiện của điện thoại và một số máy có module WWAN/4G.
`Windows.Devices.Geolocation` trên laptop thực chất định vị bằng **Wi-Fi + IP**,
sai số thường từ ~100m tới vài km.

**Thứ hai, và đây mới là vấn đề thật:** lúc bảo vệ nhóm **ngồi trong phòng học,
không đứng ở Bà Nà Hills**. Khoảng cách tới mọi POI trong dataset sẽ là hàng km,
nên điều kiện `<= 150m` (đặc tả §11.1) sẽ **luôn thất bại** bất kể phần cứng tốt
đến đâu.

Kết luận: **bắt buộc phải có đường vào vị trí giả lập.** Không phải vì thiếu tự
tin vào phần cứng, mà vì vị trí vật lý lúc demo sai.

---

## 2. Giải pháp đã chốt — `ILocationProvider` hai nhánh

```csharp
public interface ILocationProvider
{
    Coordinate GetCurrentLocation();
    double AccuracyMeters { get; }
    string ProviderName { get; }
}
```

| Implementation | Dùng khi | Ghi chú |
|---|---|---|
| `WindowsLocationProvider` | Chế độ thật | Hiện toạ độ kèm độ chính xác ±Xm |
| `SimulatedLocationProvider` | Chế độ demo | Click lên bản đồ để đặt vị trí |

`CheckInForm` có công tắc chuyển, và **hiển thị nhãn `SIMULATED` rõ ràng** khi ở
chế độ giả lập.

Cách này được cả hai đầu: vẫn **có tích hợp API định vị thật để nói khi thuyết
trình**, mà lúc demo vẫn bấm check-in thành công.

Câu trả lời khi thầy hỏi *"sao lại có chế độ giả lập?"*:
> *"Vì ứng dụng desktop không định vị chính xác được và chúng em đang ở trong
> phòng thi, nên tách rõ hai chế độ."*

Trung thực về giới hạn nền tảng được điểm cao hơn là bấm thử rồi báo lỗi.

Đây là **Bài 3 — Lớp và giao diện** được dùng đúng chỗ. Chi phí ~nửa ngày công.

---

## 3. Điều kiện check-in

```
Distance(UserLocation, PlaceLocation) <= CheckInRadius     // 150 m
```

Dùng chung hàm Haversine với `RoutingEngine` — **không viết lại**.

### Model

```
CheckIn
- CheckInId · UserId · PlaceId · TripId
- CheckInTime · Latitude · Longitude · DistanceFromPlace
- IsSimulated        <- thêm mới, để phân biệt dữ liệu demo
```

`IsSimulated` là trường nên có: trung thực về dữ liệu, và có ích khi thống kê.

---

## 4. Huy hiệu (đặc tả §12)

| Huy hiệu | Điều kiện |
|---|---|
| First Step | Check-in địa điểm đầu tiên |
| Beach Explorer | Check-in 5 bãi biển |
| History Hunter | Check-in 10 địa điểm lịch sử |
| Food Explorer | Check-in 20 nhà hàng / quán ăn |
| Travel Master | Check-in 50 địa điểm |

Điều kiện đọc từ bảng `Achievements`, **không hardcode trong C#** — để Admin
thêm huy hiệu mới mà không cần build lại.

Kiểm tra huy hiệu chạy khi nhận event `CheckInCompleted` (Bài 4).

---

## 5. Travel Passport (đặc tả §13)

Tổng hợp bằng **LINQ** (ăn Bài 7):

```
Số thành phố đã đến · Số địa điểm đã check-in · Tổng quãng đường
Tổng số chuyến đi   · Số huy hiệu            · Lịch sử du lịch
```

Đây là màn hình đẹp nhất để kết thúc demo. Đầu tư giao diện cho nó xứng đáng.

---

## 6. Tuỳ chọn nhỏ đáng làm

Đặc tả §47 có nhắc *"LEARN USER PREFERENCE"* nhưng không có thiết kế tương ứng.
Cách rẻ nhất để lời hứa đó thành thật:

> Sau mỗi `CheckInCompleted`, tăng nhẹ trọng số preference của category tương ứng
> trong `UserPreferences`.

Vài dòng code, và cho phép nói *"hệ thống học dần sở thích người dùng"* một cách
trung thực. Nếu không làm, hãy **gỡ dòng đó khỏi sơ đồ §47**.

---

## Định nghĩa "xong"

- [ ] Check-in được ở cả hai chế độ, có nhãn `SIMULATED` rõ ràng
- [ ] Chế độ thật hiện đúng toạ độ và độ chính xác thực tế
- [ ] Check-in ngoài bán kính bị từ chối với thông báo rõ ràng
- [ ] Đủ 5 huy hiệu, trao tự động khi đạt điều kiện
- [ ] Passport tổng hợp đúng số liệu
- [ ] Admin thêm được huy hiệu mới mà không cần build lại

---

## Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Chỉ làm định vị thật, tới hôm demo mới phát hiện không check-in được | Làm chế độ giả lập **trước** |
| Hardcode điều kiện huy hiệu trong C# | Đọc từ bảng `Achievements` |
| Viết lại hàm tính khoảng cách | Dùng chung với `RoutingEngine` |
| Trao huy hiệu trùng nhiều lần | Kiểm tra `UserAchievements` trước khi trao |
| Bỏ trường `IsSimulated` | Sau này không phân biệt được dữ liệu demo |
| Quên rằng `Windows.Devices.Geolocation` cần bật Location Services | Bắt exception, hiện hướng dẫn thay vì crash |

> **Quyết định nền:** [ADR-0006 — Hai nguồn vị trí](./adr/0006-simulated-location-provider.md)
