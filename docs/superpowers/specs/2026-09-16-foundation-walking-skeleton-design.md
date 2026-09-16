# Foundation & Walking Skeleton Design

**Ngày:** 2026-09-16  
**Trạng thái:** Đã duyệt  
**Phạm vi:** Nền móng tuần 1 cho Travility

## 1. Mục tiêu

Foundation tạo một baseline chung để bốn thành viên có thể phát triển song song
mà không trôi schema, entity hoặc interface. Kết quả cuối là một walking skeleton
đi xuyên từ WinForms tới SQL Server, kèm kiểm thử logic thuần và CI.

Walking skeleton phải chứng minh được các luồng sau:

```text
Đăng ký/Đăng nhập
    -> AuthenticationService
    -> UserRepository
    -> EF6 DbContext
    -> SQL Server Express

Đăng nhập thành công
    -> UserSession
    -> MainForm
    -> Navigation theo Role
```

Foundation cũng triển khai phép tính Haversine x 1.3 đầu tiên cùng NUnit test,
để xác nhận `Travility.Core` có thể chạy và kiểm thử mà không cần UI hoặc kết
nối CSDL.

## 2. Baseline kỹ thuật

| Thành phần | Quyết định |
|---|---|
| IDE | Visual Studio 2022 |
| Nền tảng | .NET Framework 4.8 |
| Ngôn ngữ | C# 7.3 |
| UI | Windows Forms kiểu project cổ điển |
| ORM | Entity Framework 6 Database First, EDMX |
| CSDL | SQL Server Express 2022, instance `.\\SQLEXPRESS` |
| Xác thực SQL | Windows Authentication |
| Package | `packages.config` |
| Test | NUnit 3 + NUnit3TestAdapter |
| DI | Constructor injection thủ công; không dùng DI container |

EF entities và repository nằm trong `Travility.Data`. `Travility.Core` sử dụng
trực tiếp các entity này. Đây là lựa chọn có chủ đích để tránh tạo một lớp domain
model thứ hai và gánh nặng mapping trong đồ án sáu tuần.

## 3. Phạm vi

### 3.1. Trong phạm vi

- Solution bốn project nhắm .NET Framework 4.8.
- Schema v1 đầy đủ, seed, stored procedure baseline và EDMX.
- Contract của Data/Core.
- Data session và `UserRepository`.
- Password hashing, đăng ký, đăng nhập và đổi mật khẩu.
- `UserSession`.
- `LoginForm`, `RegisterForm`, `ChangePasswordDialog`.
- `MainForm`, navigation theo role và `HomePage`.
- Logging và global exception handling.
- Haversine x 1.3.
- Unit test, database smoke test và Windows CI.

### 3.2. Ngoài phạm vi

- Map/WebView2 và CRUD `Place`.
- Trip Wizard, Booking, Budget và Expense.
- Recommendation/Itinerary implementation.
- Routing ngoài phép tính khoảng cách.
- Check-in, Achievement và Travel Passport.
- Chatbot.
- Admin CRUD thực tế ngoài việc phân quyền shell.

Contract cho các module trên được chốt ở Foundation, nhưng không tạo hàng loạt
implementation rỗng bằng `NotImplementedException`.

## 4. Hai cổng tích hợp

### 4.1. Cổng 1 — Foundation Baseline

Cổng 1 tạo một solution build sạch và là điểm xuất phát chung của cả nhóm:

1. Tạo solution và bốn project `net48`.
2. Viết schema v1 đầy đủ.
3. Chạy schema trên `.\\SQLEXPRESS`.
4. Sinh `TravilityModel.edmx` trong `Travility.Data`.
5. Viết data-session/repository contract trong Data.
6. Viết service/engine contract trong Core.
7. Thiết lập project reference và package baseline.
8. Build Debug và Release.
9. Chạy CI, review và merge baseline trước khi nhóm tách việc.

Schema và EDMX nằm ở Cổng 1 vì các public contract của Core dùng EF entities từ
Data. Cách này tránh tạo entity giả rồi xóa hoặc thay thế sau đó.

### 4.2. Cổng 2 — Walking Skeleton

Cổng 2 triển khai Auth xuyên suốt Data -> Core -> WinForms, tạo `UserSession`,
mở `MainForm` theo role, thêm logging/global error handling, rồi triển khai
Haversine cùng test đầu tiên.

## 5. Cấu trúc solution

```text
Travility.sln
├── src/
│   ├── Travility.Data/
│   │   ├── Model/
│   │   │   ├── TravilityModel.edmx
│   │   │   └── *.partial.cs
│   │   ├── Repositories/
│   │   └── StoredProcedures/
│   ├── Travility.Core/
│   │   ├── Contracts/
│   │   ├── Services/
│   │   ├── Engines/
│   │   ├── Events/
│   │   ├── Security/
│   │   └── Session/
│   └── Travility.WinForms/
│       ├── Auth/
│       ├── Shell/
│       ├── Pages/
│       ├── Dialogs/
│       ├── Infrastructure/
│       └── Properties/
├── tests/
│   └── Travility.Tests/
│       ├── Core/
│       ├── Engines/
│       └── TestData/
└── database/
    ├── schema.sql
    ├── seed_reference.sql
    ├── seed_demo.sql
    ├── procedures.sql
    ├── smoke_test.sql
    └── reset-dev.sql
```

### 5.1. Phụ thuộc

```text
WinForms --> Core --> Data
Tests --------> Core --> Data
```

Vì public contract của Core trả về EF entities thuộc Data, WinForms có thể cần
reference biên dịch tới Data. Reference này chỉ cho phép nhận kiểu dữ liệu:

- Form không tạo `TravilityEntities`.
- Form không gọi repository.
- Form không truy vấn `DbSet`.
- Mọi nghiệp vụ đi qua service/engine của Core.

`Program.cs` là composition root: tạo repository/data-session factory, service,
session và form bằng constructor injection thủ công.

## 6. Schema v1

### 6.1. Nhóm bảng

| Nhóm | Bảng |
|---|---|
| Identity | `Roles`, `Users`, `UserProfiles`, `UserPreferences`, `TravelWallets` |
| Places | `PlaceCategories`, `PlaceTypes`, `Places`, `PlaceImages`, `PlaceRatings`, `PlaceReviews`, `ExternalPlaceMappings` |
| Travel | `Trips`, `TripMembers`, `TripPreferences`, `TransportOptions`, `Bookings`, `TripPlaces` |
| Itinerary | `Itineraries`, `ItineraryDays`, `ItineraryItems`, `RoutePlans`, `RouteSegments`, `DistanceMatrix` |
| Finance | `BudgetCategories`, `TripBudgets`, `BudgetAllocations`, `Expenses` |
| Gamification | `CheckIns`, `Achievements`, `UserAchievements` |
| AI | `ChatSessions`, `ChatMessages`, `ToolExecutions` |

Không tạo bảng `TravelPassports`. Theo `CONTEXT.md`, `TravelPassport` là khung
nhìn tổng hợp bằng LINQ từ `Trips`, `CheckIns` và `UserAchievements`, không phải
dữ liệu lưu trùng.

### 6.2. Quy ước kiểu dữ liệu

- Khóa chính: `int IDENTITY(1,1)`.
- Tiền: `decimal(18,2)`.
- Rating và trọng số: `decimal` với precision theo phạm vi.
- Tọa độ: `float`.
- Chuỗi hiển thị: `nvarchar`.
- Ngày chuyến đi: `date`.
- Giờ mở/đóng cửa: `time(0)`.
- Thời điểm hệ thống: `datetime2(0)` lưu UTC.
- Enum ổn định: `tinyint`, chuyển đổi ở biên Data/Core.
- Bảng quan trọng có `CreatedAtUtc`; bảng chỉnh sửa có `UpdatedAtUtc`.

Ngày chuyến đi và giờ mở cửa giữ theo giờ địa phương của điểm đến. Thời điểm xảy
ra sự kiện như `CheckInTime` và `ExecutedAt` lưu UTC.

### 6.3. Identity

`Users` chứa tối thiểu:

```text
UserId
RoleId
Username
NormalizedUsername
Email
NormalizedEmail
PasswordHash
PasswordSalt
PasswordIterations
PasswordAlgorithm
MustChangePassword
IsActive
CreatedAtUtc
UpdatedAtUtc
```

Ràng buộc:

- Unique index trên `NormalizedUsername` và `NormalizedEmail`.
- Username không chứa `@`.
- Email bắt buộc.
- User tự đăng ký luôn nhận role `Traveler`.
- Chỉ Admin có thể tạo hoặc nâng quyền Admin.
- User bị vô hiệu hóa bằng `IsActive = 0`, không xóa thật.
- Người dùng đăng nhập được bằng username hoặc email. Identifier có `@` được
  xem là email; identifier còn lại được xem là username.

### 6.4. Xóa và tính toàn vẹn

Mặc định foreign key dùng `ON DELETE NO ACTION`.

- `Users`, `Places`, `Achievements`, `TransportOptions` dùng `IsActive`.
- `Expenses`, `CheckIns`, `Bookings` không bị cascade-delete.
- Dữ liệu con của draft chưa xác nhận có thể được service xóa tường minh trong
  transaction.
- Mọi thao tác xóa nghiệp vụ đi qua service.

Database thực thi các constraint cục bộ như rating, tọa độ, giá, thời lượng,
ngày đi/về và số người. Quy tắc cần dữ liệu nhiều bảng như
`PlannedCost <= Budget` thuộc service/engine, không dùng trigger.

Unique constraint `(UserId, AchievementId)` bảo vệ BR-10. `DistanceMatrix` lưu
mỗi cặp địa điểm một lần theo thứ tự chuẩn hóa.

### 6.5. Script và reset

`schema.sql`, seed và procedure là các file riêng. `reset-dev.sql` chỉ được phép
reset đúng database `TravilityDev` sau khi kiểm tra tên chính xác. Không xóa dựa
trên biến, wildcard hoặc database hiện tại.

Seed phải chạy lại được và gồm hai role, một Admin demo và một Traveler demo.
SQL chỉ chứa password hash và salt; plaintext của tài khoản demo chỉ nằm trong
hướng dẫn nội bộ.

### 6.6. Data dictionary tối thiểu

Bảng dưới đây chốt cột nghiệp vụ tối thiểu của schema v1. Mọi bảng có khóa chính
`<TênSốÍt>Id`; các cột audit tuân theo mục 6.2 nên không lặp lại ở từng dòng.

| Bảng | Cột nghiệp vụ tối thiểu |
|---|---|
| `Roles` | `Name`, `Description`, `IsActive` |
| `Users` | `RoleId`, `Username`, `NormalizedUsername`, `Email`, `NormalizedEmail`, `PasswordHash`, `PasswordSalt`, `PasswordIterations`, `PasswordAlgorithm`, `MustChangePassword`, `IsActive` |
| `UserProfiles` | `UserId` (unique), `DisplayName`, `Phone`, `AvatarPath` |
| `UserPreferences` | `UserId` (unique), bảy điểm `Beach`, `Food`, `History`, `Culture`, `Entertainment`, `Shopping`, `Nature` trong `[1,5]` |
| `TravelWallets` | `UserId` (unique), `Balance`, `UpdatedAtUtc` |
| `PlaceCategories` | `Name`, `IconKey`, `IsActive` |
| `PlaceTypes` | `CategoryId`, `Name`, `IsActive` |
| `Places` | `Name`, `Description`, `Address`, `Latitude`, `Longitude`, `CategoryId`, `PlaceTypeId`, `Rating`, `ReviewCount`, `TicketPrice`, `OpenTime`, `CloseTime`, `VisitDuration`, `PreferenceTags`, `IsMealPlace`, `PricingUnit`, `IsActive` |
| `PlaceImages` | `PlaceId`, `ImagePath`, `SortOrder` |
| `PlaceRatings` | `PlaceId` (unique), `AverageRating`, `ReviewCount`, `CalculatedAtUtc` |
| `PlaceReviews` | `PlaceId`, `UserId`, `Rating`, `Content`, `ReviewedAtUtc`, `IsActive` |
| `ExternalPlaceMappings` | `PlaceId`, `ProviderName`, `ExternalPlaceId`, `LastSyncedAtUtc`; unique `(ProviderName, ExternalPlaceId)` |
| `Trips` | `OwnerUserId`, `Origin`, `Destination`, `DepartDate`, `ReturnDate`, `PeopleCount`, `RoomCount`, `Status`, `IsActive` |
| `TripMembers` | `TripId`, `UserId`, `MemberRole`; unique `(TripId, UserId)`; giữ cho schema tương lai nhưng v1 chỉ có owner |
| `TripPreferences` | `TripId` (unique), bảy điểm sở thích trong `[1,5]` giống `UserPreferences` |
| `TransportOptions` | `BookingType`, `ProviderName`, `Origin`, `Destination`, `DepartAt`, `ArriveAt`, `UnitPrice`, `PricingUnit`, `AvailableQuantity`, `IsActive` |
| `Bookings` | `TripId`, `TransportOptionId` nullable, `PlaceId` nullable cho khách sạn, `BookingType`, `BookingStatus`, `Quantity`, `UnitPrice`, `TotalCost`, `PricingUnit`, `ReferenceCode`, `BookedAtUtc` |
| `TripPlaces` | `TripId`, `PlaceId`, `AddedByUserId`, `AddedAtUtc`, `PreferredDay` nullable; unique `(TripId, PlaceId)` |
| `Itineraries` | `TripId`, `TravelStyle`, `TotalCost`, `TotalDistanceKm`, `TotalTravelTimeMinutes`, `ExperienceScore`, `IsSelected` |
| `ItineraryDays` | `ItineraryId`, `DayNumber`, `Date`; unique `(ItineraryId, DayNumber)` |
| `ItineraryItems` | `ItineraryDayId`, `PlaceId`, `SequenceNumber`, `StartTime`, `EndTime`, `VisitDuration`, `PlannedCost`; unique `(ItineraryDayId, SequenceNumber)` |
| `RoutePlans` | `ItineraryDayId` (unique), `TotalDistanceKm`, `TotalTravelTimeMinutes`, `TotalTransportCost` |
| `RouteSegments` | `RoutePlanId`, `SequenceNumber`, `FromPlaceId`, `ToPlaceId`, `DistanceKm`, `TravelTimeMinutes`, `TransportCost`; unique `(RoutePlanId, SequenceNumber)` |
| `DistanceMatrix` | `FromPlaceId`, `ToPlaceId`, `DistanceKm`, `EstimatedTravelTimeMinutes`, `CalculatedAtUtc`; unique `(FromPlaceId, ToPlaceId)` với `FromPlaceId < ToPlaceId` |
| `BudgetCategories` | `Name`, `DisplayOrder`, `IsActive` |
| `TripBudgets` | `TripId` (unique), `Budget`, `PlannedCost`, `ActualSpent`, `UpdatedAtUtc` |
| `BudgetAllocations` | `TripBudgetId`, `BudgetCategoryId`, `Ratio`, `AllocatedAmount`; unique `(TripBudgetId, BudgetCategoryId)` |
| `Expenses` | `TripId`, `UserId`, `BudgetCategoryId`, `Amount`, `Description`, `SpentAtUtc` |
| `CheckIns` | `UserId`, `TripId`, `PlaceId`, `CheckInTimeUtc`, `Latitude`, `Longitude`, `DistanceFromPlace`, `IsSimulated` |
| `Achievements` | `Code`, `Name`, `Description`, `ConditionType`, `Threshold`, `CategoryId` nullable, `BadgeImagePath`, `IsActive` |
| `UserAchievements` | `UserId`, `AchievementId`, `AwardedAtUtc`; unique `(UserId, AchievementId)` |
| `ChatSessions` | `UserId`, `TripId` nullable, `ProviderName`, `StartedAtUtc`, `EndedAtUtc` nullable, `IsReplay` |
| `ChatMessages` | `ChatSessionId`, `Role`, `Content`, `CreatedAtUtc` |
| `ToolExecutions` | `ChatSessionId`, `ToolName`, `Arguments`, `Result`, `ExecutedAtUtc`, `Status`, `ErrorMessage` nullable |

`TripBudgets` là nguồn sự thật cho `Budget`, `PlannedCost` và `ActualSpent`; ba
cột này không lặp lại trong `Trips`. `TravelWallets.Balance` chỉ thay đổi trong
cùng transaction ghi `Expense` theo BR-04.

`Places.Rating` và `Places.ReviewCount` là cache vận hành mà Recommendation
Engine đọc nhanh. `PlaceRatings` lưu bản tổng hợp cùng thời điểm tính; service
cập nhật hai nơi trong cùng transaction. `PlaceReviews` lưu từng đánh giá.

Mỗi `Trip` có tối đa một `Itinerary` với `IsSelected = 1`, được bảo vệ bằng
filtered unique index trên `TripId`. `Booking` phải tham chiếu đúng một nguồn:
`TransportOptionId` cho phương tiện hoặc `PlaceId` cho khách sạn, không đồng
thời cả hai; check constraint bảo vệ quy tắc này.

Bảy điểm sở thích dùng cùng một thứ tự và cùng miền giá trị ở
`UserPreferences`, `TripPreferences` và scoring contract. `PreferenceTags` chỉ
được chứa đúng bảy tag đã định nghĩa trong `CONTEXT.md`; validation được thực
thi ở service và seed validation script vì SQL Server không phù hợp để kiểm tra
danh sách CSV bằng check constraint.

## 7. Data access và transaction

Data cung cấp một session ngắn hạn theo use case:

```csharp
public interface ITravilityDataSession : IDisposable
{
    IUserRepository Users { get; }
    IPlaceRepository Places { get; }
    ITripRepository Trips { get; }
    IBudgetRepository Budgets { get; }

    void BeginTransaction();
    int SaveChanges();
    void Commit();
    void Rollback();
}

public interface ITravilityDataSessionFactory
{
    ITravilityDataSession Create();
}
```

Mỗi service tạo session, gọi `BeginTransaction()` cho use case có ghi dữ liệu,
gọi một hoặc nhiều repository, `SaveChanges()` và `Commit()` một lần rồi dispose.
Use case chỉ đọc không mở transaction tường minh. Repository không tự
`SaveChanges()` và không trả `IQueryable`.

Repository tập trung theo nghiệp vụ; không tạo `GenericRepository<T>`. Cổng 1
chốt contract, nhưng walking skeleton chỉ cần implementation hoàn chỉnh cho
`UserRepository`.

```csharp
public interface IUserRepository
{
    User GetById(int userId);
    User GetByNormalizedUsername(string normalizedUsername);
    User GetByNormalizedEmail(string normalizedEmail);
    bool UsernameExists(string normalizedUsername);
    bool EmailExists(string normalizedEmail);
    void Add(User user);
    void Update(User user);
}
```

## 8. Contract Core

Giữ nguyên ý nghĩa của các contract đã có:

- `ITripService`, `IBudgetService`, `IPlaceService`.
- `IRecommendationEngine`, `IRoutingEngine`, `IItineraryEngine`.
- `ILocationProvider`, `IChatProvider`.

Bổ sung contract còn thiếu:

- `IAuthenticationService`, `IPasswordHasher`.
- `IBookingService`, `ICheckInService`, `IAchievementService`.
- `IAppLogger`.
- Data session/repository contract.
- Event arguments cho các event đã chốt.

Expected failure dùng result/error code; không dùng exception cho dữ liệu sai,
sai mật khẩu hoặc thiếu quyền. Lỗi SQL Server, cấu hình và lỗi lập trình mới đi
theo exception tới logger và global handler.

## 9. Auth và session

### 9.1. Contract

```csharp
public interface IAuthenticationService
{
    AuthenticationResult Login(string identifier, string password);
    RegistrationResult Register(RegistrationRequest request);
    PasswordResetResult SetTemporaryPassword(
        int actorUserId,
        int targetUserId,
        string temporaryPassword);
    ChangePasswordResult ChangePassword(
        int userId,
        string currentPassword,
        string newPassword);
}
```

Error code tối thiểu:

```text
InvalidCredentials
AccountDisabled
UsernameAlreadyExists
EmailAlreadyExists
InvalidUsername
InvalidEmail
WeakPassword
PasswordChangeRequired
Forbidden
```

### 9.2. Password

Password dùng PBKDF2-HMAC-SHA256 với salt riêng cho mỗi user. Baseline là
600.000 vòng và iteration được lưu theo user. Trước khi đóng băng cấu hình,
benchmark trên laptop yếu nhất nhóm; chỉ giảm nếu một lần hash/verify vượt
khoảng một giây và phải ghi lại kết quả.

Salt được tạo bằng CSPRNG, dài 16 byte; derived key dài 32 byte. Password hợp lệ
có từ 8 đến 128 ký tự. Ứng dụng không cắt khoảng trắng hoặc âm thầm biến đổi
password trước khi hash.

Thiết kế này thay thế cấu hình 10.000 vòng trong `docs/11-security.md` và
`.claude/rules/security.md`; hai file đó phải được cập nhật trong cùng thay đổi
implementation.

So sánh hash phải theo thời gian hằng. Hệ thống lưu `PasswordAlgorithm` và
`PasswordIterations` để xác minh hoặc nâng cấp hash cũ.

### 9.3. `UserSession`

`UserSession` là đối tượng chỉ đọc gồm:

```text
UserId
Username
DisplayName
Role
AuthenticatedAtUtc
MustChangePassword
```

Session không chứa hash, salt hoặc EF context. Nếu `MustChangePassword = true`,
ứng dụng chỉ mở dialog đổi mật khẩu trước khi cho vào `MainForm`.

## 10. UI shell

### 10.1. `LoginForm`

- Một ô identifier dùng cho username hoặc email.
- Password và nút đăng nhập.
- Liên kết sang `RegisterForm`.
- Validation bằng `ErrorProvider`.
- Không phân biệt công khai “không tồn tại user” và “sai password”.
- Khóa nút khi đang xác thực để ngăn submit lặp.

### 10.2. `MainForm`

`MainForm` mặc định 1280 x 800, resize được, `AutoScaleMode = Dpi`, gồm sidebar,
header, content host và status area.

Traveler thấy:

```text
Tổng quan · Bản đồ · Chuyến đi · Lịch trình
Ngân sách · Check-in · Travel Passport
AI Assistant (khi module được bật)
```

Admin thấy:

```text
Tổng quan · Địa điểm · Người dùng
Phương tiện · Huy hiệu · Thống kê
```

Ẩn navigation chỉ là UX. Service luôn kiểm tra role.

Page chính là `UserControl`, được tạo khi mở lần đầu và cache trong phiên
`MainForm`. Dialog ngắn được tạo mới mỗi lần. WebView2 không khởi tạo trước khi
người dùng mở Map. Đăng xuất dispose toàn bộ page và session rồi quay về Login.

## 11. Logging và xử lý lỗi

Ba lớp lỗi:

1. Lỗi nhập liệu: `ErrorProvider`, không log và không `MessageBox`.
2. Lỗi nghiệp vụ dự kiến: result/error code, ánh xạ sang thông báo tiếng Việt.
3. Lỗi hệ thống: log exception đầy đủ, UI chỉ hiện thông báo an toàn cùng error ID.

`IAppLogger` hỗ trợ `Info`, `Warning`, `Error`. Log nằm tại:

```text
%LocalAppData%\Travility\Logs\travility-yyyyMMdd.log
```

Mỗi record gồm UTC timestamp, severity, component, error ID và exception. Không
log password, hash, salt, API key hoặc connection string. File logger dùng khóa
đồng bộ và giữ log 14 ngày.

Ứng dụng đăng ký `Application.ThreadException` và
`AppDomain.CurrentDomain.UnhandledException`.

## 12. Kiểm thử

### 12.1. Unit test

Password hashing:

- Cùng password với hai salt cho hai hash khác nhau.
- Password đúng thành công, password sai thất bại.
- Hash dùng algorithm và iteration đã cấu hình.
- Hash cũ có iteration thấp vẫn xác minh được.

Identifier:

- Chuỗi có `@` được xem là email.
- Chuỗi không có `@` được xem là username.
- Username chứa `@` bị từ chối.
- Username/email không phân biệt hoa thường.

Authorization:

- Traveler không gọi được thao tác Admin.
- Admin đặt được mật khẩu tạm cho Traveler.
- Tài khoản `MustChangePassword` không nhận phiên sử dụng đầy đủ.

Haversine:

- Hai tọa độ giống nhau cho khoảng cách 0.
- Khoảng cách mẫu khớp giá trị tham chiếu trong sai số cho phép.
- `Distance()` trả Haversine x 1.3.
- Khoảng cách đối xứng.

Không dùng mocking framework. Test dùng fake data-session/repository nhỏ và dữ
liệu đọc được như một câu chuyện.

### 12.2. Database smoke test

`database/smoke_test.sql` xác nhận schema, foreign key, hai role, tài khoản demo,
unique normalized identifiers, không có cột password plaintext, truy vấn bằng
username/email và các check constraint quan trọng.

Smoke test không tự xóa database. Sau khi sinh EDMX phải chạy ứng dụng để xác
nhận EF đọc được tài khoản seed.

### 12.3. CI

Thêm GitHub Actions chạy trên Windows:

```text
Restore NuGet
-> Build Debug
-> Run NUnit tests
-> Build Release
```

CI không cần SQL Server vì unit test không phụ thuộc DB. Database smoke test là
bước thủ công được ghi trong PR.

## 13. Tiêu chí qua cổng

### 13.1. Cổng 1

- Clone mới và restore package thành công.
- `schema.sql` dựng được `TravilityDev` sạch.
- Seed reference/demo chạy không lỗi.
- EDMX đúng và được commit.
- Solution build Debug và Release.
- Contract compile; không có implementation giả thừa.
- CI xanh.
- Không commit secret, `bin/`, `obj/`, `.vs/`, `packages/`.

### 13.2. Cổng 2

- Đăng ký Traveler thành công.
- Đăng nhập bằng username và email.
- Sai password, account disabled và trùng identifier được xử lý đúng.
- Admin/Traveler thấy navigation phù hợp.
- Mật khẩu tạm buộc người dùng đổi.
- Đăng xuất hủy session và quay lại Login.
- Lỗi SQL được log; UI không lộ stack trace.
- Toàn bộ test Foundation xanh.
- Database smoke test đạt.
- Build Debug, Release và CI xanh.

## 14. Phối hợp nhóm

Cổng 1 do Thành sở hữu độc quyền vùng nền móng. Sau khi merge baseline:

- Thành triển khai Auth, Session, Shell, Logging và tích hợp Data.
- Quân triển khai Haversine, NUnit tests và test-data builders.
- Tùng có thể bắt đầu seed/dataset `Place` dựa trên schema đã chốt.
- Nhật có thể dựng Map UI và Trip UI dựa trên contract đã chốt.

Mọi thay đổi contract/schema quay lại Thành điều phối.

```text
Cổng 1: Thành — Solution + Schema + EDMX + Contract + CI
                         |
                         v
           Cả nhóm cập nhật từ baseline
                         |
             +-----------+-----------+
             |                       |
             v                       v
Cổng 2A: Thành                Cổng 2B: Quân
Auth/Session/Shell            Haversine/NUnit
             |                       |
             +-----------+-----------+
                         v
              Walking skeleton tích hợp
                         v
              Smoke test + Release build
```

## 15. Rủi ro và biện pháp

| Rủi ro | Biện pháp |
|---|---|
| EDMX khác nhau giữa máy | Chỉ regenerate sau thay đổi schema được duyệt; commit cùng schema |
| Conflict `.sln`/`.csproj` | Cổng 1 chỉ một người sửa; file mới vào bằng PR nhỏ |
| PBKDF2 chậm | Benchmark laptop yếu nhất trước khi đóng băng iteration |
| SQL chạy một máy | Chuẩn hóa `.\\SQLEXPRESS`; smoke test ít nhất hai máy |
| Form lạm dụng Data reference | Review cấm Form tạo context/repository hoặc truy vấn `DbSet` |
| Scope tràn | Không tích hợp Map/chatbot trước khi Cổng 2 đạt toàn bộ tiêu chí |

## 16. Quyết định đã loại

### Database First tuần tự hoàn toàn

Schema -> EDMX -> repository -> service -> UI làm tuần tự khiến ba thành viên
còn lại phải chờ Thành và đi ngược mục tiêu làm song song.

### Chia bốn project cho bốn người ngay từ đầu

Cách này dễ lệch framework, package, namespace, entity và interface; `.sln` và
`.csproj` trở thành điểm conflict trước khi có baseline.

### Model Core riêng và mapping hai chiều

Kiến trúc sạch hơn về lý thuyết nhưng tăng số model và code mapping mà không
tạo đủ giá trị trong phạm vi sáu tuần.

### `GenericRepository<T>`

Chủ yếu bọc lại EF mà không biểu đạt truy vấn nghiệp vụ hoặc tạo ranh giới hữu
ích. Repository theo aggregate/use case được chọn thay thế.
