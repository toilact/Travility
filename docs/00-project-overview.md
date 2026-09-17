# 00 — Tổng quan dự án

> **Đọc trước mọi phiên làm việc.** Kèm [`CONTEXT.md`](../CONTEXT.md) nếu bạn sắp
> viết code hoặc đặt tên bất cứ thứ gì.

| | |
|---|---|
| **Dự án** | Travility — Intelligent Travel Planning & Cost Optimization System |
| **Môn** | Lập trình trên môi trường Windows |
| **Repo** | https://github.com/toilact/Travility |
| **Bắt đầu** | 11/09/2026 · **Thuyết trình** 23/10/2026 · **Còn 6 tuần** |
| **Nhóm** | 4 người, cả 4 đều code được |

---

## 1. Bài toán

Ứng dụng desktop hỗ trợ lập kế hoạch du lịch. **Không phải phần mềm CRUD quản lý
tour** — điểm cốt lõi là hệ hỗ trợ ra quyết định: đề xuất địa điểm theo sở thích
và ngân sách, tối ưu tuyến đường, sinh nhiều phương án lịch trình để so sánh.

```
Đăng nhập → Tạo Trip → Chọn sở thích → Chọn phương tiện →
Đề xuất Top-K → Chọn Place → Tối ưu tuyến → Sinh 3 Itinerary →
Chọn phương án → Đi (CheckIn, ghi Expense) → TravelPassport & Achievement
```

Ba thành phần tạo nên giá trị: **Recommendation Engine** (chọn gì),
**Routing Engine** (đi thứ tự nào), **Itinerary Optimization Engine** (xếp ngày giờ nào).

---

## 2. Bản đồ tài liệu

| Bạn cần | Đọc |
|---|---|
| Thuật ngữ chuẩn, tên biến, tên bảng | [`CONTEXT.md`](../CONTEXT.md) |
| Phạm vi, actor, use case | [`01-requirements.md`](./01-requirements.md) |
| 4 project, phụ thuộc, event | [`02-system-architecture.md`](./02-system-architecture.md) |
| Bảng, cột, ERD | [`03-database-design.md`](./03-database-design.md) |
| Chữ ký interface, JSON bridge, tool schema | [`04-internal-contracts.md`](./04-internal-contracts.md) |
| Quy ước giao diện WinForms | [`05-ui-guidelines.md`](./05-ui-guidelines.md) |
| Làm module của tôi | `06` … `10` theo phân công bên dưới |
| Mật khẩu, API key, phân quyền | [`11-security.md`](./11-security.md) |
| Viết test gì | [`12-testing-strategy.md`](./12-testing-strategy.md) |
| Nhánh, commit, PR | [`13-git-workflow.md`](./13-git-workflow.md) |
| Đóng gói, kịch bản demo | [`14-packaging-and-demo.md`](./14-packaging-and-demo.md) |
| Lộ trình, tài liệu phải nộp | [`15-roadmap-and-deliverables.md`](./15-roadmap-and-deliverables.md) |
| Vì sao quyết định như vậy | [`adr/`](./adr/) |

**Thứ tự ưu tiên khi mâu thuẫn:** `CONTEXT.md` → `adr/` → `docs/NN-*.md` →
[`archive/`](./archive/). Tài liệu trong `archive/` là bản mô tả tham vọng ban
đầu, nhiều chỗ đã bị sửa có chủ đích — **không phải nguồn sự thật**.

---

## 3. Ràng buộc cứng

| | |
|---|---|
| Nền tảng | C# · **.NET Framework** · Windows Forms · Visual Studio |
| CSDL | Microsoft SQL Server, **Database First** ([ADR-0001](./adr/0001-database-first.md)) |
| Truy cập dữ liệu | Entity Framework (CRUD) **+** ADO.NET (stored procedure) |
| Ngân sách nhân lực | 4 người × 6 tuần ≈ **380–480 giờ-người** |
| Dữ liệu | **Tự xây**, 120–150 `Place`, một thành phố (TP.HCM) |
| Máy demo | Laptop cá nhân của nhóm |

---

## 4. Phân công

| Người | Sở hữu | Tài liệu |
|---|---|---|
| **A** (lead) | `schema.sql`, seed, DbContext, Repository, Auth, MainForm, Admin | `03`, `11` |
| **B** | Smart Map, Layers, PlaceDetail, cầu WebView2 ↔ C# | `06` |
| **C** | Trip Wizard, Transport, Booking, Budget, Expense | `08` |
| **D** | 3 Engine, Itinerary UI, Comparison, unit test | `07`, `12` |

Check-in/Gamification (`09`) và Chatbot (`10`) chưa gán cứng — ghép tuần 4–5 cho
ai xong trước.

---

## 5. Ba luật git không được phá

Chi tiết ở [`13-git-workflow.md`](./13-git-workflow.md). Tóm tắt:

1. **Không commit `bin/`, `obj/`, `packages/`, `.vs/`.**
2. **Không ai mở Form của người khác trong Designer.**
3. **Conflict trên `.csproj`: giữ CẢ HAI dòng** `<Compile Include=...>`.

Mọi thay đổi vào `main` đi qua Pull Request. Một người approve; nếu sau 12 giờ
không ai review thì người tạo PR được tự merge.

---

## 6. Việc quan trọng nhất tuần 1

> **Ngày 2–3: chốt toàn bộ interface trong `Travility.Core`, merge một lần duy
> nhất.** Sau đó không ai sửa chữ ký nếu chưa báo cả nhóm.

Vì nhóm chọn merge tự do (không theo lịch), đây là thứ thay thế kỷ luật merge —
bốn người code cùng một bản hợp đồng thì sự trôi lệch không còn chỗ xảy ra.
Chữ ký cụ thể: [`04-internal-contracts.md`](./04-internal-contracts.md).

---

## 7. Hai điểm dừng khẩn cấp

Quyết định trước, để lúc đó không ai phải tranh cãi trong lúc đang hoảng:

- Hết tuần 4 **chưa chạy end-to-end** → **bỏ chatbot**, dồn tuần 5 vào lõi.
- Hết tuần 5 lõi còn lỗi nặng → **bỏ huy hiệu nâng cao**, giữ check-in + passport cơ bản.

**Tuần 6 không viết tính năng mới.** Đồ án điểm thấp hiếm khi vì thiếu chức năng
— thường vì demo vỡ, vì lỗi khi thầy bấm thử, vì không ai tập nói trước.
