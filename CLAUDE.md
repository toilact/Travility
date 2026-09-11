# CLAUDE.md — Travility

Đồ án môn **Lập trình trên môi trường Windows**. Ứng dụng desktop hỗ trợ lập kế
hoạch du lịch: đề xuất địa điểm theo sở thích và ngân sách, tối ưu tuyến đường,
sinh nhiều phương án lịch trình để so sánh.

**C# · .NET Framework · Windows Forms · SQL Server (Database First) · EF + ADO.NET**
4 người · 6 tuần · thuyết trình **23/10/2026** · https://github.com/toilact/Travility

---

## Đọc gì trước khi làm gì

| Việc | Đọc |
|---|---|
| **Bất cứ việc gì** | [`CONTEXT.md`](./CONTEXT.md) — từ điển thuật ngữ |
| Định hướng chung | [`docs/00-project-overview.md`](./docs/00-project-overview.md) |
| Viết code nghiệp vụ | [`docs/04-internal-contracts.md`](./docs/04-internal-contracts.md) |
| Đụng tới CSDL | [`docs/03-database-design.md`](./docs/03-database-design.md) |
| Làm giao diện | [`docs/05-ui-guidelines.md`](./docs/05-ui-guidelines.md) |
| Vì sao quyết định thế này | [`docs/adr/`](./docs/adr/) |

`docs/archive/` là tài liệu lịch sử — **không phải nguồn sự thật**.

---

## Ràng buộc không được phá

1. **Phụ thuộc một chiều:** `WinForms → Core → Data`.
   `Travility.Core` không được tham chiếu `System.Windows.Forms`.
2. **Tiền tệ luôn là `decimal`.** Không bao giờ `float` hay `double`.
3. **Không commit** `bin/`, `obj/`, `packages/`, `.vs/`, API key, mật khẩu.
4. **Không sửa Form của người khác** — `.Designer.cs` merge rất tệ.
5. **Không đổi chữ ký interface** trong `Travility.Core` mà chưa báo cả nhóm.
6. **Chỉ một người sửa** `database/schema.sql`.
7. **Chatbot không truy cập CSDL trực tiếp:**
   `AI → Tool → Service → Repository → SQL Server`.

---

## Thuật ngữ cấm dùng

Không viết **"Pareto Front"**, **"TSP with Time Windows"**, **"Orienteering
Problem"** trong code, tài liệu hay báo cáo — dự án không triển khai đúng những
mô hình đó. Xem mục 5 của [`CONTEXT.md`](./CONTEXT.md) để biết dùng từ gì thay thế.

Tương tự, không nói "tích hợp Google Places" (dataset tự xây) hay "đặt vé"
(là mô phỏng).

---

## Quy ước làm việc

- **Trả lời bằng tiếng Việt.**
- Dùng đúng thuật ngữ trong `CONTEXT.md` khi đặt tên biến, bảng, test, issue.
- Mọi thay đổi vào `main` đi qua Pull Request — xem
  [`docs/13-git-workflow.md`](./docs/13-git-workflow.md).
- Quy tắc viết code chi tiết: [`.claude/rules/`](./.claude/rules/).

---

## Agent skills

### Issue tracker

GitHub Issues trên `toilact/Travility`, thao tác bằng `gh` CLI.
Xem `docs/agents/issue-tracker.md`.

### Triage labels

Năm nhãn mặc định: `needs-triage`, `needs-info`, `ready-for-agent`,
`ready-for-human`, `wontfix`. Xem `docs/agents/triage-labels.md`.

### Domain docs

Single-context: `CONTEXT.md` ở gốc repo + `docs/adr/`.
Xem `docs/agents/domain.md`.
