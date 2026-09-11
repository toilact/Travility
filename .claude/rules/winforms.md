# Quy tắc WinForms

- **Mỗi Form chỉ một người chạm.** Không mở Form của người khác trong Designer.
- Đặt tên control theo tiền tố: `btn` `txt` `lbl` `cbo` `chk` `dgv` `dtp` `num`
  `tab` `pnl` `trk` `cht`. Không để `button1`, `textBox3`.
- Dùng `TableLayoutPanel`/`FlowLayoutPanel`, **không** đặt toạ độ tuyệt đối.
- Mọi Form đặt `AutoScaleMode = Dpi`.
- Validation dùng `ErrorProvider`, không dùng `MessageBox`.
  `MessageBox` chỉ cho xác nhận hành động phá huỷ và lỗi hệ thống.
- Thông báo lỗi nói người dùng phải làm gì, không chỉ nói "Lỗi".
- Văn bản giao diện bằng tiếng Việt có dấu.
- Thao tác > 1 giây: `WaitCursor` + vô hiệu hoá nút để tránh bấm hai lần.
- Form **không** gọi `DbContext`. Luôn đi qua Service ở tầng `Core`.

Chi tiết bảng màu, font, khoảng cách: `docs/05-ui-guidelines.md`.
