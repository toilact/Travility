# Quy tắc kiểm thử

Test ở `tests/Travility.Tests`. NUnit hoặc xUnit. Không dùng framework mock.

## Test cái gì

✅ Ba engine · hàm tính tiền (`PricingUnit`, phân bổ) · hàm Haversine
❌ Form · EF/Repository · lời gọi LLM

## Bắt buộc

- Dữ liệu test tạo trong code bằng `List<Place>`, **không** đọc CSDL.
- Mỗi test tự dựng dữ liệu của nó — không phụ thuộc thứ tự chạy.
- Dữ liệu test phải đọc được như một câu chuyện, không dùng `place1`, `place2`.
- Tên test: `Phải_<kỳ vọng>_Khi_<điều kiện>`. Tiếng Việt được.
- Viết test **cùng lúc** với engine, không để tới cuối.

## Tối thiểu trước khi bảo vệ

15 test xanh, trong đó có đủ 8 test bắt buộc ở `docs/12-testing-strategy.md` mục 3.
