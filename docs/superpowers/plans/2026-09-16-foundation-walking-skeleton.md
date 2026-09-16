# Foundation & Walking Skeleton Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Tạo baseline .NET Framework 4.8 build được cho bốn thành viên và một walking skeleton đăng ký/đăng nhập xuyên WinForms → Core → Data → SQL Server, kèm Haversine, kiểm thử và CI.

**Architecture:** Solution gồm `Travility.Data`, `Travility.Core`, `Travility.WinForms` và `Travility.Tests`; phụ thuộc chính là `WinForms → Core → Data`. EF6 Database First sinh entity trong Data, service Core mở data session ngắn hạn theo use case, còn WinForms chỉ gọi service qua constructor injection thủ công tại `Program.cs`.

**Tech Stack:** Visual Studio 2022, C# 7.3, .NET Framework 4.8, Windows Forms, SQL Server Express 2022, Entity Framework 6.4.4, NUnit 3.14.0, NUnit3TestAdapter 4.6.0, GitHub Actions Windows runner.

**Spec:** `docs/superpowers/specs/2026-09-16-foundation-walking-skeleton-design.md`

## Global Constraints

- Thực thi và kiểm chứng build trên Windows có Visual Studio 2022 workload **.NET desktop development** và SQL Server Express 2022 instance `.\SQLEXPRESS`.
- Dùng project .NET Framework cổ điển, target `.NET Framework 4.8`, C# 7.3 và `packages.config`; không chuyển sang SDK-style hoặc `PackageReference`.
- Phụ thuộc chính: `Travility.WinForms → Travility.Core → Travility.Data`; WinForms có compile reference trực tiếp tới Data chỉ cho composition root và các EF entity lộ qua public contract. Form không dùng Data API. `Travility.Core` không tham chiếu `System.Windows.Forms`.
- EF entities và repository nằm trong `Travility.Data`; Form không tạo `TravilityEntities`, repository hoặc truy vấn `DbSet`.
- EF cho CRUD; ADO.NET chỉ cho stored procedure `DistanceMatrix` và thống kê.
- Tiền luôn là `decimal`; thời điểm hệ thống lưu UTC; ngày chuyến đi và giờ mở cửa giữ theo giờ địa phương.
- Password dùng PBKDF2-HMAC-SHA256, salt 16 byte, derived key 32 byte, baseline 600.000 vòng; password dài 8–128 ký tự.
- Validation UI dùng `ErrorProvider`; văn bản UI bằng tiếng Việt có dấu; mọi Form đặt `AutoScaleMode = Dpi`.
- Không dùng mocking framework. Test logic thuần bằng fake nhỏ trong `Travility.Tests`.
- Không sửa file sinh từ EDMX bằng tay; extension nằm trong file partial.
- Không commit `bin/`, `obj/`, `packages/`, `.vs/`, secret hoặc connection string chứa mật khẩu.
- Chỉ Thành sửa `database/schema.sql`, contract nền, `.sln` và `.csproj` trong Cổng 1.

---

## File Map

### Solution và cấu hình

- `Travility.sln` — solution bốn project.
- `src/Travility.Data/Travility.Data.csproj` — EF model, repository và data session.
- `src/Travility.Core/Travility.Core.csproj` — contract, service, security, session và engine thuần.
- `src/Travility.WinForms/Travility.WinForms.csproj` — composition root và UI.
- `src/Travility.WinForms/App.config` — connection string Windows Authentication.
- `tests/Travility.Tests/Travility.Tests.csproj` — NUnit tests.
- `.github/workflows/windows-build.yml` — restore, Debug build/test và Release build.

### Database

- `database/reset-dev.sql` — reset duy nhất database `TravilityDev`.
- `database/schema.sql` — bảng, FK, constraint và index.
- `database/seed_reference.sql` — role, category, budget category, achievement.
- `database/seed_demo.sql` — Admin/Traveler demo bằng hash PBKDF2 cố định.
- `database/procedures.sql` — ba stored procedure đã chốt.
- `database/smoke_test.sql` — kiểm tra schema/seed, không xóa dữ liệu.
- `database/README.md` — thứ tự chạy và tài khoản demo.

### Data

- `src/Travility.Data/Model/TravilityModel.edmx` — model Database First.
- `src/Travility.Data/Model/User.partial.cs` — extension không sửa generated code.
- `src/Travility.Data/Contracts/ITravilityDataSession.cs` — biên transaction/use case.
- `src/Travility.Data/Contracts/ITravilityDataSessionFactory.cs` — tạo session ngắn hạn.
- `src/Travility.Data/Contracts/IUserRepository.cs` — truy vấn/ghi user.
- `src/Travility.Data/Contracts/IPlaceRepository.cs` — contract Place tối thiểu.
- `src/Travility.Data/Contracts/ITripRepository.cs` — contract Trip tối thiểu.
- `src/Travility.Data/Contracts/IBudgetRepository.cs` — contract Budget tối thiểu.
- `src/Travility.Data/Repositories/UserRepository.cs` — implementation EF cho Auth.
- `src/Travility.Data/Repositories/PlaceRepository.cs` — lookup Place tối thiểu.
- `src/Travility.Data/Repositories/TripRepository.cs` — lookup Trip tối thiểu.
- `src/Travility.Data/Repositories/BudgetRepository.cs` — lookup Budget tối thiểu.
- `src/Travility.Data/TravilityDataSession.cs` — context + transaction.
- `src/Travility.Data/TravilityDataSessionFactory.cs` — factory concrete.

### Core

- `src/Travility.Core/Security/IPasswordHasher.cs` — contract hash/verify.
- `src/Travility.Core/Security/PasswordHash.cs` — salt/hash/algorithm/iterations.
- `src/Travility.Core/Security/Pbkdf2PasswordHasher.cs` — PBKDF2-HMAC-SHA256.
- `src/Travility.Core/Security/LoginIdentifier.cs` — định danh đã chuẩn hóa.
- `src/Travility.Core/Security/LoginIdentifierNormalizer.cs` — username/email dispatch.
- `src/Travility.Core/Contracts/IAuthenticationService.cs` — Auth use cases.
- `src/Travility.Core/Contracts/IAppLogger.cs` — logging abstraction.
- `src/Travility.Core/Contracts/*.cs` — service/engine/external-provider contract còn lại.
- `src/Travility.Core/Authentication/*.cs` — request/result/error code.
- `src/Travility.Core/Models/*.cs` — DTO dùng trong public contracts.
- `src/Travility.Core/Services/AuthenticationService.cs` — register/login/password flows.
- `src/Travility.Core/Session/UserSession.cs` — session chỉ đọc.
- `src/Travility.Core/Session/NavigationPolicy.cs` — menu theo role.
- `src/Travility.Core/Engines/RoutingEngine.cs` — Haversine × 1.3.
- `src/Travility.Core/Events/*.cs` — event args đã chốt.

### WinForms

- `src/Travility.WinForms/Program.cs` — composition root và global handlers.
- `src/Travility.WinForms/TravilityApplicationContext.cs` — chuyển Login ↔ MainForm trong một message loop.
- `src/Travility.WinForms/Infrastructure/FileAppLogger.cs` — log theo ngày.
- `src/Travility.WinForms/Infrastructure/GlobalExceptionHandler.cs` — error ID + safe UI.
- `src/Travility.WinForms/Auth/LoginForm.*` — login bằng username/email.
- `src/Travility.WinForms/Auth/RegisterForm.*` — đăng ký Traveler.
- `src/Travility.WinForms/Auth/ChangePasswordDialog.*` — bắt buộc đổi mật khẩu tạm.
- `src/Travility.WinForms/Shell/MainForm.*` — sidebar/header/content/status.
- `src/Travility.WinForms/Pages/HomePage.*` — page Foundation tối thiểu.

### Tests

- `tests/Travility.Tests/Security/Pbkdf2PasswordHasherTests.cs`.
- `tests/Travility.Tests/Security/LoginIdentifierNormalizerTests.cs`.
- `tests/Travility.Tests/Authentication/AuthenticationServiceTests.cs`.
- `tests/Travility.Tests/Session/NavigationPolicyTests.cs`.
- `tests/Travility.Tests/Infrastructure/FileAppLoggerTests.cs`.
- `tests/Travility.Tests/Engines/RoutingEngineTests.cs`.
- `tests/Travility.Tests/TestData/FakeTravilityDataSession.cs`.
- `tests/Travility.Tests/TestData/FakeUserRepository.cs`.
- `tests/Travility.Tests/TestData/AuthFixture.cs`.
- `tests/Travility.Tests/TestData/TempDirectory.cs`.

---

### Task 1: Scaffold solution và test harness

**Files:**
- Create: `Travility.sln`
- Create: `src/Travility.Data/Travility.Data.csproj`
- Create: `src/Travility.Core/Travility.Core.csproj`
- Create: `src/Travility.WinForms/Travility.WinForms.csproj`
- Create: `tests/Travility.Tests/Travility.Tests.csproj`
- Create: `tests/Travility.Tests/Architecture/DependencyTests.cs`
- Modify: `src/README.md`
- Modify: `tests/README.md`

**Interfaces:**
- Consumes: Không có.
- Produces: Bốn assembly target .NET Framework 4.8 và project references `Core → Data`, `WinForms → Core + Data`, `Tests → Core + Data + WinForms`.

- [x] **Step 1: Tạo bốn project bằng Visual Studio 2022**

Trong Visual Studio, tạo blank solution `Travility`; thêm ba **Class Library (.NET Framework)** và một **Windows Forms App (.NET Framework)**, chọn Framework 4.8. Đặt test project là Class Library `.NET Framework 4.8`. Xóa `Class1.cs` và Form mặc định.

Đặt trong từng `.csproj`:

```xml
<LangVersion>7.3</LangVersion>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```

Thêm references: Core → Data; WinForms → Core và Data; Tests → Core, Data và WinForms. Tests tham chiếu WinForms chỉ để test infrastructure như file logger, không test Form. Reference Data trong WinForms chỉ dùng tại `Program.cs`/composition root và để compile kiểu entity; các Form không được import namespace repository/context. Core không có reference `System.Windows.Forms`.

Sau khi xóa Form mặc định, giữ entry point build được:

```csharp
[STAThread]
private static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
}
```

- [x] **Step 2: Cài package test và EF**

Trong Package Manager Console:

```powershell
Install-Package EntityFramework -Version 6.4.4 -ProjectName Travility.Data
Install-Package NUnit -Version 3.14.0 -ProjectName Travility.Tests
Install-Package NUnit3TestAdapter -Version 4.6.0 -ProjectName Travility.Tests
Install-Package Microsoft.NET.Test.Sdk -Version 17.11.1 -ProjectName Travility.Tests
```

- [x] **Step 3: Viết architecture test đầu tiên**

```csharp
using System.Linq;
using NUnit.Framework;

namespace Travility.Tests.Architecture
{
    [TestFixture]
    public sealed class DependencyTests
    {
        [Test]
        public void Phai_KhongThamChieuWinForms_Khi_LoadTravilityCore()
        {
            var references = typeof(Travility.Core.CoreAssemblyMarker)
                .Assembly
                .GetReferencedAssemblies()
                .Select(x => x.Name)
                .ToArray();

            CollectionAssert.DoesNotContain(references, "System.Windows.Forms");
            CollectionAssert.DoesNotContain(references, "Travility.WinForms");
        }
    }
}
```

Tạo `src/Travility.Core/CoreAssemblyMarker.cs`:

```csharp
namespace Travility.Core
{
    public sealed class CoreAssemblyMarker
    {
    }
}
```

- [x] **Step 4: Restore, build và chạy test**

Chạy trong **Developer PowerShell for VS 2022**:

```powershell
nuget restore Travility.sln
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll
```

Expected: build thành công; một test PASS.

- [x] **Step 5: Commit scaffold**

```bash
git add Travility.sln src tests
git commit -m "build: khoi tao solution net48"
```

---

### Task 2: Password hashing test-first và cập nhật quy tắc bảo mật

**Files:**
- Create: `src/Travility.Core/Security/IPasswordHasher.cs`
- Create: `src/Travility.Core/Security/PasswordHash.cs`
- Create: `src/Travility.Core/Security/Pbkdf2PasswordHasher.cs`
- Create: `tests/Travility.Tests/Security/Pbkdf2PasswordHasherTests.cs`
- Modify: `docs/11-security.md`
- Modify: `.claude/rules/security.md`

**Interfaces:**
- Consumes: `Travility.Core` testable không cần UI/DB.
- Produces: `PasswordHash IPasswordHasher.Hash(string password)` và `bool IPasswordHasher.Verify(...)`.

- [x] **Step 1: Viết failing tests**

```csharp
[Test]
public void Phai_TaoHaiHashKhacNhau_Khi_CungMatKhau()
{
    var hasher = new Pbkdf2PasswordHasher(600000);
    var first = hasher.Hash("MatKhau@123");
    var second = hasher.Hash("MatKhau@123");

    CollectionAssert.AreNotEqual(first.Salt, second.Salt);
    CollectionAssert.AreNotEqual(first.Hash, second.Hash);
}

[TestCase("MatKhau@123", true)]
[TestCase("SaiMatKhau", false)]
public void Phai_XacMinhDung_Khi_SoSanhMatKhau(string candidate, bool expected)
{
    var hasher = new Pbkdf2PasswordHasher(600000);
    var stored = hasher.Hash("MatKhau@123");

    Assert.That(hasher.Verify(candidate, stored), Is.EqualTo(expected));
}
```

- [x] **Step 2: Chạy test để xác nhận RED**

```powershell
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll /TestCaseFilter:"FullyQualifiedName~Pbkdf2PasswordHasherTests"
```

Expected: FAIL do chưa có `Pbkdf2PasswordHasher`.

- [x] **Step 3: Viết contract và value object**

```csharp
public interface IPasswordHasher
{
    PasswordHash Hash(string password);
    bool Verify(string password, PasswordHash stored);
}

public sealed class PasswordHash
{
    public PasswordHash(byte[] hash, byte[] salt, int iterations, string algorithm)
    {
        Hash = hash;
        Salt = salt;
        Iterations = iterations;
        Algorithm = algorithm;
    }

    public byte[] Hash { get; }
    public byte[] Salt { get; }
    public int Iterations { get; }
    public string Algorithm { get; }
}
```

- [x] **Step 4: Viết PBKDF2 tối thiểu**

```csharp
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    public const string AlgorithmName = "PBKDF2-HMAC-SHA256";
    private const int SaltLength = 16;
    private const int HashLength = 32;
    private readonly int _iterations;

    public Pbkdf2PasswordHasher(int iterations)
    {
        if (iterations <= 0) throw new ArgumentOutOfRangeException(nameof(iterations));
        _iterations = iterations;
    }

    public PasswordHash Hash(string password)
    {
        Validate(password);
        var salt = new byte[SaltLength];
        using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
        return new PasswordHash(Derive(password, salt, _iterations), salt, _iterations, AlgorithmName);
    }

    public bool Verify(string password, PasswordHash stored)
    {
        if (stored == null ||
            !string.Equals(stored.Algorithm, AlgorithmName, StringComparison.Ordinal) ||
            stored.Salt == null || stored.Salt.Length != SaltLength ||
            stored.Hash == null || stored.Hash.Length != HashLength ||
            stored.Iterations <= 0)
            return false;
        var actual = Derive(password, stored.Salt, stored.Iterations);
        var difference = 0;
        for (var i = 0; i < actual.Length; i++) difference |= actual[i] ^ stored.Hash[i];
        return difference == 0;
    }

    private static byte[] Derive(string password, byte[] salt, int iterations)
    {
        using (var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            return derive.GetBytes(HashLength);
    }

    private static void Validate(string password)
    {
        if (password == null || password.Length < 8 || password.Length > 128)
            throw new ArgumentException("Mật khẩu phải có từ 8 đến 128 ký tự.", nameof(password));
    }
}
```

- [x] **Step 5: Chạy test và benchmark thủ công**

```powershell
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll /TestCaseFilter:"FullyQualifiedName~Pbkdf2PasswordHasherTests"
Measure-Command { 1..3 | ForEach-Object { & vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll /TestCaseFilter:"Name=Phai_XacMinhDung_Khi_SoSanhMatKhau" | Out-Null } }
```

Expected: tests PASS; ghi thời gian benchmark vào PR. Chỉ thay đổi iteration nếu một lần hash/verify vượt một giây trên laptop yếu nhất.

- [x] **Step 6: Đồng bộ tài liệu bảo mật**

Thay mọi chỗ ghi `10.000 vòng` bằng `PBKDF2-HMAC-SHA256, baseline 600.000 vòng, salt 16 byte, hash 32 byte; benchmark dưới khoảng một giây` trong hai file quy tắc.

- [x] **Step 7: Commit**

```bash
git add src/Travility.Core/Security tests/Travility.Tests/Security docs/11-security.md .claude/rules/security.md
git commit -m "feat(auth): them pbkdf2 password hasher"
```

---

### Task 3: Identity schema và seed xác định

**Files:**
- Create: `database/reset-dev.sql`
- Create: `database/schema.sql`
- Create: `database/seed_reference.sql`
- Create: `database/seed_demo.sql`
- Create: `database/smoke_test.sql`
- Modify: `database/README.md`

**Interfaces:**
- Consumes: Schema §6 của spec; PBKDF2 format từ Task 2.
- Produces: `Roles`, `Users`, `UserProfiles`, `UserPreferences`, `TravelWallets` và hai tài khoản demo.

- [x] **Step 1: Viết smoke assertions trước schema**

`database/smoke_test.sql` phải bật lỗi cho SQLCMD:

```sql
USE TravilityDev;
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL THROW 51000, 'Missing dbo.Users', 1;
IF (SELECT COUNT(*) FROM dbo.Roles WHERE Name IN (N'Admin', N'Traveler')) <> 2
    THROW 51001, 'Roles seed is invalid', 1;
IF COL_LENGTH(N'dbo.Users', N'Password') IS NOT NULL
    THROW 51002, 'Plaintext Password column is forbidden', 1;
IF (SELECT COUNT(*) FROM dbo.Users WHERE IsActive = 1) < 2
    THROW 51003, 'Demo users are missing', 1;
```

- [x] **Step 2: Chạy smoke test để xác nhận RED**

```powershell
sqlcmd -S .\SQLEXPRESS -E -b -i database\smoke_test.sql
```

Expected: non-zero exit do `TravilityDev` hoặc `dbo.Users` chưa tồn tại.

- [x] **Step 3: Viết reset script có guard**

```sql
USE master;
IF DB_ID(N'TravilityDev') IS NOT NULL
BEGIN
    ALTER DATABASE TravilityDev SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TravilityDev;
END;
CREATE DATABASE TravilityDev;
```

File không nhận database name từ biến và không thao tác database khác.

- [x] **Step 4: Viết Identity DDL**

Tạo năm bảng đúng §6.6 của spec. `Users` phải có:

```sql
CREATE TABLE dbo.Users
(
    UserId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    RoleId int NOT NULL,
    Username nvarchar(50) NOT NULL,
    NormalizedUsername nvarchar(50) NOT NULL,
    Email nvarchar(254) NOT NULL,
    NormalizedEmail nvarchar(254) NOT NULL,
    PasswordHash varbinary(32) NOT NULL,
    PasswordSalt varbinary(16) NOT NULL,
    PasswordIterations int NOT NULL,
    PasswordAlgorithm nvarchar(40) NOT NULL,
    MustChangePassword bit NOT NULL CONSTRAINT DF_Users_MustChange DEFAULT 0,
    IsActive bit NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
    CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Users_Created DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Users_Updated DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId),
    CONSTRAINT CK_Users_Username_NoAt CHECK (Username NOT LIKE N'%@%'),
    CONSTRAINT CK_Users_Iterations_Positive CHECK (PasswordIterations > 0)
);
CREATE UNIQUE INDEX UX_Users_NormalizedUsername ON dbo.Users(NormalizedUsername);
CREATE UNIQUE INDEX UX_Users_NormalizedEmail ON dbo.Users(NormalizedEmail);
```

- [x] **Step 5: Viết reference/demo seed**

Seed role bằng `MERGE`. Seed demo dùng đúng dữ liệu xác định sau:

```text
admin / admin@travility.local / Admin@12345
traveler / traveler@travility.local / Traveler@12345
```

```sql
-- admin: salt 00112233445566778899AABBCCDDEEFF
-- PBKDF2-HMAC-SHA256(Admin@12345, 600000)
0xA9041EADD01FFF29325410FE8A716DA3E86CC159B7AED88F6E1486B8B80D4D28

-- traveler: salt FFEEDDCCBBAA99887766554433221100
-- PBKDF2-HMAC-SHA256(Traveler@12345, 600000)
0x71F29F3C959444A62FBD6478F40241CF151F66DC4F33D070A68A8B6EE1977919
```

Đặt `MustChangePassword = 0` cho hai tài khoản demo; `PasswordAlgorithm = N'PBKDF2-HMAC-SHA256'`.

- [x] **Step 6: Chạy Identity scripts và smoke test**

```powershell
sqlcmd -S .\SQLEXPRESS -E -b -i database\reset-dev.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\schema.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\seed_reference.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\seed_demo.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\smoke_test.sql
```

Expected: mọi lệnh exit code 0.

- [x] **Step 7: Commit**

```bash
git add database
git commit -m "feat(database): them identity schema va demo seed"
```

---

### Task 4: Hoàn thành domain schema, procedure và smoke test

**Files:**
- Modify: `database/schema.sql`
- Modify: `database/seed_reference.sql`
- Create: `database/procedures.sql`
- Modify: `database/smoke_test.sql`
- Modify: `database/README.md`

**Interfaces:**
- Consumes: Data dictionary §6.6 và business rules trong spec.
- Produces: Toàn bộ schema v1 và ba stored procedure ADO.NET.

- [x] **Step 1: Mở rộng smoke test để yêu cầu toàn schema**

Thêm danh sách và fail nếu thiếu bảng:

```sql
DECLARE @RequiredTables TABLE (Name sysname NOT NULL);
INSERT INTO @RequiredTables(Name) VALUES
(N'Places'), (N'Trips'), (N'Itineraries'), (N'Expenses'),
(N'CheckIns'), (N'Achievements'), (N'ChatSessions'), (N'ToolExecutions');

IF EXISTS
(
    SELECT 1 FROM @RequiredTables r
    WHERE OBJECT_ID(N'dbo.' + r.Name, N'U') IS NULL
)
    THROW 51010, 'One or more domain tables are missing', 1;
```

Thêm assertions cho 8 `BudgetCategories`, unique `(UserId, AchievementId)`, không có `TravelPassports`, và ba procedure.

- [x] **Step 2: Chạy smoke test để xác nhận RED**

```powershell
sqlcmd -S .\SQLEXPRESS -E -b -i database\smoke_test.sql
```

Expected: FAIL do domain tables/procedures chưa tồn tại.

- [x] **Step 3: Thêm các nhóm bảng đúng thứ tự FK**

Thêm vào `schema.sql` theo thứ tự:

```text
PlaceCategories → PlaceTypes → Places → PlaceImages → PlaceRatings
→ PlaceReviews → ExternalPlaceMappings
Trips → TripMembers → TripPreferences → TransportOptions → Bookings → TripPlaces
Itineraries → ItineraryDays → ItineraryItems → RoutePlans → RouteSegments → DistanceMatrix
BudgetCategories → TripBudgets → BudgetAllocations → Expenses
CheckIns → Achievements → UserAchievements
ChatSessions → ChatMessages → ToolExecutions
```

Dùng đúng cột ở spec §6.6. Mọi tiền là `decimal(18,2)`. FK mặc định `NO ACTION`.

Thêm check quan trọng:

```sql
CONSTRAINT CK_Places_Latitude CHECK (Latitude BETWEEN -90 AND 90),
CONSTRAINT CK_Places_Longitude CHECK (Longitude BETWEEN -180 AND 180),
CONSTRAINT CK_Places_Rating CHECK (Rating BETWEEN 0 AND 5),
CONSTRAINT CK_Trips_Dates CHECK (ReturnDate > DepartDate),
CONSTRAINT CK_Trips_People CHECK (PeopleCount >= 1),
CONSTRAINT CK_DistanceMatrix_Order CHECK (FromPlaceId < ToPlaceId)
```

Tạo filtered unique index:

```sql
CREATE UNIQUE INDEX UX_Itineraries_SelectedPerTrip
ON dbo.Itineraries(TripId)
WHERE IsSelected = 1;
```

- [x] **Step 4: Seed reference data**

Seed đúng 8 budget category:

```text
Transportation · Accommodation · Food · Activities
LocalTransport · Shopping · Reserve · Other
```

Seed bảy preference/category labels đúng `CONTEXT.md`; seed năm achievement từ `docs/09-checkin-gamification.md`.

- [x] **Step 5: Viết ba stored procedure**

`procedures.sql` tạo:

```sql
CREATE OR ALTER PROCEDURE dbo.sp_GetDistanceMatrix
    @PlaceIds dbo.IntIdList READONLY
AS
SELECT dm.*
FROM dbo.DistanceMatrix dm
WHERE dm.FromPlaceId IN (SELECT Id FROM @PlaceIds)
  AND dm.ToPlaceId IN (SELECT Id FROM @PlaceIds);
GO

CREATE OR ALTER PROCEDURE dbo.sp_TopPlacesAddedToTrips
AS
SELECT TOP (20) p.PlaceId, p.Name, COUNT_BIG(*) AS AddedCount
FROM dbo.TripPlaces tp JOIN dbo.Places p ON p.PlaceId = tp.PlaceId
GROUP BY p.PlaceId, p.Name ORDER BY AddedCount DESC;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AvgExpenseByCategory
AS
SELECT bc.BudgetCategoryId, bc.Name, AVG(CAST(e.Amount AS decimal(18,2))) AS AverageAmount
FROM dbo.Expenses e JOIN dbo.BudgetCategories bc ON bc.BudgetCategoryId = e.BudgetCategoryId
GROUP BY bc.BudgetCategoryId, bc.Name;
GO
```

Tạo table type `dbo.IntIdList(Id int NOT NULL PRIMARY KEY)` trước procedure đầu.

- [x] **Step 6: Rebuild database và chạy smoke test hai lần**

```powershell
sqlcmd -S .\SQLEXPRESS -E -b -i database\reset-dev.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\schema.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\seed_reference.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\seed_demo.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\procedures.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\smoke_test.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\seed_reference.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\seed_demo.sql
sqlcmd -S .\SQLEXPRESS -E -b -i database\smoke_test.sql
```

Expected: cả hai vòng seed/smoke exit code 0, chứng minh seed chạy lại được.

- [x] **Step 7: Commit**

```bash
git add database
git commit -m "feat(database): hoan thanh schema v1"
```

---

### Task 5: Sinh EDMX và khóa generated-model workflow

**Files:**
- Create: `src/Travility.Data/Model/TravilityModel.edmx`
- Create: generated `.tt`, context và entity files dưới `src/Travility.Data/Model/`
- Create: `src/Travility.Data/Model/User.partial.cs`
- Modify: `src/Travility.Data/App.config`
- Modify: `src/Travility.Data/Travility.Data.csproj`

**Interfaces:**
- Consumes: `TravilityDev` schema v1.
- Produces: `Travility.Data.Model.TravilityEntities` và EF entity classes như `User`, `Place`, `Trip`.

- [x] **Step 1: Sinh EDMX bằng Visual Studio**

Trong `Travility.Data`: Add → New Item → ADO.NET Entity Data Model → **EF Designer from database** → connection `.\SQLEXPRESS`, database `TravilityDev`, Windows Authentication. Chọn toàn bộ tables; không import procedure vì ba procedure được gọi bằng ADO.NET. Đặt model namespace `TravilityModel`; entity namespace `Travility.Data.Model`.

Không lưu password trong connection string. Đặt connection string name `TravilityEntities`.

- [x] **Step 2: Thêm partial extension không chạm generated file**

```csharp
namespace Travility.Data.Model
{
    public partial class User
    {
        public bool IsAdmin
        {
            get { return Role != null && Role.Name == "Admin"; }
        }
    }
}
```

- [x] **Step 3: Build model**

```powershell
nuget restore Travility.sln
msbuild Travility.sln /m /p:Configuration=Debug
```

Expected: build thành công, không có duplicate entity hoặc missing connection string.

- [x] **Step 4: Kiểm tra generated diff và commit**

```bash
git status --short
git diff --check
git add src/Travility.Data
git commit -m "feat(data): sinh ef6 model tu schema v1"
```

---

### Task 6: Data contracts và transaction boundary

**Files:**
- Create: `src/Travility.Data/Contracts/ITravilityDataSession.cs`
- Create: `src/Travility.Data/Contracts/ITravilityDataSessionFactory.cs`
- Create: `src/Travility.Data/Contracts/IUserRepository.cs`
- Create: `src/Travility.Data/Contracts/IPlaceRepository.cs`
- Create: `src/Travility.Data/Contracts/ITripRepository.cs`
- Create: `src/Travility.Data/Contracts/IBudgetRepository.cs`
- Create: `tests/Travility.Tests/Architecture/DataContractTests.cs`

**Interfaces:**
- Consumes: EF entities từ Task 5.
- Produces: Data abstractions mà Auth/Core dùng từ Task 9 trở đi.

- [x] **Step 1: Viết compile-time contract test**

```csharp
[Test]
public void Phai_TraEntityCuThe_Khi_DungUserRepositoryContract()
{
    Assert.That(typeof(IUserRepository).GetMethod("GetById").ReturnType,
        Is.EqualTo(typeof(User)));
    Assert.That(typeof(IUserRepository).GetMethods().Any(m =>
        m.ReturnType.IsGenericType &&
        m.ReturnType.GetGenericTypeDefinition() == typeof(IQueryable<>)), Is.False);
}
```

- [x] **Step 2: Chạy test để xác nhận RED**

Expected: test project không compile vì `IUserRepository` chưa tồn tại.

- [x] **Step 3: Tạo exact contracts**

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

public interface IUserRepository
{
    User GetById(int userId);
    User GetByNormalizedUsername(string normalizedUsername);
    User GetByNormalizedEmail(string normalizedEmail);
    bool UsernameExists(string normalizedUsername);
    bool EmailExists(string normalizedEmail);
    Role GetRoleByName(string roleName);
    void Add(User user);
    void AddProfile(UserProfile profile);
    void AddWallet(TravelWallet wallet);
    void Update(User user);
}
```

Tạo ba contract còn lại với chữ ký chính xác:

```csharp
public interface IPlaceRepository
{
    Place GetById(int placeId);
    IList<Place> GetByCategory(int categoryId);
    IList<Place> GetByIds(IList<int> placeIds);
    void Add(Place place);
    void Update(Place place);
}

public interface ITripRepository
{
    Trip GetById(int tripId);
    IList<Trip> GetByUser(int userId);
    void Add(Trip trip);
    void Update(Trip trip);
}

public interface IBudgetRepository
{
    TripBudget GetByTrip(int tripId);
    TravelWallet GetWalletByUser(int userId);
    IList<Expense> GetExpensesByTrip(int tripId);
    void AddExpense(Expense expense);
}
```

Không contract nào trả `IQueryable` hoặc có `SaveChanges()`.

- [x] **Step 4: Chạy architecture tests**

```powershell
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll /TestCaseFilter:"FullyQualifiedName~DataContractTests"
```

Expected: PASS.

- [x] **Step 5: Commit**

```bash
git add src/Travility.Data/Contracts tests/Travility.Tests/Architecture
git commit -m "feat(data): chot data session va repository contracts"
```

---

### Task 7: Core contracts, DTO và event baseline

**Files:**
- Create: `src/Travility.Core/Contracts/IAuthenticationService.cs`
- Create: `src/Travility.Core/Contracts/IAppLogger.cs`
- Create: `src/Travility.Core/Contracts/ITripService.cs`
- Create: `src/Travility.Core/Contracts/IBudgetService.cs`
- Create: `src/Travility.Core/Contracts/IPlaceService.cs`
- Create: `src/Travility.Core/Contracts/IBookingService.cs`
- Create: `src/Travility.Core/Contracts/ICheckInService.cs`
- Create: `src/Travility.Core/Contracts/IAchievementService.cs`
- Create: `src/Travility.Core/Contracts/IRecommendationEngine.cs`
- Create: `src/Travility.Core/Contracts/IRoutingEngine.cs`
- Create: `src/Travility.Core/Contracts/IItineraryEngine.cs`
- Create: `src/Travility.Core/Contracts/ILocationProvider.cs`
- Create: `src/Travility.Core/Contracts/IChatProvider.cs`
- Create: `src/Travility.Core/Authentication/*.cs`
- Create: `src/Travility.Core/Models/*.cs`
- Create: `src/Travility.Core/Events/*.cs`
- Modify: `docs/04-internal-contracts.md`

**Interfaces:**
- Consumes: Data entities/contracts từ Tasks 5–6.
- Produces: Public Core API dùng bởi mọi module và Cổng 2.

- [x] **Step 1: Viết reflection test khóa chữ ký trọng yếu**

```csharp
[Test]
public void Phai_GiuChuKyTopK_Khi_LoadRecommendationContract()
{
    var method = typeof(IRecommendationEngine).GetMethod("TopK");
    Assert.That(method, Is.Not.Null);
    Assert.That(method.ReturnType, Is.EqualTo(typeof(IList<ScoredPlace>)));
}

[Test]
public void Phai_CoBonAuthUseCase_Khi_LoadAuthenticationContract()
{
    var names = typeof(IAuthenticationService).GetMethods().Select(x => x.Name).ToArray();
    CollectionAssert.AreEquivalent(
        new[] { "Login", "Register", "SetTemporaryPassword", "ChangePassword" }, names);
}
```

- [x] **Step 2: Chạy test để xác nhận RED**

Expected: compile fail vì Core contracts chưa tồn tại.

- [x] **Step 3: Tạo Auth DTO/result contracts**

```csharp
public interface IAuthenticationService
{
    AuthenticationResult Login(string identifier, string password);
    RegistrationResult Register(RegistrationRequest request);
    PasswordResetResult SetTemporaryPassword(int actorUserId, int targetUserId, string temporaryPassword);
    ChangePasswordResult ChangePassword(int userId, string currentPassword, string newPassword);
}

public interface IAppLogger
{
    void Info(string component, string message);
    void Warning(string component, string message);
    void Error(string component, string errorId, Exception exception);
}

public enum AuthenticationErrorCode
{
    None,
    InvalidCredentials,
    AccountDisabled,
    UsernameAlreadyExists,
    EmailAlreadyExists,
    InvalidUsername,
    InvalidEmail,
    WeakPassword,
    PasswordChangeRequired,
    Forbidden
}
```

`RegistrationRequest` chứa `Username`, `Email`, `DisplayName`, `Password`.
`AuthenticationResult` chứa `bool Succeeded`, `AuthenticationErrorCode ErrorCode`
và `AuthenticatedUser User`. Các result còn lại đều có `Succeeded` và
`ErrorCode`; `RegistrationResult` thêm `int UserId` khi thành công.
`AuthenticatedUser` chứa `UserId`, `Username`, `DisplayName`, `Role`, `MustChangePassword`.

Tạo các DTO được nhắc trong `docs/04-internal-contracts.md`: `TripDraft`,
`PlaceQuery`, `RecommendationContext`, `RoutingOptions`, `Coordinate`,
`ScoredPlace`, `ChatResponse`, `ChatMessage`, `ToolDefinition`. DTO không tham
chiếu WinForms và không chứa `DbContext`.

- [x] **Step 4: Chuyển chữ ký đã duyệt từ tài liệu sang interface**

Dùng nguyên chữ ký trong `docs/04-internal-contracts.md` cho Trip/Budget/Place/ba engine/location/chat.

Các contract bổ sung có chữ ký:

```csharp
public interface IBookingService
{
    Booking Create(BookingDraft draft);
    IList<Booking> GetByTrip(int tripId);
    void UpdateStatus(int bookingId, BookingStatus status, int actorUserId);
}

public interface ICheckInService
{
    CheckInResult CheckIn(CheckInRequest request);
    IList<CheckIn> GetByTrip(int tripId);
}

public interface IAchievementService
{
    IList<Achievement> EvaluateForCheckIn(int checkInId);
    IList<UserAchievement> GetByUser(int userId);
}
```

Tạo `BookingDraft`, `CheckInRequest`, `CheckInResult`, `BookingStatus` trong
`Core/Models`; chúng dùng property có kiểu cụ thể (`int`, `decimal`, `DateTime`,
`Coordinate`) và không dùng `object` hoặc dictionary thay model.

Event args cho đúng tám event:

```text
PlaceAdded · PlaceRemoved · HotelChanged · BookingConfirmed
BudgetChanged · ExpenseRecorded · CheckInCompleted · ItineraryGenerated
```

Mỗi event args chỉ mang ID và dữ liệu tối thiểu cần cho subscriber; không mang Form hoặc DbContext.

- [x] **Step 5: Chạy toàn bộ architecture tests**

```powershell
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll /TestCaseFilter:"FullyQualifiedName~Architecture"
```

Expected: PASS.

- [x] **Step 6: Commit contract baseline**

```bash
git add src/Travility.Core docs/04-internal-contracts.md tests/Travility.Tests/Architecture
git commit -m "feat(core): chot contracts foundation"
```

---

### Task 8: Windows CI và xác nhận Cổng 1

**Files:**
- Create: `.github/workflows/windows-build.yml`
- Modify: `.github/pull_request_template.md`

**Interfaces:**
- Consumes: Solution, schema, EDMX và contracts từ Tasks 1–7.
- Produces: CI gate cho Debug/test/Release.

- [x] **Step 1: Thêm workflow**

```yaml
name: windows-build

on:
  pull_request:
  push:
    branches: [main]

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: NuGet/setup-nuget@v2
      - uses: microsoft/setup-msbuild@v2
      - name: Restore
        run: nuget restore Travility.sln
      - name: Build Debug
        run: msbuild Travility.sln /m /p:Configuration=Debug
      - name: Test
        shell: pwsh
        run: |
          $vstest = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.VisualStudio.PackageGroup.TestTools.Core -find **\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe | Select-Object -First 1
          & $vstest tests\Travility.Tests\bin\Debug\Travility.Tests.dll
      - name: Build Release
        run: msbuild Travility.sln /m /p:Configuration=Release
```

- [x] **Step 2: Chạy cùng chuỗi lệnh cục bộ**

```powershell
nuget restore Travility.sln
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll
msbuild Travility.sln /m /p:Configuration=Release
```

Expected: toàn bộ PASS.

- [x] **Step 3: Chạy full database rebuild/smoke**

Chạy sáu script theo đúng Task 4. Expected: exit code 0.

- [x] **Step 4: Cập nhật PR checklist**

Thêm checkbox xác nhận `database/smoke_test.sql` khi PR đổi schema/EDMX và xác nhận không sửa generated entity bằng tay.

- [x] **Step 5: Commit Cổng 1**

```bash
git add .github
git commit -m "ci: kiem tra build va unit test tren windows"
```

Reviewer chỉ duyệt Cổng 1 khi Debug/Release build, unit tests, database smoke và CI đều xanh.

---

### Task 9: EF data session và `UserRepository`

**Files:**
- Create: `src/Travility.Data/Repositories/UserRepository.cs`
- Create: `src/Travility.Data/Repositories/PlaceRepository.cs`
- Create: `src/Travility.Data/Repositories/TripRepository.cs`
- Create: `src/Travility.Data/Repositories/BudgetRepository.cs`
- Create: `src/Travility.Data/TravilityDataSession.cs`
- Create: `src/Travility.Data/TravilityDataSessionFactory.cs`
- Create: `database/user_repository_smoke.sql`

**Interfaces:**
- Consumes: `ITravilityDataSession`, `IUserRepository`, `TravilityEntities`.
- Produces: Concrete data access cho `AuthenticationService` và composition root.

- [x] **Step 1: Viết SQL fixture/smoke cho lookup**

```sql
USE TravilityDev;
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE NormalizedUsername = N'ADMIN')
    THROW 51100, 'Admin username lookup failed', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE NormalizedEmail = N'ADMIN@TRAVILITY.LOCAL')
    THROW 51101, 'Admin email lookup failed', 1;
```

- [x] **Step 2: Implement `UserRepository` không commit**

```csharp
public sealed class UserRepository : IUserRepository
{
    private readonly TravilityEntities _context;
    public UserRepository(TravilityEntities context) { _context = context; }

    public User GetById(int id) { return _context.Users.SingleOrDefault(x => x.UserId == id); }
    public User GetByNormalizedUsername(string value) { return _context.Users.SingleOrDefault(x => x.NormalizedUsername == value); }
    public User GetByNormalizedEmail(string value) { return _context.Users.SingleOrDefault(x => x.NormalizedEmail == value); }
    public bool UsernameExists(string value) { return _context.Users.Any(x => x.NormalizedUsername == value); }
    public bool EmailExists(string value) { return _context.Users.Any(x => x.NormalizedEmail == value); }
    public Role GetRoleByName(string value) { return _context.Roles.SingleOrDefault(x => x.Name == value); }
    public void Add(User user) { _context.Users.Add(user); }
    public void AddProfile(UserProfile profile) { _context.UserProfiles.Add(profile); }
    public void AddWallet(TravelWallet wallet) { _context.TravelWallets.Add(wallet); }
    public void Update(User user) { _context.Entry(user).State = EntityState.Modified; }
}
```

- [x] **Step 3: Implement ba repository đọc tối thiểu**

`PlaceRepository`, `TripRepository`, `BudgetRepository` implement đúng toàn bộ
chữ ký Task 6 bằng LINQ trên cùng `TravilityEntities`. Các phương thức `Add` và
`Update` chỉ thay đổi tracking state, không gọi `SaveChanges()`.

- [x] **Step 4: Implement session transaction state**

`BeginTransaction()` phải từ chối begin lần hai. `Commit()` yêu cầu transaction đang mở; commit rồi dispose transaction. `Rollback()` an toàn khi transaction đã mở. `Dispose()` rollback transaction chưa commit rồi dispose context.

```csharp
public int SaveChanges() { return _context.SaveChanges(); }
```

Factory tạo `new TravilityEntities()` cho mỗi `Create()`.

- [x] **Step 5: Build và chạy SQL smoke**

```powershell
msbuild Travility.sln /m /p:Configuration=Debug
sqlcmd -S .\SQLEXPRESS -E -b -i database\user_repository_smoke.sql
```

Expected: build và SQL smoke thành công.

- [x] **Step 6: Commit**

```bash
git add src/Travility.Data database/user_repository_smoke.sql
git commit -m "feat(data): implement user data session"
```

---

### Task 10: Identifier normalization và `AuthenticationService` test-first

**Files:**
- Create: `src/Travility.Core/Security/LoginIdentifier.cs`
- Create: `src/Travility.Core/Security/LoginIdentifierNormalizer.cs`
- Create: `src/Travility.Core/Services/AuthenticationService.cs`
- Create: `tests/Travility.Tests/Security/LoginIdentifierNormalizerTests.cs`
- Create: `tests/Travility.Tests/Authentication/AuthenticationServiceTests.cs`
- Create: `tests/Travility.Tests/TestData/FakeTravilityDataSession.cs`
- Create: `tests/Travility.Tests/TestData/FakeUserRepository.cs`
- Create: `tests/Travility.Tests/TestData/AuthFixture.cs`

**Interfaces:**
- Consumes: Auth/Data contracts và password hasher.
- Produces: Register, login, temporary-password và change-password use cases.

- [ ] **Step 1: Viết identifier tests**

```csharp
[TestCase("Thanh", IdentifierKind.Username, "THANH")]
[TestCase("Thanh@Example.Com", IdentifierKind.Email, "THANH@EXAMPLE.COM")]
public void Phai_PhanLoaiVaChuanHoa_Khi_NhanIdentifier(string input, IdentifierKind kind, string normalized)
{
    var result = LoginIdentifierNormalizer.Normalize(input);
    Assert.That(result.Kind, Is.EqualTo(kind));
    Assert.That(result.NormalizedValue, Is.EqualTo(normalized));
}

[Test]
public void Phai_TuChoi_Khi_UsernameDangKyChuaKyTuAt()
{
    Assert.Throws<ArgumentException>(() => LoginIdentifierNormalizer.NormalizeUsername("abc@def"));
}
```

- [ ] **Step 2: Viết Auth tests bằng fake session**

Bao phủ tối thiểu:

```text
Register tạo Traveler, normalized identifiers và hash; commit đúng một lần.
Register trùng username/email trả đúng error code và không commit.
Login username và email trả cùng user.
Login sai password không phân biệt user không tồn tại.
Login account disabled trả AccountDisabled.
Traveler không đặt được temporary password.
Admin đặt temporary password và bật MustChangePassword.
ChangePassword đúng mật khẩu cập nhật hash và tắt MustChangePassword.
```

Một test mẫu:

```csharp
[Test]
public void Phai_DangNhapBangEmail_Khi_EmailVaMatKhauDung()
{
    var fixture = AuthFixture.CreateTraveler("thanh", "thanh@example.com", "MatKhau@123");
    var result = fixture.Service.Login("THANH@example.com", "MatKhau@123");

    Assert.That(result.Succeeded, Is.True);
    Assert.That(result.User.Username, Is.EqualTo("thanh"));
}
```

Viết các test còn lại bằng cùng fixture:

```csharp
[Test]
public void Phai_TaoTravelerVaCommitMotLan_Khi_DangKyHopLe()
{
    var fixture = AuthFixture.Empty();
    var result = fixture.Service.Register(new RegistrationRequest
    {
        Username = "Thanh",
        Email = "Thanh@Example.com",
        DisplayName = "Đỗ Chí Thành",
        Password = "MatKhau@123"
    });

    Assert.That(result.Succeeded, Is.True);
    Assert.That(fixture.Users.Items.Single().Role.Name, Is.EqualTo("Traveler"));
    Assert.That(fixture.Users.Items.Single().NormalizedUsername, Is.EqualTo("THANH"));
    Assert.That(fixture.Session.SaveCount, Is.EqualTo(1));
    Assert.That(fixture.Session.CommitCount, Is.EqualTo(1));
}

[TestCase(true, false, AuthenticationErrorCode.UsernameAlreadyExists)]
[TestCase(false, true, AuthenticationErrorCode.EmailAlreadyExists)]
public void Phai_KhongCommit_Khi_DinhDanhBiTrung(
    bool duplicateUsername,
    bool duplicateEmail,
    AuthenticationErrorCode expected)
{
    var fixture = AuthFixture.WithDuplicates(duplicateUsername, duplicateEmail);
    var result = fixture.Service.Register(AuthFixture.ValidRegistration());
    Assert.That(result.ErrorCode, Is.EqualTo(expected));
    Assert.That(fixture.Session.CommitCount, Is.Zero);
}

[Test]
public void Phai_TuChoi_Khi_TaiKhoanBiVoHieuHoa()
{
    var fixture = AuthFixture.CreateTraveler("thanh", "thanh@example.com", "MatKhau@123", isActive: false);
    var result = fixture.Service.Login("thanh", "MatKhau@123");
    Assert.That(result.ErrorCode, Is.EqualTo(AuthenticationErrorCode.AccountDisabled));
}

[Test]
public void Phai_CamTraveler_Khi_DatMatKhauTam()
{
    var fixture = AuthFixture.WithActorAndTarget(actorRole: "Traveler");
    var result = fixture.Service.SetTemporaryPassword(fixture.ActorId, fixture.TargetId, "TamThoi@123");
    Assert.That(result.ErrorCode, Is.EqualTo(AuthenticationErrorCode.Forbidden));
}

[Test]
public void Phai_BatDoiMatKhau_Khi_AdminDatMatKhauTam()
{
    var fixture = AuthFixture.WithActorAndTarget(actorRole: "Admin");
    var result = fixture.Service.SetTemporaryPassword(fixture.ActorId, fixture.TargetId, "TamThoi@123");
    Assert.That(result.Succeeded, Is.True);
    Assert.That(fixture.Users.GetById(fixture.TargetId).MustChangePassword, Is.True);
}

[Test]
public void Phai_TatCoDoiMatKhau_Khi_DoiMatKhauThanhCong()
{
    var fixture = AuthFixture.CreateTraveler("thanh", "thanh@example.com", "MatKhau@123", mustChangePassword: true);
    var result = fixture.Service.ChangePassword(fixture.TargetId, "MatKhau@123", "MatKhauMoi@123");
    Assert.That(result.Succeeded, Is.True);
    Assert.That(fixture.Users.GetById(fixture.TargetId).MustChangePassword, Is.False);
}
```

`AuthFixture` tạo `FakeUserRepository`, `FakeTravilityDataSession`,
`Pbkdf2PasswordHasher` và `AuthenticationService`; không đọc DB và không dùng
mocking framework.

- [ ] **Step 3: Chạy tests để xác nhận RED**

```powershell
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll /TestCaseFilter:"FullyQualifiedName~Security|FullyQualifiedName~AuthenticationServiceTests"
```

Expected: FAIL vì normalizer/service chưa tồn tại.

- [ ] **Step 4: Implement normalizer**

Trim username/email nhưng không trim password. Dùng `ToUpperInvariant()` cho normalized identifiers. Username dài 3–50, không chứa `@`; email dùng `MailAddress` và giới hạn 254 ký tự.

- [ ] **Step 5: Implement Auth transaction flow**

Register:

```text
validate → normalize → create session → begin transaction
→ check duplicate → hash → create User(Role Traveler)
→ create UserProfile + TravelWallet(Balance = 0) → save once → commit → return success
```

Login là read-only, không gọi `BeginTransaction()`. Mọi invalid credential trả cùng thông báo bên UI; service vẫn dùng error code nội bộ.

Set temporary/change password mở transaction, cập nhật cả hash/salt/iterations/algorithm và `UpdatedAtUtc`.

- [ ] **Step 6: Chạy Auth tests**

Expected: toàn bộ PASS; fake session xác nhận số lần begin/save/commit/rollback.

- [ ] **Step 7: Commit**

```bash
git add src/Travility.Core/Security src/Travility.Core/Services tests/Travility.Tests
git commit -m "feat(auth): implement registration va login"
```

---

### Task 11: `UserSession` và navigation policy test-first

**Files:**
- Create: `src/Travility.Core/Session/UserSession.cs`
- Create: `src/Travility.Core/Session/NavigationItem.cs`
- Create: `src/Travility.Core/Session/NavigationPolicy.cs`
- Create: `tests/Travility.Tests/Session/NavigationPolicyTests.cs`

**Interfaces:**
- Consumes: `AuthenticatedUser` từ Task 7/10.
- Produces: Session immutable và menu role-based cho `MainForm`.

- [ ] **Step 1: Viết failing navigation tests**

```csharp
[Test]
public void Phai_AnQuanTri_Khi_LaTraveler()
{
    var items = NavigationPolicy.ForRole("Traveler").Select(x => x.Key).ToArray();
    CollectionAssert.Contains(items, "Map");
    CollectionAssert.DoesNotContain(items, "Users");
}

[Test]
public void Phai_HienQuanTri_Khi_LaAdmin()
{
    var items = NavigationPolicy.ForRole("Admin").Select(x => x.Key).ToArray();
    CollectionAssert.Contains(items, "Users");
    CollectionAssert.DoesNotContain(items, "Trips");
}
```

- [ ] **Step 2: Implement immutable session và exact menu keys**

Traveler keys: `Home`, `Map`, `Trips`, `Itineraries`, `Budget`, `CheckIn`, `Passport`; `Assistant` chỉ thêm khi feature flag bật.

Admin keys: `Home`, `Places`, `Users`, `TransportOptions`, `Achievements`, `Statistics`.

`UserSession` chỉ có get-only properties và không chứa EF entity.

- [ ] **Step 3: Chạy tests và commit**

```powershell
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll /TestCaseFilter:"FullyQualifiedName~NavigationPolicyTests"
```

```bash
git add src/Travility.Core/Session tests/Travility.Tests/Session
git commit -m "feat(shell): them user session va navigation policy"
```

---

### Task 12: File logging và global error IDs

**Files:**
- Create: `src/Travility.WinForms/Infrastructure/FileAppLogger.cs`
- Create: `src/Travility.WinForms/Infrastructure/GlobalExceptionHandler.cs`
- Create: `tests/Travility.Tests/Infrastructure/FileAppLoggerTests.cs`
- Create: `tests/Travility.Tests/TestData/TempDirectory.cs`
- Modify: `src/Travility.WinForms/Program.cs`

**Interfaces:**
- Consumes: `IAppLogger` từ Task 7.
- Produces: Thread-safe daily logs dưới `%LocalAppData%\Travility\Logs` và safe error dialog.

- [ ] **Step 1: Viết logger tests với temp directory**

Tạo helper xác định và tự dọn:

```csharp
public sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "TravilityTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public void Dispose()
    {
        if (Directory.Exists(Path)) Directory.Delete(Path, recursive: true);
    }
}
```

```csharp
[Test]
public void Phai_GhiErrorIdVaException_Khi_LogError()
{
    using (var temp = new TempDirectory())
    {
        var logger = new FileAppLogger(temp.Path, retentionDays: 14);
        logger.Error("Auth", "ERR-1234", new InvalidOperationException("boom"));
        var text = File.ReadAllText(Directory.GetFiles(temp.Path).Single());
        StringAssert.Contains("ERR-1234", text);
        StringAssert.Contains("InvalidOperationException", text);
    }
}
```

Thêm hai test:

```csharp
[Test]
public void Phai_KhongMatDong_Khi_GhiDongThoi()
{
    using (var temp = new TempDirectory())
    {
        var logger = new FileAppLogger(temp.Path, retentionDays: 14);
        Parallel.For(0, 20, i => logger.Info("Parallel", "line-" + i));
        var lines = File.ReadAllLines(Directory.GetFiles(temp.Path).Single());
        Assert.That(lines.Length, Is.EqualTo(20));
    }
}

[Test]
public void Phai_XoaLogCu_Khi_QuaHanMuoiBonNgay()
{
    using (var temp = new TempDirectory())
    {
        var oldFile = System.IO.Path.Combine(temp.Path, "travility-20260101.log");
        File.WriteAllText(oldFile, "old");
        File.SetLastWriteTimeUtc(oldFile, DateTime.UtcNow.AddDays(-15));
        var logger = new FileAppLogger(temp.Path, retentionDays: 14);
        logger.Info("Test", "new");
        Assert.That(File.Exists(oldFile), Is.False);
    }
}
```

- [ ] **Step 2: Chạy RED rồi implement logger**

Mỗi dòng log có `UtcTimestamp|Severity|Component|ErrorId|Message`. Dùng một private lock quanh append. Không nhận hoặc ghi password, hash, salt, API key, connection string.

- [ ] **Step 3: Implement global handler**

Error ID format: `yyyyMMdd-HHmmss-` + 4 ký tự hex ngẫu nhiên. Handler log exception rồi hiện:

```text
Ứng dụng gặp lỗi ngoài dự kiến. Vui lòng thử lại.
Mã lỗi: <error-id>
```

Đăng ký cả `Application.ThreadException` và `AppDomain.CurrentDomain.UnhandledException` trong `Program.Main`.

- [ ] **Step 4: Chạy tests và commit**

```bash
git add src/Travility.WinForms/Infrastructure src/Travility.WinForms/Program.cs tests/Travility.Tests/Infrastructure
git commit -m "feat(logging): ghi log va xu ly loi toan cuc"
```

---

### Task 13: Auth Forms và forced password change

**Files:**
- Create: `src/Travility.WinForms/Auth/LoginForm.cs`
- Create: `src/Travility.WinForms/Auth/LoginForm.Designer.cs`
- Create: `src/Travility.WinForms/Auth/RegisterForm.cs`
- Create: `src/Travility.WinForms/Auth/RegisterForm.Designer.cs`
- Create: `src/Travility.WinForms/Auth/ChangePasswordDialog.cs`
- Create: `src/Travility.WinForms/Auth/ChangePasswordDialog.Designer.cs`

**Interfaces:**
- Consumes: `IAuthenticationService`, `AuthenticationResult`, `RegistrationResult`, `AuthenticatedUser`.
- Produces: `LoginSucceeded` event mang `AuthenticatedUser` cho composition root.

- [ ] **Step 1: Dựng forms theo UI rules**

Mỗi Form dùng `TableLayoutPanel`, Segoe UI 9pt, `AutoScaleMode = Dpi`, `ErrorProvider`. Control names:

```text
Login: txtIdentifier, txtPassword, btnLogin, lnkRegister
Register: txtUsername, txtEmail, txtDisplayName, txtPassword, txtConfirmPassword, btnRegister
Change: txtCurrentPassword, txtNewPassword, txtConfirmPassword, btnChangePassword
```

- [ ] **Step 2: Implement validation cục bộ**

Rỗng, password confirmation và username chứa `@` được chặn trước service. Không dùng `MessageBox` cho validation. Disable submit button và bật wait cursor trong service call; khôi phục trong `finally`.

- [ ] **Step 3: Ánh xạ error code sang tiếng Việt**

```text
InvalidCredentials → Tên đăng nhập/email hoặc mật khẩu không đúng.
AccountDisabled → Tài khoản đã bị vô hiệu hóa. Hãy liên hệ quản trị viên.
UsernameAlreadyExists → Tên đăng nhập đã được sử dụng.
EmailAlreadyExists → Email đã được sử dụng.
Forbidden → Bạn không có quyền thực hiện thao tác này.
```

Không hiển thị exception message hoặc stack trace.

- [ ] **Step 4: Implement forced change flow**

Khi login thành công nhưng `MustChangePassword = true`, mở `ChangePasswordDialog`; chỉ phát `LoginSucceeded` sau khi đổi thành công. Hủy dialog quay lại Login.

- [ ] **Step 5: Manual UI verification**

Chạy với fake service hoặc DB thật và xác nhận: tab order đúng; Enter submit; nút khóa khi xử lý; ErrorProvider đúng control; DPI 100% và 150% không cắt chữ.

- [ ] **Step 6: Commit**

```bash
git add src/Travility.WinForms/Auth
git commit -m "feat(auth): them giao dien dang ky dang nhap"
```

---

### Task 14: `MainForm`, page cache và logout

**Files:**
- Create: `src/Travility.WinForms/Shell/MainForm.cs`
- Create: `src/Travility.WinForms/Shell/MainForm.Designer.cs`
- Create: `src/Travility.WinForms/Pages/HomePage.cs`
- Create: `src/Travility.WinForms/Pages/HomePage.Designer.cs`
- Create: `src/Travility.WinForms/Shell/PageFactory.cs`

**Interfaces:**
- Consumes: `UserSession`, `NavigationPolicy`, `IAppLogger`.
- Produces: Role-aware shell, lazy page cache và `LogoutRequested` event.

- [ ] **Step 1: Dựng MainForm layout**

Default 1280×800, resizable, sidebar 220px, header 56px, fill content panel và status strip. Màu dùng đúng `docs/05-ui-guidelines.md`; không thêm màu tùy ý.

- [ ] **Step 2: Render navigation từ policy**

Không hardcode hai menu riêng trong Designer. Tạo button từ `NavigationPolicy.ForRole(session.Role)`. Mỗi button giữ `NavigationItem.Key` trong `Tag`.

- [ ] **Step 3: Implement lazy page cache**

```csharp
private readonly Dictionary<string, UserControl> _pages =
    new Dictionary<string, UserControl>(StringComparer.OrdinalIgnoreCase);

private UserControl GetOrCreatePage(string key)
{
    UserControl page;
    if (_pages.TryGetValue(key, out page)) return page;
    page = _pageFactory.Create(key);
    _pages.Add(key, page);
    return page;
}
```

Foundation `PageFactory` chỉ tạo `HomePage`. Các navigation button khác vẫn
hiện đúng theo role nhưng bị disable và có tooltip “Chưa khả dụng trong bản
Foundation”; không tạo page/service/engine giả.

- [ ] **Step 4: Implement logout/dispose**

Nút Logout phát `LogoutRequested`. `MainForm.Dispose()` dispose từng cached page, clear dictionary, rồi composition root mở `LoginForm` mới; không tái sử dụng session.

- [ ] **Step 5: Manual role verification**

Login bằng hai tài khoản demo. Xác nhận Traveler không thấy `Users`; Admin không thấy `Trips`; HomePage chỉ được tạo một lần khi mở lặp; logout quay về Login.

- [ ] **Step 6: Commit**

```bash
git add src/Travility.WinForms/Shell src/Travility.WinForms/Pages
git commit -m "feat(shell): them main form va dieu huong theo role"
```

---

### Task 15: Haversine × 1.3 test-first

**Files:**
- Create: `src/Travility.Core/Engines/RoutingEngine.cs`
- Create: `tests/Travility.Tests/Engines/RoutingEngineTests.cs`
- Create: `tests/Travility.Tests/TestData/TestPlaceBuilder.cs`

**Interfaces:**
- Consumes: EF `Place` entity và chữ ký `Distance` đã chốt trong `IRoutingEngine`.
- Produces: `double RoutingEngine.Distance(Place a, Place b)` dùng chung cho Routing và CheckIn.

- [ ] **Step 1: Viết failing tests**

```csharp
[Test]
public void Phai_BangKhong_Khi_HaiToaDoTrungNhau()
{
    var place = TestPlaceBuilder.Named("Cầu Rồng").At(16.0611, 108.2275).Build();
    Assert.That(_engine.Distance(place, place), Is.EqualTo(0d).Within(0.000001));
}

[Test]
public void Phai_DoiXung_Khi_DoiThuTuDiaDiem()
{
    var museum = TestPlaceBuilder.Named("Bảo tàng Chăm").At(16.0599, 108.2235).Build();
    var beach = TestPlaceBuilder.Named("Biển Mỹ Khê").At(16.0544, 108.2428).Build();
    Assert.That(_engine.Distance(museum, beach), Is.EqualTo(_engine.Distance(beach, museum)).Within(0.000001));
}
```

Thêm test giá trị tham chiếu và null arguments:

```csharp
[Test]
public void Phai_XapXiHaiPhayBayChinSauKm_Khi_DiTuBaoTangChamDenMyKhe()
{
    var museum = TestPlaceBuilder.Named("Bảo tàng Chăm").At(16.0599, 108.2235).Build();
    var beach = TestPlaceBuilder.Named("Biển Mỹ Khê").At(16.0544, 108.2428).Build();
    Assert.That(_engine.Distance(museum, beach), Is.EqualTo(2.79644d).Within(0.001d));
}

[Test]
public void Phai_NemArgumentNull_Khi_ThieuDiaDiem()
{
    var place = TestPlaceBuilder.Named("Cầu Rồng").At(16.0611, 108.2275).Build();
    Assert.Throws<ArgumentNullException>(() => _engine.Distance(null, place));
    Assert.Throws<ArgumentNullException>(() => _engine.Distance(place, null));
}
```

- [ ] **Step 2: Chạy RED**

Expected: FAIL vì `RoutingEngine` chưa tồn tại.

- [ ] **Step 3: Implement công thức**

Trong Foundation, `RoutingEngine` cung cấp phép tính `Distance` nhưng chưa khai
báo `: IRoutingEngine`, vì `Optimise` nằm ngoài phạm vi và không được phép thêm
implementation giả chỉ để compile. Khi module Routing triển khai đủ
`Optimise`/`BuildMatrix`, class mới nhận interface đã chốt.

```csharp
private const double EarthRadiusKm = 6371.0088;
private const double DetourFactor = 1.3;

public double Distance(Place a, Place b)
{
    if (a == null) throw new ArgumentNullException(nameof(a));
    if (b == null) throw new ArgumentNullException(nameof(b));
    var lat1 = DegreesToRadians(a.Latitude);
    var lat2 = DegreesToRadians(b.Latitude);
    var deltaLat = lat2 - lat1;
    var deltaLon = DegreesToRadians(b.Longitude - a.Longitude);
    var h = Math.Pow(Math.Sin(deltaLat / 2d), 2d)
          + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(deltaLon / 2d), 2d);
    var haversineKm = EarthRadiusKm * 2d * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1d - h));
    return haversineKm * DetourFactor;
}
```

Không gọi API, DB hoặc WinForms.

- [ ] **Step 4: Chạy tests và commit**

```powershell
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll /TestCaseFilter:"FullyQualifiedName~RoutingEngineTests"
```

```bash
git add src/Travility.Core/Engines tests/Travility.Tests/Engines tests/Travility.Tests/TestData
git commit -m "feat(routing): them haversine voi detour factor"
```

---

### Task 16: Composition root, DB integration và nghiệm thu Cổng 2

**Files:**
- Modify: `src/Travility.WinForms/Program.cs`
- Create: `src/Travility.WinForms/TravilityApplicationContext.cs`
- Modify: `src/Travility.WinForms/App.config`
- Modify: `database/smoke_test.sql`
- Modify: `README.md`
- Modify: `docs/12-testing-strategy.md`

**Interfaces:**
- Consumes: Concrete Data, Auth, logger, session, forms và Haversine.
- Produces: Walking skeleton chạy thật và hướng dẫn setup có thể lặp lại.

- [ ] **Step 1: Cấu hình connection string**

```xml
<connectionStrings>
  <add name="TravilityEntities"
       connectionString="metadata=res://*/Model.TravilityModel.csdl|res://*/Model.TravilityModel.ssdl|res://*/Model.TravilityModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=.\SQLEXPRESS;initial catalog=TravilityDev;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;"
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

- [ ] **Step 2: Wire composition root**

Trong `Program.Main`:

```text
FileAppLogger
→ TravilityDataSessionFactory
→ Pbkdf2PasswordHasher(600000)
→ AuthenticationService
→ LoginForm
→ UserSession từ AuthenticatedUser
→ MainForm
```

`TravilityApplicationContext` kế thừa `ApplicationContext`, giữ đúng một Form
hiện tại. Khi nhận `LoginSucceeded`, nó đóng Login, tạo `UserSession`, mở
MainForm và đăng ký `LogoutRequested`. Khi logout, nó dispose MainForm/session
reference rồi tạo LoginForm mới. `Program.Main` chỉ gọi một lần:

```csharp
Application.Run(new TravilityApplicationContext(
    authenticationService,
    logger));
```

- [ ] **Step 3: Chạy database integration scenarios**

Trên database vừa reset:

```text
1. Login admin bằng username.
2. Logout; login admin bằng email.
3. Login sai password và kiểm tra thông báo chung.
4. Đăng ký Traveler mới; xác nhận normalized username/email trong SQL.
5. Thử đăng ký trùng username và trùng email.
6. Set IsActive = 0 bằng SQL; xác nhận login bị từ chối.
7. Set MustChangePassword = 1; xác nhận bắt buộc đổi trước MainForm.
8. Login Admin/Traveler và xác nhận menu khác nhau.
9. Logout và xác nhận session/form cũ đã dispose.
```

- [ ] **Step 4: Kiểm tra lỗi SQL và logging**

Dừng service SQL Server Express, thử login, xác nhận UI hiện hướng dẫn an toàn và file `%LocalAppData%\Travility\Logs\travility-yyyyMMdd.log` chứa error ID + exception nhưng không chứa password/connection string. Khởi động lại SQL Server.

- [ ] **Step 5: Chạy toàn bộ verification**

```powershell
nuget restore Travility.sln
msbuild Travility.sln /m /p:Configuration=Debug
vstest.console.exe tests\Travility.Tests\bin\Debug\Travility.Tests.dll
msbuild Travility.sln /m /p:Configuration=Release
sqlcmd -S .\SQLEXPRESS -E -b -i database\smoke_test.sql
```

Expected: Debug/Release build PASS, toàn bộ NUnit PASS, smoke test exit 0.

- [ ] **Step 6: Cập nhật tài liệu chạy dự án**

README ghi đúng thứ tự script, prerequisites, hai tài khoản demo, lệnh build/test và vị trí log. `docs/12-testing-strategy.md` chốt NUnit 3 thay cho “NUnit hoặc xUnit”.

- [ ] **Step 7: Kiểm tra diff và commit integration**

```bash
git status --short
git diff --check
git add src/Travility.WinForms database/smoke_test.sql README.md docs/12-testing-strategy.md
git commit -m "feat: hoan thanh foundation walking skeleton"
```

- [ ] **Step 8: Chuẩn bị PR nghiệm thu**

PR phải ghi chính xác kết quả Debug, Release, NUnit, SQL smoke, hai DPI đã thử và thời gian PBKDF2 benchmark. Không tick mục chưa thực hiện.

---

## Dependency Order và Parallel Work

```text
Task 1
├── Task 2
└── Task 3 → Task 4 → Task 5 → Task 6 → Task 7 → Task 8   [Cổng 1]
                                      ├── Task 9 → Task 10 → Task 11
                                      ├── Task 12 → Task 13 → Task 14
                                      └── Task 15                           [Quân]
                                                    └── Task 16             [Cổng 2]
```

- Tasks 1–8: Thành thực hiện tuần tự, merge baseline một lần.
- Sau Task 8, Task 15 có thể giao Quân trên nhánh riêng.
- Tasks 9–11 phải xong trước khi chạy Auth Forms bằng DB thật.
- Task 12 có thể làm song song Task 9–11; Tasks 13–14 chạy sau contracts/session.
- Task 16 chỉ bắt đầu khi Tasks 9–15 đã merge và test riêng đều xanh.

## Completion Definition

- Cổng 1 và Cổng 2 đều đạt mọi tiêu chí trong spec §§13.1–13.2.
- Không còn sự khác nhau giữa docs/rules và code về NUnit hoặc PBKDF2.
- `main` chỉ nhận thay đổi qua PR từ `develop-<tên-thành-viên>` và chủ dự án approve.
- Foundation kết thúc trước khi tích hợp Map, Trip, Budget, CheckIn hoặc chatbot.
