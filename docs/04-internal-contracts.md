# 04 — Hợp đồng nội bộ

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md) · [`02-system-architecture.md`](./02-system-architecture.md)
> **Đây là file chống trôi lệch giữa bốn người. Đọc trước khi viết dòng nghiệp vụ đầu tiên.**

---

## Vì sao file này tồn tại

Nhóm chọn merge tự do, không theo lịch. Rủi ro không phải conflict — mà là **trôi
giao diện**: C viết form dựa trên giả định `ITripService` trả về kiểu A, D viết
engine trả về kiểu B. Cả hai chạy tốt trên máy mình, đến tuần 5 ghép lại mới vỡ.

Biện pháp phòng vệ nằm ở đây, không ở quy trình:

> **Ngày 2–3 tuần 1: chốt toàn bộ chữ ký dưới đây, merge một lần duy nhất.**
> Thân hàm để `throw new NotImplementedException()`.
> Sau đó **không ai sửa chữ ký nếu chưa báo cả nhóm**.

Có ba hợp đồng: interface C#, cầu JSON WebView2, và schema tool của chatbot.

---

## 1. Interface trong `Travility.Core`

### Services

```csharp
public interface IAuthenticationService
{
    AuthenticationResult Login(string identifier, string password);
    RegistrationResult   Register(RegistrationRequest request);
    PasswordResetResult  SetTemporaryPassword(int actorUserId, int targetUserId, string temporaryPassword);
    ChangePasswordResult ChangePassword(int userId, string currentPassword, string newPassword);
}

public interface IAppLogger
{
    void Info(string component, string message);
    void Warning(string component, string message);
    void Error(string component, string errorId, Exception exception);
}

public interface ITripService
{
    Trip          Create(TripDraft draft);
    Trip          GetById(int tripId);
    IList<Trip>   GetByUser(int userId);
    void          AddPlace(int tripId, int placeId);
    void          RemovePlace(int tripId, int placeId);
    void          Update(Trip trip);
}

public interface IBudgetService
{
    decimal       GetPlannedCost(int tripId);
    decimal       GetActualSpent(int tripId);
    decimal       GetWalletBalance(int userId);
    IList<BudgetAllocation> Allocate(int tripId, IDictionary<BudgetCategory, decimal> ratios);
    void          RecordExpense(Expense expense);
    bool          ExceedsWallet(int userId, decimal additionalBudget);  // cảnh báo, không chặn
}

public interface IPlaceService
{
    IList<Place>  Search(PlaceQuery query);
    Place         GetById(int placeId);
    IList<Place>  GetByCategory(int categoryId);
    IList<Place>  GetForMapLayers(IEnumerable<int> categoryIds);
}

public interface IBookingService
{
    Booking       Create(BookingDraft draft);
    IList<Booking> GetByTrip(int tripId);
    void          UpdateStatus(int bookingId, BookingStatus status, int actorUserId);
}

public interface ICheckInService
{
    CheckInResult CheckIn(CheckInRequest request);
    IList<CheckIn> GetByTrip(int tripId);
}

public interface IAchievementService
{
    IList<Achievement>     EvaluateForCheckIn(int checkInId);
    IList<UserAchievement> GetByUser(int userId);
}
```

### Engines

```csharp
public interface IRecommendationEngine
{
    IList<ScoredPlace> TopK(RecommendationContext ctx, int k);
    decimal            Score(Place place, RecommendationContext ctx);
}

public interface IRoutingEngine
{
    RoutePlan     Optimise(IList<Place> places, RoutingOptions options);
    double        Distance(Place a, Place b);           // Haversine × 1.3
    double[,]     BuildMatrix(IList<Place> places);
}

public interface IItineraryEngine
{
    IList<Itinerary> Generate(Trip trip);               // trả về 3 phương án
    Itinerary        GenerateFor(Trip trip, TravelStyle style);
}
```

### Nguồn bên ngoài

```csharp
public interface ILocationProvider
{
    Coordinate    GetCurrentLocation();
    double        AccuracyMeters { get; }
    string        ProviderName   { get; }   // "Windows" | "Simulated"
}

public interface IChatProvider
{
    ChatResponse  Send(IList<ChatMessage> history, IList<ToolDefinition> tools);
    string        ProviderName { get; }     // "Gemini" | "OpenAI"
}
```

### DTO dùng chung

```csharp
public class Coordinate       { public double Latitude; public double Longitude; }
public class ScoredPlace      { public Place Place; public decimal Score; }
public enum  TravelStyle      { Budget, Balanced, Experience }
public enum  PricingUnit      { PerPerson, PerRoom, PerTrip }
public enum  BookingStatus    { Pending, Confirmed, Cancelled, Completed }

public class TripDraft        { public int UserId; public string Title; public string Destination;
                                public DateTime DepartDate; public DateTime ReturnDate;
                                public int PeopleCount; public decimal InitialBudget; }
public class PlaceQuery       { public string Keyword; public int? CategoryId;
                                public decimal? MaxPrice; public Coordinate Near;
                                public double? RadiusKm; }
public class RecommendationContext { public int TripId; public int UserId;
                                     public Coordinate AnchorLocation;
                                     public decimal RemainingBudget;
                                     public IList<int> ExcludedPlaceIds; }
public class RoutingOptions   { public bool PreferWalking; public double MaxDetourKm; }
public class BookingDraft     { public int TripId; public int? PlaceId; public string ServiceType;
                                public string BookingReference; public decimal TotalCost;
                                public string Currency; public DateTime? CheckInDate;
                                public DateTime? CheckOutDate; public string Notes; }
public class CheckInRequest   { public int TripId; public int PlaceId; public int UserId;
                                public Coordinate Location; public DateTime CheckInTime;
                                public string Notes; }
public class CheckInResult    { public bool Succeeded; public int? CheckInId; public string Message;
                                public double DistanceMeters; public bool IsWithinRange;
                                public IList<int> UnlockedAchievementIds; }
public class ToolDefinition   { public string Name; public string Description;
                                public string ParametersJson; }
public class ChatToolCall     { public string ToolName; public string ArgumentsJson; }
public class ChatResponse     { public string Content; public IList<ChatToolCall> ToolCalls;
                                public bool HasToolCalls; }
```

### Event baseline (8 Events)

```csharp
// PlaceAdded · PlaceRemoved · HotelChanged · BookingConfirmed
// BudgetChanged · ExpenseRecorded · CheckInCompleted · ItineraryGenerated
public class PlaceAddedEventArgs         : EventArgs { public int TripId; public int PlaceId; public int? DayNumber; }
public class PlaceRemovedEventArgs       : EventArgs { public int TripId; public int PlaceId; public int? DayNumber; }
public class HotelChangedEventArgs       : EventArgs { public int TripId; public int? OldHotelPlaceId; public int NewHotelPlaceId; }
public class BookingConfirmedEventArgs   : EventArgs { public int BookingId; public int TripId; public decimal TotalCost; }
public class BudgetChangedEventArgs      : EventArgs { public int TripId; public decimal NewTotalBudget; }
public class ExpenseRecordedEventArgs    : EventArgs { public int ExpenseId; public int TripId; public int CategoryId; public decimal Amount; }
public class CheckInCompletedEventArgs   : EventArgs { public int CheckInId; public int TripId; public int PlaceId; public int UserId; public IList<int> UnlockedAchievementIds; }
public class ItineraryGeneratedEventArgs : EventArgs { public int TripId; public int ItineraryId; }
```

> Mọi kiểu tiền tệ là `decimal`. Không bao giờ `float` hay `double`.


---

## 2. Cầu JSON WebView2 ↔ C#

Đây là hợp đồng giữa B (JavaScript) và phần còn lại (C#). Chốt ở tuần 1, coi như
một interface — đổi giữa chừng sẽ vỡ cả hai đầu.

```
C# → JS:   webView.CoreWebView2.PostWebMessageAsJson(json)
JS → C#:   webView.CoreWebView2.WebMessageReceived += OnMessage
```

### C# gửi xuống bản đồ

```jsonc
{ "action": "addMarkers",  "places": [ { "id": 1, "lat": 16.05, "lng": 108.24,
                                         "category": "hotel", "name": "..." } ] }
{ "action": "clearLayer",  "category": "hotel" }
{ "action": "drawRoute",   "day": 1, "colour": "#e63946",
                           "points": [ [16.05, 108.24], [16.07, 108.22] ] }
{ "action": "clearRoutes" }
{ "action": "setUserPin",  "lat": 16.05, "lng": 108.24 }
{ "action": "focusPlace",  "id": 1 }
```

### Bản đồ gửi lên C#

```jsonc
{ "event": "markerClick", "placeId": 1 }
{ "event": "mapClick",    "lat": 16.05, "lng": 108.24 }   // dùng cho check-in giả lập
{ "event": "mapReady" }
```

`mapClick` phục vụ `SimulatedLocationProvider` — làm sẵn từ đầu, rẻ hơn thêm sau.

---

## 3. Schema 7 tool của chatbot

Hợp đồng giữa nhà cung cấp LLM và tầng `Core`. Rút từ 16 tool trong đặc tả gốc
xuống 7 — đủ chứng minh chatbot điều khiển được hệ thống.

```
find_places(category: string, maxPrice: number, near: string)
get_trip_budget()
add_place_to_itinerary(placeId: int, day: int)
remove_place_from_itinerary(itemId: int)
optimize_route(day: int)
generate_itinerary(style: "Budget" | "Balanced" | "Experience")
record_expense(category: string, amount: number)
```

Mỗi tool là **một lời gọi mỏng** tới service đã có ở mục 1 — không viết lại
nghiệp vụ trong tầng tool.

Luồng bắt buộc: `AI → Tool → Business Service → Repository → SQL Server`.
Chatbot **không** được truy cập CSDL trực tiếp.

---

## 4. Quy trình đổi hợp đồng

Nếu thật sự cần đổi một chữ ký:

1. Báo cả nhóm **trước khi** sửa.
2. Sửa file này trong cùng PR với thay đổi code.
3. Người bị ảnh hưởng xác nhận đã cập nhật.

Đổi hợp đồng mà không báo là cách chắc chắn nhất để mất một ngày của người khác.
