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

### Yêu cầu tiên quyết (Prerequisites)

- Visual Studio 2022 (Workload *.NET desktop development*, bao gồm .NET Framework 4.8 Targeting Pack)
- Microsoft SQL Server 2022 hoặc SQL Server Express (`.\SQLEXPRESS`)
- `sqlcmd` (đi kèm SQL Server hoặc Command Line Utilities)
- NuGet CLI (`nuget.exe`)

### 1. Khởi tạo cơ sở dữ liệu

Chạy các script SQL theo đúng thứ tự sau bằng `sqlcmd` (hoặc mở lần lượt trong SSMS trên database `TravilityDev`):

```powershell
sqlcmd -S .\SQLEXPRESS -E -b -i database\reset-dev.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\schema.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\seed_reference.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\seed_demo.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\procedures.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\smoke_test.sql
```

### 2. Tài khoản Demo xác thực

Dữ liệu mẫu đã bao gồm hai tài khoản thử nghiệm phân quyền:

| Vai trò | Tên đăng nhập | Email | Mật khẩu mặc định |
|---|---|---|---|
| **Admin** | `admin` | `admin@travility.local` | `Admin@12345` |
| **Traveler** | `traveler` | `traveler@travility.local` | `Traveler@12345` |

### 3. Biên dịch và Chạy Kiểm thử (Build & Test)

Khôi phục package và biên dịch giải pháp qua command line:

```powershell
nuget restore Travility.sln
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll
msbuild Travility.sln /m /p:Configuration=Release
```

### 4. Cấu hình & Vị trí Nhật ký (Logging)

- **Chuỗi kết nối CSDL:** Cấu hình tại [src/Travility.WinForms/App.config](src/Travility.WinForms/App.config) (mặc định trỏ tới `.\SQLEXPRESS` và database `TravilityDev`).
- **Tập tin Log:** Được ghi hàng ngày tại thư mục `%LocalAppData%\Travility\Logs\travility-yyyyMMdd.log`. Hệ thống tự động lưu trữ và xoay vòng dọn dẹp sau 14 ngày.

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
