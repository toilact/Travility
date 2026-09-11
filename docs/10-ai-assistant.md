# 10 — AI Travel Assistant

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) (từ điển thuật ngữ) · [`00-project-overview.md`](./00-project-overview.md)
> **Người phụ trách:** chưa gán cứng — tuần 5
> **⚠ Đây là hạng mục ĐẦU TIÊN bị cắt nếu hết tuần 4 chưa chạy end-to-end.**

---

## Mục tiêu

Chatbot **không phải** hộp hỏi đáp. Luận điểm cần chứng minh (đặc tả §31):

> Chatbot là một **giao diện điều khiển hệ thống bằng ngôn ngữ tự nhiên**.

Người dùng nói *"Thêm quán thứ 2 vào lịch trình tối nay"* → hệ thống thật sự
thêm địa điểm, tính lại tuyến, cập nhật ngân sách.

---

## Quyết định đã chốt

| | |
|---|---|
| Nhà cung cấp | **Gemini API / OpenAI API** (nhóm đã có key) |
| Trừu tượng hoá | `IChatProvider` với `GeminiProvider` + `OpenAiProvider` |
| Cách gọi | `HttpClient` + `Newtonsoft.Json` — **không dùng SDK chính thức** |
| Số tool | **7** (rút từ 16 trong đặc tả §29) |
| Bảo hiểm demo | **Chế độ phát lại** từ bảng `ToolExecutions` |

**Vì sao hai provider:** hết hạn mức bên này đổi sang bên kia ngay giữa buổi
demo. Đúng nguyên tắc §49.1 mà đặc tả gốc đã tự đặt ra, và đúng **Bài 3**.

**Vì sao không dùng SDK chính thức:** phần lớn nhắm .NET hiện đại và hay vướng
khi chạy trên .NET Framework. Cả hai API chỉ là REST + JSON, gọi thẳng đơn giản hơn.

---

## Bộ 7 tool

```
find_places(category, maxPrice, near)
get_trip_budget()
add_place_to_itinerary(placeId, day)
remove_place_from_itinerary(itemId)
optimize_route(day)
generate_itinerary(style)
record_expense(category, amount)
```

Đủ để chứng minh luận điểm §31. Đặc tả gốc liệt kê 16 tool — quá nhiều cho 1 tuần.

---

## Kiến trúc — luật bất di bất dịch

```
Người dùng
   ↓
AI Assistant
   ↓
Tool                      <- AI dừng ở đây, chỉ sinh ra lệnh gọi
   ↓
Business Service          <- ITripService, IBudgetService, IItineraryEngine
   ↓
Repository / DAL
   ↓
SQL Server
```

**Chatbot KHÔNG được truy cập database trực tiếp.** Mỗi tool là một lời gọi
mỏng tới service đã có sẵn của C và D — không viết lại nghiệp vụ.

---

## Ví dụ luồng (đặc tả §30)

```
Người dùng: "Tôi còn 1 triệu. Tìm quán hải sản gần khách sạn tối nay
             dưới 300k/người."

get_current_trip() -> get_trip_budget() -> get_current_hotel()
   -> find_places(category="seafood", maxPrice=300000, near=hotel)
   -> xếp hạng bằng RecommendationEngine
   -> trả Top 5
```

Sau đó *"Thêm quán thứ 2 vào lịch trình tối nay"*:

```
add_place_to_itinerary(placeId, day) -> optimize_route(day)
   -> event PlaceAdded -> BudgetService tự cập nhật
```

---

## Hai điều BẮT BUỘC (không phải tuỳ chọn)

### 1. Chế độ phát lại — bảo hiểm demo

Mọi lệnh gọi tool ghi vào bảng `ToolExecutions`:

```
ToolExecutionId · ChatSessionId · ToolName
Arguments · Result · ExecutedAt · Status
```

Thêm nút **"Phát lại phiên đã ghi"**: đọc một hội thoại thành công từ DB và diễn
lại trên UI. **Mất mạng lúc bảo vệ vẫn demo được.**

Đây là công dụng thứ hai, gần như miễn phí, của một bảng vốn đã phải làm cho
mục đích audit/debug.

### 2. API key đọc từ file config ngoài git

**Không hardcode key trong source.** Nếu nhóm đẩy key lên GitHub public, nó sẽ bị
quét và vô hiệu hoá trong vài giờ.

```
config/api-keys.json     <- thêm vào .gitignore
config/api-keys.example.json  <- commit file mẫu không có key thật
```

---

## Bẫy kỹ thuật .NET Framework

Một số phiên bản .NET Framework không bật TLS 1.2 mặc định → gọi HTTPS lỗi khó
hiểu (`SecureChannelFailure`, không nói rõ nguyên nhân). Thêm dòng này lúc khởi
động app:

```csharp
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
```

---

## Định nghĩa "xong"

- [ ] Nói bằng tiếng Việt tự nhiên, chatbot gọi đúng tool
- [ ] Thêm/xoá địa điểm qua chat thật sự thay đổi lịch trình trong DB
- [ ] Ngân sách tự cập nhật sau thao tác của chatbot
- [ ] Mọi lệnh gọi được ghi vào `ToolExecutions`
- [ ] **Ngắt mạng, nút phát lại vẫn diễn được một phiên hoàn chỉnh**
- [ ] Đổi provider Gemini ↔ OpenAI không cần sửa code, chỉ sửa config
- [ ] Không có API key nào trong git

---

## Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Hardcode API key, đẩy lên GitHub | File config ngoài git, có file `.example` |
| Chatbot truy cập DB trực tiếp cho nhanh | Luôn đi qua Business Service |
| Làm đủ 16 tool | 7 là đủ chứng minh luận điểm |
| Không có phương án dự phòng khi mất mạng | Chế độ phát lại, làm cùng lúc với tool |
| Quên bật TLS 1.2 | Thêm dòng `ServicePointManager` ngay đầu `Program.cs` |
| Test lặp đi lặp lại tốn token | Ghi lại phản hồi mẫu, test tool bằng dữ liệu đã ghi |
| Bắt đầu chatbot khi lõi chưa xong | Điểm dừng khẩn cấp: hết tuần 4 chưa end-to-end thì **bỏ** |

> **Quyết định nền:** [ADR-0007 — IChatProvider + chế độ phát lại](./adr/0007-chat-provider-abstraction.md)
