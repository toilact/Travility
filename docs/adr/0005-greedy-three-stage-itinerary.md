# ADR-0005 — Itinerary Engine dùng pipeline tham lam ba tầng

- **Trạng thái:** Chấp nhận
- **Ngày:** 2026-09-11

## Bối cảnh

Đặc tả gốc mô tả *kết quả mong muốn* (ba phương án, tối ưu đa mục tiêu) nhưng
không nói *làm thế nào*. "Tối ưu đa mục tiêu" nghe gọn nhưng không có hình hài
để code.

Thực chất đây là **ba bài toán chồng lên nhau**:

| Tầng | Câu hỏi | Bài toán kinh điển |
|---|---|---|
| Chọn | Trong 150 địa điểm, đưa cái nào vào chuyến 4 ngày? | Knapsack |
| Chia ngày | Địa điểm nào đi ngày nào? | Clustering |
| Xếp thứ tự | Trong một ngày, đi theo thứ tự nào, mấy giờ? | TSP + Time Window |

## Quyết định

Pipeline tham lam ba tầng, chạy ba lần với ba bộ trọng số `TravelStyle`:

```
1. CHỌN     tham lam theo tỉ lệ (RecommendationScore / Chi phí)
            đến khi hết Budget hoặc hết quỹ thời gian
2. CHIA NGÀY gom thành N cụm địa lý (N = số ngày)
3. XẾP GIỜ  Nearest Neighbor + 2-opt, chèn giờ ăn,
            kiểm tra OpenTime/CloseTime, tích luỹ VisitDuration + TravelTime
```

## Hệ quả

- Mỗi tầng **test độc lập được**; nếu tầng 3 chưa xong vẫn demo được tầng 1+2.
- Giải thích được trong ba câu khi bảo vệ.
- Chắc chắn hoàn thành trong sáu tuần.
- **Tầng 2 là tầng quyết định lịch trình trông có thông minh hay không.** Bỏ nó
  sẽ sinh ra lịch trình vô lý kiểu *sáng lên núi, trưa xuống biển, chiều lại lên
  núi*. Đây cũng là thứ nhìn thấy được trên bản đồ — mỗi ngày một cụm, một màu.
- **Chi phí:** nghiệm tham lam không tối ưu toàn cục. Chấp nhận được, và phải nói
  đúng điều này trong báo cáo.
- **Ràng buộc thuật ngữ:** ba phương án A/B/C là **ba preset trọng số**, không
  phải Pareto Front — Pareto Front đòi hỏi lọc dominance trên tập nghiệm lớn.
  Xem mục "Thuật ngữ cấm dùng" trong `CONTEXT.md`.

## Phương án đã loại

**Simulated Annealing / Genetic Algorithm trên toàn lịch trình** — một hàm mục
tiêu duy nhất, metaheuristic tự tìm nghiệm. Ấn tượng hơn khi thuyết trình nhưng
khó debug, khó giải thích vì sao ra kết quả đó, và dễ sinh lịch trình vô lý mà
không biết sửa ở đâu.

**Chỉ tối ưu thứ tự, để người dùng tự chọn địa điểm và tự chia ngày** — đơn giản
nhất nhưng bỏ mất phần lớn giá trị của hệ thống hỗ trợ quyết định.
