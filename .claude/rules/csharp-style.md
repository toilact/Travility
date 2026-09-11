# Quy tắc C#

- Tiền tệ **luôn** `decimal`. Không bao giờ `float`/`double`.
- Thời lượng dùng `TimeSpan`, giờ trong ngày dùng `TimeSpan` hoặc `DateTime.TimeOfDay`.
- Đặt tên theo `CONTEXT.md` mục 6. Không tự nghĩ từ đồng nghĩa.
- Interface: `I` + danh từ. Service: `<Miền>Service`. Engine: `<Vai trò>Engine`.
- Event ở thì quá khứ: `PlaceAdded`, `BudgetChanged`.
- Không `async` trừ khi thật sự gọi I/O (HTTP, file). Truy vấn EF trong đồ án
  này chạy đồng bộ cho đơn giản.
- Không viết comment mô tả lại code. Comment chỉ để giải thích **vì sao**.
- Một phương thức làm một việc. Nếu phải cuộn để đọc hết, tách ra.
- Không bắt `catch (Exception)` rồi nuốt lỗi. Bắt đúng loại, hoặc để nổi lên.
