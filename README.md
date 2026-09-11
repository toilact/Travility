# Travility

**Intelligent Travel Planning & Cost Optimization System**

Ứng dụng desktop hỗ trợ lập kế hoạch du lịch: khám phá địa điểm trên bản đồ đa
lớp, tạo chuyến đi, quản lý ngân sách, nhận đề xuất theo sở thích, tối ưu tuyến
đường và sinh nhiều phương án lịch trình để so sánh.

Đồ án môn **Lập trình trên môi trường Windows** · nhóm 4 người · thuyết trình 23/10/2026.

---

## Công nghệ

| | |
|---|---|
| Ngôn ngữ | C# |
| Nền tảng | .NET Framework |
| Giao diện | Windows Forms |
| CSDL | Microsoft SQL Server (Database First) |
| Truy cập dữ liệu | Entity Framework + ADO.NET |
| Truy vấn | LINQ |
| Bản đồ | WebView2 + Leaflet.js + OpenStreetMap |
| AI | Gemini API / OpenAI API (tool calling) |

---

## Ba engine cốt lõi

Đây là thứ phân biệt Travility với một phần mềm CRUD quản lý tour:

| Engine | Trả lời |
|---|---|
| **Recommendation** | Nên chọn khách sạn / quán ăn / địa điểm nào? |
| **Routing** | Đi theo thứ tự nào để giảm quãng đường, thời gian, chi phí? |
| **Itinerary** | Chọn những điểm nào, xếp vào ngày nào, giờ nào? |

---

## Cấu trúc repo

```
CONTEXT.md              Từ điển thuật ngữ — đọc trước khi đặt tên bất cứ thứ gì
CLAUDE.md               Chỉ dẫn cho AI agent làm việc trong repo

docs/
├── 00 … 15             Tài liệu thiết kế, đánh số theo thứ tự đọc
├── adr/                Quyết định kiến trúc và lý do
├── agents/             Cấu hình cho bộ skill engineering
└── archive/            Tài liệu lịch sử — KHÔNG phải nguồn sự thật

src/                    Travility.Data · Travility.Core · Travility.WinForms
tests/                  Travility.Tests
database/               schema.sql · seed · stored procedure
config/                 api-keys.example.json
```

---

## Bắt đầu

### Yêu cầu

- Visual Studio (có workload .NET desktop development)
- SQL Server hoặc SQL Server Express
- WebView2 Runtime

### Cài đặt

```bash
git clone https://github.com/toilact/Travility.git
cd Travility

git config user.name  "Tên của bạn"
git config user.email "email@cua.ban"
git config pull.rebase true
```

1. Chạy `database/schema.sql` rồi `database/seed_*.sql` trên SQL Server.
2. Sửa connection string trong `src/Travility.WinForms/App.config`.
3. Copy `config/api-keys.example.json` thành `config/api-keys.json`, điền key
   (chỉ cần nếu chạy chatbot).
4. Mở `Travility.sln`, build, chạy.

---

## Đọc tài liệu theo thứ tự nào

| Bạn là | Đọc |
|---|---|
| Người mới vào dự án | `CONTEXT.md` → `docs/00-project-overview.md` |
| Sắp viết code | thêm `docs/04-internal-contracts.md` |
| Làm module cụ thể | `docs/06` … `docs/10` theo phân công |
| Muốn biết vì sao chọn thế này | `docs/adr/` |
| Chuẩn bị bảo vệ | `docs/14-packaging-and-demo.md` |

---

## Đóng góp

Mọi thay đổi vào `main` đi qua Pull Request.
Đọc [`docs/13-git-workflow.md`](./docs/13-git-workflow.md) trước commit đầu tiên —
đặc biệt ba luật về `.gitignore`, `.Designer.cs` và `.csproj`.
