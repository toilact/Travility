# 05 — Quy ước giao diện WinForms

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md)
> **Mục đích:** để bốn người làm ra một giao diện, không phải bốn.

---

## 1. Luật vàng

> **Mỗi Form chỉ đúng một người chạm.**

`.Designer.cs` là file sinh tự động. Hai người cùng mở một Form trong Designer
rồi merge = hỏng form, phải dựng lại tay. Nếu cần sửa Form của người khác, nhắn
họ, đừng tự mở.

---

## 2. Đặt tên

### Form

```
<Danh từ><Loại>Form      LoginForm · TripWizardForm · BudgetDashboardForm
```

### Control — tiền tố theo loại

| Loại | Tiền tố | Ví dụ |
|---|---|---|
| Button | `btn` | `btnSave`, `btnOptimize` |
| TextBox | `txt` | `txtBudget` |
| Label | `lbl` | `lblPlannedCost` |
| ComboBox | `cbo` | `cboTravelStyle` |
| CheckBox | `chk` | `chkShowHotels` |
| DataGridView | `dgv` | `dgvPlaces` |
| DateTimePicker | `dtp` | `dtpDepartDate` |
| NumericUpDown | `num` | `numPeople` |
| TabControl | `tab` | `tabItinerary` |
| Panel | `pnl` | `pnlFilters` |
| TrackBar | `trk` | `trkBeach` |
| Chart | `cht` | `chtExpenseByCategory` |

Không để tên mặc định `button1`, `textBox3`. Sáu tuần sau không ai đọc nổi.

---

## 3. Bố cục

| | |
|---|---|
| Kích thước Form chính | 1280 × 800, cho phép resize |
| Lề ngoài | 16 px |
| Khoảng cách giữa control | 8 px |
| Khoảng cách giữa nhóm | 16 px |
| Chiều cao nút | 32 px |
| Chiều cao ô nhập | 28 px |

**Dùng `TableLayoutPanel` / `FlowLayoutPanel`** thay vì đặt toạ độ tuyệt đối.
Form đặt cứng toạ độ sẽ vỡ khi máy demo có DPI khác máy phát triển — đây là lỗi
hay gặp và chỉ lộ ra vào đúng hôm bảo vệ.

Đặt `AutoScaleMode = Dpi` cho mọi Form.

---

## 4. Font và màu

```
Font chính      Segoe UI, 9pt
Tiêu đề         Segoe UI Semibold, 14pt
Số liệu lớn     Segoe UI Light, 24pt
```

Bảng màu — dùng đúng sáu màu này, không tự thêm:

| Vai trò | Mã | Dùng cho |
|---|---|---|
| Chính | `#2A6F97` | Nút chính, tiêu đề, thanh điều hướng |
| Phụ | `#468FAF` | Nút phụ, viền được chọn |
| Thành công | `#2D6A4F` | Check-in thành công, trong ngân sách |
| Cảnh báo | `#BC6C25` | Gần vượt ngân sách |
| Lỗi | `#9B2226` | Vượt ngân sách, lỗi nhập liệu |
| Nền | `#F8F9FA` | Nền Form |

Màu theo ngày trong lịch trình (khớp với màu tuyến trên bản đồ):

```
Ngày 1 #E63946   Ngày 2 #2A9D8F   Ngày 3 #E9C46A
Ngày 4 #6A4C93   Ngày 5 #F4A261
```

---

## 5. Thông báo và lỗi

### Validation dùng `ErrorProvider`, không dùng `MessageBox`

```csharp
if (dtpReturn.Value <= dtpDepart.Value)
{
    errorProvider.SetError(dtpReturn, "Ngày về phải sau ngày đi");   // BR-01
    return;
}
```

`MessageBox` chỉ dùng cho: xác nhận hành động phá huỷ, và báo lỗi hệ thống
(mất kết nối CSDL).

### Thông báo phải nói người dùng làm gì tiếp

```
❌ "Lỗi"
❌ "Invalid input"
✅ "Ngân sách phải lớn hơn 0"
✅ "Không kết nối được cơ sở dữ liệu. Kiểm tra SQL Server đã chạy chưa."
```

Toàn bộ văn bản giao diện bằng **tiếng Việt có dấu**.

---

## 6. Định dạng dữ liệu

```
Tiền        1.250.000 ₫           ToString("N0") + " ₫"
Ngày        20/10/2026            "dd/MM/yyyy"
Giờ         08:30                 "HH:mm"
Khoảng cách 2,4 km                ToString("N1") + " km"
Thời lượng  1h25m
Rating      4,7 ★ (2.580 đánh giá)
```

---

## 7. Thao tác chạy lâu

Bất cứ thao tác nào có thể quá 1 giây (tối ưu lịch trình, gọi LLM, tải bản đồ)
phải có phản hồi thị giác:

- `Cursor.Current = Cursors.WaitCursor`, hoặc
- `ProgressBar` ở dạng `Marquee`, và
- **vô hiệu hoá nút** để tránh bấm hai lần.

Không để giao diện đứng hình không giải thích — lúc demo trông như treo máy.

---

## 8. Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Đặt toạ độ tuyệt đối → vỡ ở DPI khác | `TableLayoutPanel` + `AutoScaleMode = Dpi` |
| Tên control mặc định `button1` | Tiền tố ở mục 2, đặt tên ngay khi kéo thả |
| Mỗi người một bảng màu | Dùng đúng sáu màu ở mục 4 |
| `MessageBox` cho mọi lỗi validation | `ErrorProvider` |
| Chuỗi tiếng Việt hardcode rải rác | Gom vào một lớp `Strings` tĩnh nếu có thời gian |
| Giao diện đứng hình khi tối ưu | `WaitCursor` + vô hiệu hoá nút |
| Hai người sửa cùng một Form | Luật vàng ở mục 1 |
