# 07 — Ba Engine: Recommendation, Routing, Itinerary

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) (từ điển thuật ngữ) · [`00-project-overview.md`](./00-project-overview.md)
> **Người phụ trách:** D
> **Đây là trái tim của đồ án — thứ phân biệt Travility với một phần mềm CRUD.**

---

## Mục tiêu

Ba engine trả lời ba câu hỏi khác nhau:

| Engine | Trả lời |
|---|---|
| **Recommendation** | Nên chọn khách sạn / quán ăn / địa điểm nào? |
| **Routing** | Đi theo thứ tự nào để giảm quãng đường, thời gian, chi phí? |
| **Itinerary** | Chọn những điểm nào, xếp vào ngày nào, giờ nào? |

---

## Lợi thế lớn nhất của module này

`Travility.Core` **không tham chiếu WinForms và không cần DB**. Nghĩa là D có thể
viết engine và chạy thử trên `List<Place>` giả **ngay từ ngày 1**, không chờ A,
không chờ B, không chờ C.

Tận dụng triệt để điều này. Đây là lý do cấu trúc 4 project được chọn.

---

## Sở hữu

```
Travility.Core/Engines/
├── RecommendationEngine.cs
├── RoutingEngine.cs
├── ItineraryEngine.cs
└── Scoring/
    ├── BayesianRating.cs
    └── TravelStyleWeights.cs

Travility.Tests/                 ← unit test cho cả ba engine
Travility.WinForms/Itinerary/
├── ItineraryForm.cs
└── ItineraryComparisonForm.cs
```

---

## 1. Recommendation Engine

### Công thức (đặc tả §16)

```
RecommendationScore = w1*RatingScore + w2*ReviewConfidence + w3*PreferenceScore
                    + w4*LocationScore + w5*PriceScore + w6*ScheduleCompatibility
Sum(w) = 1
```

Trọng số gợi ý: Rating 30% · ReviewConfidence 15% · Preference 20% ·
Location 15% · Price 15% · Schedule 5%.

### Bayesian Rating — giữ bằng mọi giá

Đây là điểm nhấn học thuật rẻ nhất và ấn tượng nhất của cả đồ án. Nó giải quyết
vấn đề "5.0 sao / 4 review" không được phép đứng trên "4.8 sao / 8200 review":

```
WeightedRating = (v/(v+m)) * R + (m/(v+m)) * C

R = rating của địa điểm
v = số review của địa điểm
C = rating trung bình toàn bộ tập địa điểm
m = số review tối thiểu để coi là đủ tin cậy (gợi ý: m = 50)
```

### PreferenceScore

Nối 7 giá trị sở thích của người dùng (1..5) với `PreferenceTags` của địa điểm.
Cách đơn giản và đủ tốt: trung bình điểm sở thích của các tag mà địa điểm mang,
chuẩn hoá về [0,1].

### Top-K bằng LINQ (ăn Bài 7)

```csharp
var topK = places
    .Select(p => new { Place = p, Score = Score(p, ctx) })
    .OrderByDescending(x => x.Score)
    .Take(k)
    .ToList();
```

---

## 2. Routing Engine

### Khoảng cách — quyết định đã chốt

| Mục đích | Dùng gì |
|---|---|
| **Bên trong engine** (chạy hàng vạn lần) | **Haversine x 1.3** |
| **Vẽ tuyến lên bản đồ** (chạy 1 lần) | OSRM / Leaflet Routing Machine |

Hệ số 1.3 là *detour factor* — bù cho việc đường thật không đi thẳng.

**Vì sao không gọi API trong engine:** thuật toán 2-opt đánh giá hàng nghìn đến
hàng chục nghìn tổ hợp cho mỗi lần bấm "Tối ưu". Mỗi lần là một HTTP request thì
người dùng chờ vài phút và hết hạn mức API trong buổi test đầu tiên.

Câu trả lời khi hội đồng hỏi *"sao không dùng khoảng cách đường thật?"*:
> *"Engine cần đánh giá hàng vạn phương án nên dùng xấp xỉ Haversine có hệ số
> hiệu chỉnh; đường thật chỉ dùng ở bước hiển thị tuyến cuối cùng."*

### Distance Matrix

Tính sẵn, lưu vào bảng, truy vấn bằng **stored procedure qua ADO.NET**.
Đây là chỗ thể hiện **Bài 8** có lý do thật, không gượng ép.

### Thuật toán

**Nearest Neighbor + cải thiện 2-opt.** Với n <= 8 có so sánh đối chứng
brute-force để đánh giá chất lượng nghiệm — đây là thứ nên đưa vào báo cáo.

> **KHÔNG tuyên bố** đã triển khai "TSP with Time Windows" hay "Orienteering
> Problem". Viết đúng những gì code làm. Nêu tên một mô hình không triển khai là
> cách nhanh nhất để bị hỏi vặn.

### RouteCost (đặc tả §20)

```
RouteCost = alpha*Distance + beta*TravelTime + gamma*TransportationCost
```

Ràng buộc: `OpeningHourConstraint` · `MealTimeConstraint` · `UserTimeConstraint`.

---

## 3. Itinerary Engine — pipeline tham lam 3 tầng

```
Với mỗi TravelStyle (Budget / Balanced / Experience):

  Tầng 1 — CHỌN
    Xếp hạng toàn bộ POI theo RecommendationScore
    Tham lam nhặt theo tỉ lệ (Score / Chi phí)
    đến khi hết ngân sách hoặc hết quỹ thời gian

  Tầng 2 — CHIA NGÀY
    Gom các điểm đã chọn thành N cụm địa lý (N = số ngày)
    -> mỗi ngày một vùng, không chạy lòng vòng

  Tầng 3 — XẾP GIỜ
    Mỗi ngày: Nearest Neighbor + 2-opt
    Chèn giờ ăn (dùng cờ IsMealPlace)
    Kiểm tra OpenTime / CloseTime
    Tích luỹ VisitDuration + thời gian di chuyển

-> 3 lịch trình để so sánh
```

### Tầng 2 là tầng quan trọng nhất

Bỏ nó đi sẽ sinh ra lịch trình vô lý kiểu *sáng lên Bà Nà, trưa xuống biển Mỹ
Khê, chiều lại lên núi*. Đây cũng là thứ **nhìn thấy được trên bản đồ lúc demo** —
mỗi ngày một cụm, một màu. Ăn điểm trực tiếp.

Cách làm đơn giản mà đủ: k-means trên (lat, lng) với k = số ngày, hoặc chia theo
khoảng cách tới trọng tâm.

### Ba Travel Style (đặc tả §26)

|  | Cost | Distance | Experience | Preference | Rating |
|---|---|---|---|---|---|
| **Budget** | 45% | 20% | 15% | 10% | 10% |
| **Balanced** | 25% | 15% | 25% | 20% | 15% |
| **Experience** | 10% | 10% | 35% | 25% | 20% |

> **Thuật ngữ:** ba phương án A/B/C là **3 preset trọng số**, KHÔNG phải Pareto
> Front. Pareto Front đòi hỏi lọc theo quan hệ dominance trên một tập nghiệm lớn.
> Gọi đúng tên trong báo cáo.

### Ràng buộc cứng

```
TotalCost <= Budget
ActivityTime <= AvailableTime
VisitTime thuộc [OpenTime, CloseTime]
Không xếp hai hoạt động trùng giờ
Mỗi hoạt động đủ VisitDuration
Thời gian di chuyển được tính giữa hai hoạt động
```

---

## 4. Unit test — điểm cộng thật khi bảo vệ

`Travility.Tests` là project tồn tại vì engine là logic thuần, dễ test nhất.
*"Nhóm em có 20 unit test cho engine"* là câu rất ít nhóm nói được.

Những test đáng viết nhất:

- [ ] Ngân sách 5 triệu **không bao giờ** sinh lịch trình > 5 triệu
- [ ] Không địa điểm nào bị xếp ngoài `[OpenTime, CloseTime]`
- [ ] Không hai hoạt động nào trùng giờ trong cùng một ngày
- [ ] Bayesian: địa điểm 5.0 sao / 4 review xếp **dưới** 4.8 sao / 8200 review
- [ ] 2-opt luôn cho kết quả **không tệ hơn** Nearest Neighbor
- [ ] Với n <= 8, NN+2opt cách brute-force không quá X%
- [ ] Travel Style khác nhau cho lịch trình khác nhau trên cùng dữ liệu
- [ ] Tổng `VisitDuration` + di chuyển <= số giờ khả dụng trong ngày

---

## Định nghĩa "xong"

- [ ] Ba engine chạy độc lập, không cần UI, không cần DB
- [ ] Sinh được 3 lịch trình khác nhau rõ rệt từ cùng một chuyến đi
- [ ] `ItineraryComparisonForm` hiện 3 phương án kèm Cost / Distance / Experience
- [ ] Lịch trình xuất ra vẽ được lên bản đồ của B, mỗi ngày một màu
- [ ] Tối ưu xong trong dưới 3 giây với 150 POI
- [ ] >= 15 unit test xanh

---

## Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| Gọi API khoảng cách bên trong vòng lặp tối ưu | Haversine. Luôn. |
| Bỏ tầng chia ngày, xếp thẳng toàn chuyến | Lịch trình sẽ vô lý và lộ ngay trên bản đồ |
| Quên cộng thời gian di chuyển giữa hai điểm | Lịch trình sẽ nhồi 10 điểm vào một ngày |
| Quên `VisitDuration` | Không xếp được giờ, engine vô nghĩa |
| Dùng từ "Pareto" / "TSPTW" trong báo cáo | Gọi đúng tên việc mình làm |
| Chờ A xong DB mới bắt đầu | Viết trên `List<Place>` giả từ ngày 1 |
| Engine tham chiếu `System.Windows.Forms` | Vi phạm quy tắc phụ thuộc, mất khả năng test |

> **Quyết định nền:** [ADR-0004 — Haversine trong engine](./adr/0004-haversine-in-engine-osrm-for-display.md) · [ADR-0005 — Pipeline tham lam ba tầng](./adr/0005-greedy-three-stage-itinerary.md)
