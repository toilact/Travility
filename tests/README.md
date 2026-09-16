# tests/

`Travility.Tests` — architecture test và unit test cho ba engine.

Test logic thuần (engine, tính tiền, Haversine). Không test Form, không test EF,
không test lời gọi LLM.

Dữ liệu test tạo bằng `List<Place>` trong code, không đọc CSDL.

Test architecture đầu tiên bảo vệ ràng buộc `Travility.Core` không tham chiếu
WinForms. Tối thiểu 15 test xanh trước khi bảo vệ, gồm đủ 8 test bắt buộc.

Chi tiết: [`docs/12-testing-strategy.md`](../docs/12-testing-strategy.md)
