# ADR-0007 — Chatbot gọi LLM qua `IChatProvider`, có chế độ phát lại

- **Trạng thái:** Chấp nhận
- **Ngày:** 2026-09-11

## Bối cảnh

AI Travel Assistant phải chứng minh được luận điểm: chatbot là **giao diện điều
khiển hệ thống bằng ngôn ngữ tự nhiên**, không phải hộp hỏi đáp.

Ba rủi ro: phụ thuộc mạng lúc bảo vệ, hạn mức API có thể hết giữa chừng, và tốn
token khi test lặp lại. Nhóm đã có sẵn key của cả Gemini và OpenAI.

## Quyết định

- Trừu tượng hoá sau **`IChatProvider`** với `GeminiProvider` và `OpenAiProvider`.
- Gọi bằng `HttpClient` + `Newtonsoft.Json`, **không dùng SDK chính thức**.
- **Bảy tool** (rút từ mười sáu trong đặc tả gốc).
- Mọi lệnh gọi ghi vào bảng `ToolExecutions`, kèm nút **"Phát lại phiên đã ghi"**.
- API key đọc từ `config/api-keys.json`, nằm ngoài git.

## Hệ quả

- Hết hạn mức một nhà cung cấp thì đổi sang nhà cung cấp kia ngay giữa buổi demo,
  chỉ sửa config.
- **Mất mạng lúc bảo vệ vẫn demo được** nhờ chế độ phát lại — đây là công dụng
  thứ hai, gần như miễn phí, của một bảng vốn đã phải làm cho mục đích audit.
- Là chỗ dùng **Bài 3** lần thứ hai, đúng nguyên tắc "không phụ thuộc cứng vào
  một API" mà đặc tả gốc đã tự đặt ra.
- Bảy tool đủ chứng minh luận điểm mà vừa với một tuần công.
- **Ràng buộc:** chatbot **không** được truy cập CSDL trực tiếp. Luồng đúng là
  `AI → Tool → Business Service → Repository → SQL Server`.
- **Bẫy nền tảng:** một số phiên bản .NET Framework không bật TLS 1.2 mặc định.
  Phải đặt `ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;`
  lúc khởi động.

## Phương án đã loại

**Rule-based, không gọi API** — miễn phí và không cần mạng, nhưng cứng nhắc và
dễ lộ là kịch bản dựng sẵn.

**Dùng SDK chính thức của Gemini/OpenAI** — phần lớn nhắm .NET hiện đại và hay
vướng khi chạy trên .NET Framework. Cả hai API chỉ là REST + JSON.

**Làm đủ mười sáu tool như đặc tả gốc** — quá nhiều cho một tuần, và không thêm
sức thuyết phục nào so với bảy.
