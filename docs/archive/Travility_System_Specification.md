# TRAVILITY
## Intelligent Travel Planning & Cost Optimization System

> **Loại tài liệu:** Đặc tả tổng quan hệ thống  
> **Phiên bản:** 1.0  
> **Nền tảng dự kiến:** C# – .NET Framework – Windows Forms – Microsoft SQL Server  
> **Mục tiêu:** Xây dựng hệ thống hỗ trợ lập kế hoạch du lịch, quản lý ngân sách, đề xuất dịch vụ, tối ưu tuyến đường và tối ưu lịch trình.

---

## 1. Tầm nhìn của hệ thống

Travility là ứng dụng desktop hỗ trợ khách du lịch trong toàn bộ quá trình:

**Khám phá → Lập chuyến đi → Chọn phương tiện → Chọn nơi ở → Chọn địa điểm → Tối ưu tuyến → Tối ưu lịch trình → Quản lý ngân sách → Check-in → Nhận thành tích**

Hệ thống không chỉ lưu trữ thông tin chuyến đi mà đóng vai trò như một **Travel Decision Support System** – hệ thống hỗ trợ ra quyết định du lịch dựa trên:

- Ngân sách.
- Thời gian.
- Vị trí.
- Khoảng cách.
- Sở thích.
- Rating.
- Review.
- Phương tiện.
- Chất lượng dịch vụ.
- Mức độ ưu tiên.
- Chi phí di chuyển.
- Thời gian mở cửa.
- Lịch trình hiện tại.

---

## 2. Kiến trúc chức năng tổng thể

```text
                         TRAVILITY
                             │
          ┌──────────────────┼───────────────────┐
          │                  │                   │
          ▼                  ▼                   ▼
     DISCOVERY          TRIP PLANNING        AI ASSISTANT
          │                  │                   │
     Smart Map          Trip Builder          Chatbot
     Places             Transportation        Tool Calling
     Search             Budget
     Top-K              Itinerary
          │                  │
          └──────────┬───────┘
                     ▼
              INTELLIGENCE ENGINE
                     │
        ┌────────────┼────────────┐
        ▼            ▼            ▼
 Recommendation    Routing     Scheduling
    Engine          Engine       Engine
        │            │            │
        └────────────┼────────────┘
                     ▼
              Optimization Engine
                     │
                     ▼
                 SQL SERVER
```

---

## 3. Module A – Smart Travel Map

Travility phải có một bản đồ trực quan làm trung tâm cho việc khám phá chuyến đi.

### 3.1. Chức năng chính

Bản đồ có thể hiển thị:

- Vị trí người dùng.
- Khách sạn.
- Nhà hàng.
- Quán ăn.
- Địa điểm tham quan.
- Địa điểm lịch sử.
- Bảo tàng.
- Khu vui chơi.
- Bến xe.
- Ga tàu.
- Sân bay.
- Địa điểm đã check-in.
- Địa điểm nằm trong chuyến đi hiện tại.

### 3.2. Marker theo loại địa điểm

Ví dụ:

```text
🏨 Hotel
🍜 Restaurant
🏛 Historical Place
🎡 Entertainment
🏖 Attraction
✈ Airport
🚉 Train Station
🚌 Bus Station
✓ Checked-in
```

### 3.3. Thông tin khi chọn marker

Ví dụ:

```text
┌────────────────────────────┐
│ Bảo tàng X                 │
│ ⭐ 4.7 / 5                 │
│ 2,580 reviews              │
│                            │
│ Khoảng cách: 2.4 km        │
│ Vé: 50,000 VNĐ             │
│                            │
│ [Chi tiết]                 │
│ [Thêm vào chuyến đi]       │
│ [Chỉ đường]                │
└────────────────────────────┘
```

---

## 4. Map Layer System

Bản đồ được chia thành nhiều lớp để tránh hiển thị quá nhiều marker cùng lúc.

Ví dụ:

```text
MAP LAYERS

☑ Tourist Attractions
☑ Historical Places
☐ Hotels
☑ Restaurants
☐ Coffee Shops
☐ Entertainment
☐ Shopping
☐ Hospitals
☐ Transportation
☑ My Itinerary
☑ Checked-in Places
```

Người dùng có thể bật hoặc tắt từng lớp.

Ví dụ khi bật:

```text
☑ Hotels
☑ Restaurants
☑ Historical Places
```

bản đồ chỉ hiển thị các marker thuộc ba nhóm trên.

---

## 5. Module B – Trip Builder

Người dùng tạo chuyến đi thông qua một quy trình theo từng bước.

### 5.1. Bước 1 – Thông tin chuyến đi

```text
Điểm xuất phát:
TP. Hồ Chí Minh

Điểm đến:
Đà Nẵng

Ngày đi:
20/10/2026

Ngày về:
24/10/2026

Số người:
2

Ngân sách:
10,000,000 VNĐ
```

### 5.2. Bước 2 – Sở thích

Người dùng có thể đánh giá mức độ yêu thích theo thang điểm.

```text
Biển             █████ 5
Ẩm thực          ████  4
Lịch sử          ███   3
Văn hóa          ████  4
Giải trí         ██    2
Mua sắm          ██    2
Thiên nhiên      █████ 5
```

Thông tin này được sử dụng bởi **Recommendation Engine** và **Itinerary Optimization Engine**.

---

## 6. Module C – Transportation & Booking

Sau khi chọn điểm đi, điểm đến và thời gian, hệ thống tìm kiếm các phương án di chuyển phù hợp.

### 6.1. Loại phương tiện

```text
✈ Máy bay
🚆 Tàu hỏa
🚌 Xe khách
🚗 Xe cá nhân
```

### 6.2. Ví dụ kết quả

```text
TP.HCM → Đà Nẵng

────────────────────────

Vietnam Airlines

08:00 → 09:25

Thời gian: 1h25m

Giá: 1,450,000 VNĐ

[Chọn]

────────────────────────

Train SE2

19:30 → 12:45

Thời gian: 17h15m

Giá: 850,000 VNĐ

[Chọn]

────────────────────────

Bus

18:00 → 09:00

Thời gian: 15h

Giá: 480,000 VNĐ

[Chọn]
```

Chi phí phương tiện được đưa trực tiếp vào tổng ngân sách chuyến đi.

### 6.3. Phạm vi booking trong đồ án

Trong phiên bản đồ án, hệ thống nên hỗ trợ đầy đủ luồng:

**Tìm kiếm → So sánh → Lựa chọn → Ghi nhận booking → Quản lý trạng thái**

Thanh toán hoặc xuất vé thật qua nhà cung cấp bên ngoài có thể được xem là chức năng mở rộng vì phụ thuộc API thương mại.

---

## 7. Booking Model

Một booking có thể gồm các thuộc tính sau:

```text
Booking
- BookingId
- TripId
- UserId
- BookingType
- Provider
- DepartureLocation
- ArrivalLocation
- DepartureTime
- ArrivalTime
- Price
- NumberOfPassengers
- BookingStatus
- BookingReference
```

### 7.1. BookingType

```text
Flight
Bus
Train
Hotel
```

### 7.2. BookingStatus

```text
Pending
Reserved
Confirmed
Cancelled
```

---

## 8. Module D – Travel Wallet & Budget Management

Travility cần phân biệt hai khái niệm.

### 8.1. Travel Wallet

Là tổng số tiền người dùng dành cho hoạt động du lịch.

Ví dụ:

```text
Travel Wallet

15,000,000 VNĐ
```

### 8.2. Trip Budget

Là số tiền được phân bổ cho một chuyến cụ thể.

Ví dụ:

```text
Đà Nẵng Trip

Budget:
8,000,000 VNĐ
```

---

## 9. Phân bổ ngân sách

Hệ thống có thể đề xuất tỷ lệ phân bổ ngân sách theo nhóm.

```text
Ngân sách: 8,000,000 VNĐ

Transportation      25%
Accommodation       30%
Food                20%
Activities          15%
Local Transport      5%
Reserve               5%
```

Ví dụ thành tiền:

```text
Transportation
2,000,000

Accommodation
2,400,000

Food
1,600,000

Activities
1,200,000

Local Transport
400,000

Emergency
400,000
```

---

## 10. Budget Dashboard

```text
Budget
8,000,000 VNĐ

Spent / Planned
████████████░░░░
6,350,000 VNĐ

Remaining
1,650,000 VNĐ

────────────────────────

Transport       1,800,000
Hotel           2,200,000
Food            1,100,000
Attractions       750,000
Local Travel      500,000
```

Mỗi khi người dùng:

- Chọn khách sạn.
- Chọn hoặc đặt vé.
- Thêm nhà hàng.
- Thêm địa điểm.
- Thay đổi phương tiện.
- Thay đổi lịch trình.

**Budget Engine** phải tự động tính lại tổng chi phí, chi phí dự kiến và số tiền còn lại.

---

## 11. Module E – Check-in System

Người dùng có thể check-in tại địa điểm khi đang ở đủ gần địa điểm đó.

Ví dụ:

```text
Bạn đang ở:

Bà Nà Hills

Distance:
48 metres

[CHECK IN]
```

### 11.1. Điều kiện check-in

```text
Distance(UserLocation, PlaceLocation) ≤ CheckInRadius
```

Ví dụ:

```text
CheckInRadius = 150 metres
```

Nếu hợp lệ:

```text
CHECK-IN SUCCESSFUL
```

### 11.2. CheckIn Model

```text
CheckIn
- CheckInId
- UserId
- PlaceId
- TripId
- CheckInTime
- Latitude
- Longitude
- DistanceFromPlace
```

---

## 12. Gamification

Check-in được sử dụng để xây dựng hệ thống thành tích và tạo động lực khám phá.

Ví dụ:

### 12.1. First Step

```text
🏅 First Step

Check-in địa điểm đầu tiên.
```

### 12.2. Beach Explorer

```text
🏖 Beach Explorer

Check-in 5 bãi biển.
```

### 12.3. History Hunter

```text
🏛 History Hunter

Check-in 10 địa điểm lịch sử.
```

### 12.4. Food Explorer

```text
🍜 Food Explorer

Check-in 20 nhà hàng hoặc quán ăn.
```

### 12.5. Travel Master

```text
🌎 Travel Master

Check-in 50 địa điểm.
```

---

## 13. Travel Passport

Mỗi tài khoản có một **Travility Passport**.

Ví dụ:

```text
CHÍ THÀNH

Cities visited
7

Places visited
36

Total distance
2,840 km

Badges
12
```

Danh sách điểm đến:

```text
✓ Đà Nẵng
✓ Đà Lạt
✓ Nha Trang
✓ Huế
```

Travel Passport có thể dùng để tổng hợp:

- Số thành phố đã đến.
- Số địa điểm đã check-in.
- Tổng quãng đường.
- Tổng số chuyến đi.
- Số huy hiệu.
- Thành tích đặc biệt.
- Lịch sử du lịch.

---

## 14. Module F – Top-K Recommendation Engine

Travility cần có hệ thống đề xuất khách sạn, nhà hàng, quán ăn và địa điểm tham quan.

Ví dụ:

> Tìm 5 khách sạn phù hợp nhất cho chuyến đi hiện tại.

Kết quả:

```text
TOP 5 HOTELS

1. Hotel A
Score: 92

2. Hotel B
Score: 89

3. Hotel C
Score: 86

4. Hotel D
Score: 84

5. Hotel E
Score: 81
```

---

## 15. Nguyên tắc xếp hạng

Không nên xếp hạng chỉ dựa trên rating.

Ví dụ:

```text
Hotel A

5.0 ⭐
4 reviews
```

không nên mặc định đứng trên:

```text
Hotel B

4.8 ⭐
8,200 reviews
```

Hệ thống cần xem xét thêm:

- Rating.
- Số lượng review.
- Độ tin cậy của review.
- Giá.
- Khoảng cách.
- Ngân sách hiện tại.
- Sở thích người dùng.
- Vị trí so với lịch trình.
- Tiện ích.
- Khả năng phù hợp với thời gian chuyến đi.

---

## 16. Recommendation Score

Một mô hình ban đầu:

```text
RecommendationScore =

w1 × RatingScore
+ w2 × ReviewConfidence
+ w3 × PreferenceScore
+ w4 × LocationScore
+ w5 × PriceScore
+ w6 × ScheduleCompatibility
```

Trong đó:

```text
w1 + w2 + w3 + w4 + w5 + w6 = 1
```

Ví dụ trọng số:

```text
Rating             30%
Review confidence  15%
Preference         20%
Location           15%
Price              15%
Schedule            5%
```

Sau đó lấy **Top-K** địa điểm có điểm cao nhất.

Ví dụ bằng LINQ:

```csharp
var topK = places
    .OrderByDescending(p => p.RecommendationScore)
    .Take(k)
    .ToList();
```

---

## 17. Review Confidence

Có thể sử dụng **Bayesian Rating** để tránh trường hợp địa điểm có quá ít review nhưng rating rất cao.

```text
WeightedRating =
(v / (v + m)) × R
+
(m / (v + m)) × C
```

Trong đó:

- `R`: Rating của địa điểm.
- `v`: Số lượng review của địa điểm.
- `C`: Rating trung bình của toàn bộ tập địa điểm.
- `m`: Số review tối thiểu để được xem là đủ độ tin cậy.

---

## 18. Comment Analysis – Chức năng nâng cao

Nếu có nguồn dữ liệu review hợp lệ từ API, Travility có thể phân tích nhiều khía cạnh.

Ví dụ:

```text
Cleanliness      4.8
Location         4.7
Service          4.5
Value for money  4.3
Food             4.6
```

Khi đó:

- Người dùng ưu tiên tiết kiệm có thể được xếp hạng theo `Value for money`.
- Người dùng ưu tiên nghỉ dưỡng có thể được xếp hạng theo `Service` và `Cleanliness`.
- Người dùng ưu tiên vị trí thuận tiện có thể được xếp hạng cao ở `Location`.

Trong thiết kế hệ thống, dữ liệu từ bên ngoài nên được truy cập qua **Provider/Adapter**, không gắn trực tiếp Recommendation Engine với một nền tảng cụ thể.

---

## 19. Module G – Smart Routing Engine

Routing Engine xác định thứ tự di chuyển hợp lý giữa khách sạn, nhà hàng, địa điểm tham quan và các điểm dịch vụ.

Giả sử người dùng chọn:

```text
Hotel A
Restaurant B
Museum C
Beach D
Coffee Shop E
```

Một thứ tự ngẫu nhiên có thể là:

```text
Hotel
 ↓
Museum
 ↓
Restaurant
 ↓
Beach
 ↓
Coffee Shop
```

Nhưng chưa chắc tối ưu.

Một tuyến tốt hơn có thể là:

```text
Hotel
 ↓ 1.2 km
Restaurant
 ↓ 0.8 km
Museum
 ↓ 2.1 km
Coffee Shop
 ↓ 1.0 km
Beach
```

---

## 20. Route Cost

Routing Engine không chỉ xét khoảng cách.

```text
RouteCost =
α × Distance
+ β × TravelTime
+ γ × TransportationCost
```

Ngoài ra phải thỏa các ràng buộc:

```text
OpeningHourConstraint
MealTimeConstraint
BookingConstraint
UserTimeConstraint
```

Có thể bổ sung:

- Phương tiện được phép.
- Thời gian nghỉ.
- Thời gian tham quan tối thiểu.
- Giờ nhận/trả phòng.
- Giờ khởi hành phương tiện.
- Thời gian quay lại khách sạn.

---

## 21. Distance Matrix

Cho `N` địa điểm:

```text
      A    B    C    D

A     0    3    6    5
B     3    0    2    4
C     6    2    0    3
D     5    4    3    0
```

Travility sử dụng ma trận khoảng cách hoặc ma trận thời gian để tìm thứ tự di chuyển tốt.

---

## 22. Route Optimization

Bài toán có nét tương tự **Traveling Salesman Problem – TSP**, nhưng Travility phức tạp hơn vì có:

- Time Window.
- Budget.
- User Preference.
- Opening Hours.
- Transportation Mode.
- Meal Times.
- Hotel.
- Booking Times.

Do đó có thể mô hình hóa gần với:

- **TSP with Time Windows**.
- **Orienteering Problem with Time Windows**.

Trong phiên bản đồ án, có thể bắt đầu bằng heuristic đơn giản, sau đó nâng cấp thuật toán nếu còn thời gian.

---

## 23. Module H – Intelligent Itinerary Planner

Travility không nên chỉ trả về một lịch trình duy nhất.

Hệ thống nên sinh nhiều phương án để người dùng so sánh.

Ví dụ:

```text
────────────────────

OPTION A

CHEAPEST

4,650,000 VNĐ

Travel:
62 km

Experience:
78 / 100

────────────────────

OPTION B

BALANCED ⭐ RECOMMENDED

5,200,000 VNĐ

Travel:
49 km

Experience:
91 / 100

────────────────────

OPTION C

BEST EXPERIENCE

5,800,000 VNĐ

Travel:
58 km

Experience:
96 / 100

────────────────────
```

---

## 24. Multi-objective Optimization

Travility cần tối ưu đồng thời nhiều mục tiêu.

### 24.1. Mục tiêu cần tối thiểu hóa

```text
Minimize:

TotalCost
TravelDistance
TravelTime
```

### 24.2. Mục tiêu cần tối đa hóa

```text
Maximize:

ExperienceScore
PreferenceScore
PlaceQuality
NumberOfDesiredPlaces
```

### 24.3. Các ràng buộc

```text
TotalCost ≤ Budget
```

```text
ActivityTime ≤ AvailableTime
```

```text
VisitTime ∈ OpeningHours
```

Ngoài ra có thể có:

- Booking phải đúng giờ.
- Không được xếp hai hoạt động trùng nhau.
- Mỗi hoạt động phải có đủ thời gian tối thiểu.
- Thời gian di chuyển phải được tính giữa hai hoạt động.
- Thời gian ăn uống phải phù hợp.
- Điểm bắt đầu hoặc kết thúc ngày có thể là khách sạn.

---

## 25. Objective Function

Một phiên bản cơ bản:

```text
Score =

wExperience × Experience
+ wPreference × Preference
+ wRating × Rating
- wCost × Cost
- wDistance × Distance
- wTime × TravelTime
```

Các trọng số thay đổi tùy theo **Travel Style** của người dùng.

---

## 26. Ba Travel Style

### 26.1. Budget

Ưu tiên tiết kiệm.

```text
Cost          45%
Distance      20%
Experience    15%
Preference    10%
Rating        10%
```

### 26.2. Balanced

Cân bằng chi phí và trải nghiệm.

```text
Cost          25%
Distance      15%
Experience    25%
Preference    20%
Rating        15%
```

### 26.3. Experience

Ưu tiên chất lượng trải nghiệm.

```text
Cost          10%
Distance      10%
Experience    35%
Preference    25%
Rating        20%
```

Cùng một tập dữ liệu nhưng mỗi Travel Style có thể tạo ra lịch trình khác nhau.

---

## 27. Pareto Optimal Solutions

Không nên xem một lịch trình là phương án tối ưu tuyệt đối trong mọi tiêu chí.

Hệ thống nên sinh một tập phương án, ví dụ:

```text
A = Rẻ nhất
B = Cân bằng nhất
C = Trải nghiệm tốt nhất
```

Một phương án bị xem là kém nếu tồn tại phương án khác tốt hơn trên các tiêu chí quan trọng mà không phải đánh đổi thêm.

Travility có thể sử dụng khái niệm **Pareto Front** để giữ lại các phương án đáng cân nhắc.

---

## 28. Module I – AI Travel Assistant

Travility có chatbot cho phép người dùng tương tác bằng ngôn ngữ tự nhiên.

Ví dụ:

```text
User:

Tôi có 5 triệu, muốn đi Đà Nẵng
3 ngày và thích biển với đồ ăn.
```

Hệ thống có thể hiểu thành:

```text
Destination = Da Nang
Duration = 3 days
Budget = 5,000,000

Preference:
Beach = High
Food = High
```

Sau đó AI Assistant gọi các tool nội bộ để thao tác với hệ thống.

---

## 29. Chatbot Tool System

Chatbot không nên trực tiếp truy cập database.

Chatbot sử dụng các tool có chức năng rõ ràng.

Ví dụ:

```text
search_places()

find_hotels()

find_restaurants()

find_transport()

create_trip()

update_trip()

get_trip_budget()

allocate_budget()

add_place_to_trip()

remove_place_from_trip()

optimize_route()

generate_itinerary()

compare_itineraries()

record_expense()

check_in()

get_weather()
```

---

## 30. Ví dụ Tool Calling

Người dùng:

```text
Tôi còn 1 triệu.
Tìm quán hải sản gần khách sạn tối nay
dưới 300k/người.
```

Chatbot thực hiện:

```text
get_current_trip()

↓

get_trip_budget()

↓

get_current_hotel()

↓

find_restaurants(
    category = "seafood",
    maxPrice = 300000,
    near = currentHotel
)

↓

rank_restaurants()

↓

return Top 5
```

---

## 31. Chatbot có thể thao tác hệ thống

Ví dụ người dùng nói:

```text
Thêm quán thứ 2 vào lịch trình tối nay.
```

AI Assistant gọi:

```text
add_place_to_itinerary()
```

Sau đó:

```text
optimize_route()
```

Budget Engine tự động cập nhật lại chi phí.

Điểm quan trọng:

> Chatbot phải là một **giao diện điều khiển hệ thống bằng ngôn ngữ tự nhiên**, không chỉ là chatbot hỏi đáp.

---

## 32. Event-driven System

Các module Travility phải liên kết với nhau thông qua event để hệ thống tự cập nhật.

Ví dụ:

```text
User adds Hotel
       │
       ▼
HotelAdded Event
       │
       ├──────────► Budget Engine
       │             recalculates
       │
       ├──────────► Routing Engine
       │             recalculates
       │
       └──────────► Scheduler
                     regenerates plans
```

Ví dụ khác:

```text
User removes Restaurant

        ↓

Budget updated

        ↓

Route updated

        ↓

Available time updated

        ↓

Recommendation Engine

        ↓

Suggest replacement
```

Thiết kế này cũng giúp đồ án vận dụng kiến thức **Delegate và Event** trong C# một cách thực tế.

---

## 33. User Flow tổng thể

```text
REGISTER / LOGIN
       │
       ▼
TRAVEL DASHBOARD
       │
       ▼
CHỌN ĐIỂM ĐẾN
       │
       ▼
NHẬP NGÀY ĐI / NGÀY VỀ
       │
       ▼
NHẬP NGÂN SÁCH
       │
       ▼
CHỌN SỞ THÍCH
       │
       ▼
CHỌN PHƯƠNG TIỆN
       │
       ▼
CHỌN KHÁCH SẠN
       │
       ▼
TOP-K RECOMMENDATIONS
       │
       ▼
CHỌN ĐỊA ĐIỂM
       │
       ▼
ROUTE OPTIMIZATION
       │
       ▼
ITINERARY OPTIMIZATION
       │
       ▼
NHIỀU PHƯƠNG ÁN
       │
 ┌─────┼─────┐
 ▼     ▼     ▼
Cheap Balance Experience
       │
       ▼
CONFIRM TRIP
       │
       ▼
TRAVEL MODE
       │
       ├── Map
       ├── Budget
       ├── Navigation
       ├── Check-in
       └── AI Assistant
```

---

## 34. Database – Nhóm User

```text
Users
Roles
UserProfiles
UserPreferences
TravelWallets
```

### 34.1. Gợi ý vai trò

```text
Admin
Traveler
```

---

## 35. Database – Nhóm Places

```text
Places
PlaceCategories
PlaceTypes
PlaceImages
PlaceOpeningHours
PlaceRatings
PlaceReviews
ExternalPlaceMappings
```

### 35.1. ExternalPlaceMappings

Bảng này dùng để ánh xạ ID nội bộ của Travility với ID của các nhà cung cấp bên ngoài.

Ví dụ:

```text
TravilityPlaceId
ProviderName
ExternalPlaceId
LastSyncedAt
```

Có thể hỗ trợ:

```text
Google Places
GrabMaps
Other Provider
```

Thiết kế này giúp Recommendation Engine không phụ thuộc cứng vào một nguồn dữ liệu duy nhất.

---

## 36. Database – Nhóm Travel

```text
Trips
TripMembers
TripPreferences
TransportOptions
Bookings
TripPlaces
```

---

## 37. Database – Nhóm Lịch trình

```text
Itineraries
ItineraryDays
ItineraryItems
RoutePlans
RouteSegments
```

Một Trip có thể có nhiều phương án:

```text
Trip
 ├── Itinerary A
 ├── Itinerary B
 └── Itinerary C
```

Sau đó người dùng chọn phương án chính thức.

---

## 38. Database – Nhóm Finance

```text
TravelWallets
TripBudgets
BudgetCategories
BudgetAllocations
Expenses
```

Các loại chi phí có thể gồm:

```text
Transportation
Accommodation
Food
Activities
LocalTransport
Shopping
Emergency
Other
```

---

## 39. Database – Nhóm Gamification

```text
CheckIns
Achievements
UserAchievements
TravelPassports
```

---

## 40. Database – Nhóm AI

```text
ChatSessions
ChatMessages
ToolExecutions
```

### 40.1. ToolExecutions

```text
ToolExecutionId
ChatSessionId
ToolName
Arguments
Result
ExecutedAt
Status
```

Bảng này hỗ trợ:

- Debug.
- Audit.
- Theo dõi tool AI đã gọi.
- Phân tích lỗi.
- Kiểm tra hành động AI.
- Khôi phục lịch sử thao tác.

---

## 41. System Architecture

```text
┌───────────────────────────────────────┐
│               WINFORMS                │
│                                       │
│ Dashboard                             │
│ Smart Map                             │
│ Trip Builder                          │
│ Budget                                │
│ Itinerary                             │
│ Chatbot                               │
└──────────────────┬────────────────────┘
                   │
                   ▼
┌───────────────────────────────────────┐
│            BUSINESS LAYER             │
│                                       │
│ TripService                           │
│ BookingService                        │
│ BudgetService                         │
│ CheckInService                        │
│ AchievementService                    │
└──────────────────┬────────────────────┘
                   │
       ┌───────────┼──────────────┐
       ▼           ▼              ▼
┌────────────┐ ┌────────────┐ ┌──────────────┐
│Recommend.  │ │ Routing    │ │ Scheduling   │
│Engine      │ │ Engine     │ │ Engine       │
└────────────┘ └────────────┘ └──────────────┘
       │           │              │
       └───────────┼──────────────┘
                   ▼
          ┌─────────────────┐
          │Optimization     │
          │Engine           │
          └────────┬────────┘
                   │
          ┌────────┴─────────┐
          ▼                  ▼
┌─────────────────┐  ┌──────────────────┐
│ SQL SERVER      │  │ EXTERNAL APIs    │
│                 │  │                  │
│ Entity Framework│  │ Maps             │
│ LINQ            │  │ Places           │
│ ADO.NET         │  │ Routes           │
└─────────────────┘  │ Transport        │
                     │ AI               │
                     └──────────────────┘
```

---

## 42. Map Architecture trên WinForms

Đề xuất sử dụng:

```text
WinForms
    ↓
WebView2
    ↓
Interactive Map
    ↓
JavaScript Bridge
    ↕ JSON
C# Business Layer
```

### 42.1. C# gửi dữ liệu xuống bản đồ

```text
C#

ShowPlaceOnMap(place)

      ↓

WebView2

      ↓

JavaScript

addMarker(
    latitude,
    longitude,
    category
)
```

### 42.2. Bản đồ gửi sự kiện về C#

```text
User clicks marker

      ↓

JavaScript

      ↓

postMessage(placeId)

      ↓

C#

      ↓

Open PlaceDetailForm
```

---

## 43. Các Form chính

### 43.1. Người dùng

```text
LoginForm
RegisterForm
MainForm
DashboardForm
ExploreMapForm
PlaceDetailForm
TripWizardForm
TransportSearchForm
BookingForm
HotelRecommendationForm
RestaurantRecommendationForm
BudgetForm
ItineraryForm
ItineraryComparisonForm
CheckInForm
TravelPassportForm
AchievementForm
AITravelAssistantForm
ProfileForm
```

### 43.2. Quản trị viên

```text
AdminDashboardForm
PlaceManagementForm
UserManagementForm
TransportManagementForm
AchievementManagementForm
```

---

## 44. Ba Engine cốt lõi

Travility nên tách rõ ba bài toán.

### 44.1. Recommendation Engine

Trả lời:

> Nên chọn khách sạn, quán ăn hoặc địa điểm nào?

Đầu vào:

- Sở thích.
- Rating.
- Số review.
- Giá.
- Khoảng cách.
- Ngân sách.
- Lịch trình.

Đầu ra:

- Top-K địa điểm phù hợp.

### 44.2. Routing Engine

Trả lời:

> Nên đi theo thứ tự nào để giảm quãng đường, thời gian và chi phí di chuyển?

Đầu vào:

- Danh sách điểm.
- Distance Matrix.
- Travel Time Matrix.
- Phương tiện.
- Time Window.

Đầu ra:

- Thứ tự ghé thăm.
- Tổng quãng đường.
- Tổng thời gian.
- Tổng chi phí di chuyển.

### 44.3. Itinerary Optimization Engine

Trả lời:

> Với toàn bộ chuyến đi, nên chọn những địa điểm nào, xếp vào ngày và giờ nào để đạt sự cân bằng tốt nhất giữa ngân sách và trải nghiệm?

Đầu vào:

- Budget.
- Ngày đi/ngày về.
- Sở thích.
- Điểm đã chọn.
- Khách sạn.
- Booking.
- Route cost.
- Opening hours.

Đầu ra:

- Nhiều lịch trình tối ưu hoặc gần tối ưu.

---

## 45. Phạm vi đồ án nên triển khai

Không nên cố biến đồ án môn học thành một hệ thống thương mại hoàn chỉnh ngay từ đầu.

### 45.1. Core – Nên hoàn thiện

```text
Smart Map
Map Layers
Trip Creation
Budget Management
POI Database
Hotel Recommendation
Restaurant Recommendation
Top-K Ranking
Route Optimization
Itinerary Generation
Multiple Plans
Check-in
Achievements
SQL Server
LINQ
Entity Framework
```

### 45.2. Prototype nâng cao

```text
Chatbot Tool Calling
External Places API
Route API
Review Analysis
Transport Provider Adapter
```

### 45.3. Extension

```text
Đặt vé thật
Thanh toán thật
Real-time airline pricing
Real-time hotel booking
Real-time traffic
Full AI personalization
```

---

## 46. Phát biểu bài toán chính thức

> **Travility** là hệ thống hỗ trợ lập kế hoạch du lịch thông minh trên nền tảng Windows, cho phép người dùng khám phá địa điểm thông qua bản đồ đa lớp, tạo và quản lý chuyến đi, tìm kiếm phương tiện và dịch vụ du lịch, quản lý ngân sách, nhận đề xuất địa điểm dựa trên sở thích và đánh giá cộng đồng, tối ưu tuyến đường và xây dựng nhiều lịch trình dựa trên sự cân bằng giữa chi phí và trải nghiệm.

> Hệ thống còn hỗ trợ check-in, gamification và trợ lý AI có khả năng tương tác với các chức năng của hệ thống thông qua ngôn ngữ tự nhiên.

> Bài toán trọng tâm của Travility là **bài toán tối ưu đa mục tiêu** nhằm tối thiểu hóa chi phí, quãng đường và thời gian di chuyển, đồng thời tối đa hóa mức độ phù hợp, chất lượng địa điểm và trải nghiệm của người dùng trong các ràng buộc về ngân sách, thời gian và lịch hoạt động của các địa điểm.

---

## 47. Điểm khác biệt cốt lõi

Một ứng dụng du lịch thông thường:

```text
Search
  ↓
Book
  ↓
Done
```

Travility hướng tới:

```text
UNDERSTAND USER
        ↓
SEARCH
        ↓
RECOMMEND
        ↓
OPTIMIZE
        ↓
COMPARE OPTIONS
        ↓
PLAN
        ↓
TRAVEL
        ↓
ADAPT
        ↓
CHECK-IN
        ↓
LEARN USER PREFERENCE
```

Đặc trưng quan trọng nhất của Travility là khả năng **hiểu nhu cầu → đề xuất → tối ưu → so sánh → thích ứng** thay vì chỉ cung cấp chức năng tìm kiếm và lưu chuyến đi.

---

## 48. Công nghệ dự kiến

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ | C# |
| Platform | .NET Framework |
| Desktop UI | Windows Forms |
| IDE | Visual Studio |
| Database | Microsoft SQL Server |
| ORM | Entity Framework |
| Database Access | ADO.NET |
| Query | LINQ |
| Map Container | WebView2 |
| Map/Place Integration | Provider Adapter |
| Architecture | GUI – BUS – DAL – Service/Engine |
| Version Control | Git |

---

## 49. Nguyên tắc thiết kế kỹ thuật

### 49.1. Không phụ thuộc cứng vào một API

Các nguồn bản đồ, địa điểm và review phải đi qua interface hoặc adapter.

Ví dụ:

```csharp
public interface IPlaceProvider
{
    IEnumerable<PlaceDto> SearchPlaces(string keyword);
    PlaceDetailDto GetPlaceDetail(string externalId);
}
```

Sau này có thể triển khai:

```text
GooglePlaceProvider
GrabMapsProvider
LocalDatabasePlaceProvider
```

### 49.2. Không gộp toàn bộ tối ưu vào một hàm

Không nên thiết kế:

```text
OptimizeTrip()
```

để xử lý mọi thứ.

Nên tách thành:

```text
RecommendationEngine
RoutingEngine
ItineraryOptimizationEngine
BudgetEngine
```

### 49.3. AI không được truy cập database trực tiếp

Luồng đúng:

```text
User
 ↓
AI Assistant
 ↓
Tool
 ↓
Business Service
 ↓
Repository / DAL
 ↓
SQL Server
```

### 49.4. Event phải được sử dụng cho thay đổi quan trọng

Ví dụ:

```text
HotelChanged
PlaceAdded
PlaceRemoved
BookingConfirmed
BudgetChanged
ExpenseRecorded
CheckInCompleted
```

Các event giúp những module liên quan tự cập nhật lại.

---

## 50. Hướng phát triển tài liệu tiếp theo

Sau tài liệu tổng quan này, tài liệu phân tích và thiết kế nên được xây dựng theo thứ tự:

1. Actor và quyền hạn.
2. Danh sách Use Case.
3. Use Case Diagram.
4. Đặc tả chi tiết từng Use Case.
5. Business Rules.
6. Activity Diagram.
7. Sequence Diagram cho nghiệp vụ quan trọng.
8. ERD.
9. Data Dictionary.
10. Thiết kế SQL Server.
11. Thiết kế lớp C#.
12. Kiến trúc GUI – BUS – DAL.
13. Đặc tả Recommendation Engine.
14. Đặc tả Routing Engine.
15. Đặc tả Itinerary Optimization Engine.
16. Thiết kế giao diện WinForms.
17. Test Case.
18. Kế hoạch triển khai theo Sprint.

---

## 51. Kết luận

Travility được định hướng là một **hệ thống hỗ trợ quyết định du lịch thông minh**, không chỉ là phần mềm quản lý tour hoặc phần mềm CRUD.

Ba thành phần tạo nên giá trị cốt lõi của hệ thống là:

1. **Recommendation Engine** – chọn những dịch vụ và địa điểm phù hợp.
2. **Routing Engine** – tìm thứ tự di chuyển hợp lý.
3. **Itinerary Optimization Engine** – tạo nhiều phương án lịch trình cân bằng giữa chi phí và trải nghiệm.

Khi kết hợp với Smart Map, Budget Management, Check-in, Gamification và AI Travel Assistant, Travility có thể trở thành một đồ án có chiều sâu nghiệp vụ, thể hiện rõ các nội dung của học phần **Lập trình trên môi trường Windows** và vẫn có hướng mở rộng thành hệ thống thực tế trong tương lai.
