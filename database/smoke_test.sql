:ON ERROR EXIT
USE [TravilityDev];
GO
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF DB_NAME() COLLATE Latin1_General_100_BIN2 <> N'TravilityDev'
   OR DATALENGTH(DB_NAME()) <> DATALENGTH(N'TravilityDev')
    THROW 51000, 'Smoke tests must run against TravilityDev.', 1;

-- 1. Kiểm tra tất cả 34 bảng trong Schema v1
DECLARE @RequiredTables TABLE (Name sysname NOT NULL);
INSERT INTO @RequiredTables (Name) VALUES
    (N'Roles'), (N'Users'), (N'UserProfiles'), (N'UserPreferences'), (N'TravelWallets'),
    (N'PlaceCategories'), (N'PlaceTypes'), (N'Places'), (N'PlaceImages'),
    (N'PlaceRatings'), (N'PlaceReviews'), (N'ExternalPlaceMappings'),
    (N'Trips'), (N'TripMembers'), (N'TripPreferences'), (N'TransportOptions'),
    (N'Bookings'), (N'TripPlaces'),
    (N'Itineraries'), (N'ItineraryDays'), (N'ItineraryItems'), (N'RoutePlans'),
    (N'RouteSegments'), (N'DistanceMatrix'),
    (N'BudgetCategories'), (N'TripBudgets'), (N'BudgetAllocations'), (N'Expenses'),
    (N'CheckIns'), (N'Achievements'), (N'UserAchievements'),
    (N'ChatSessions'), (N'ChatMessages'), (N'ToolExecutions');

IF EXISTS (
    SELECT 1 FROM @RequiredTables r
    WHERE OBJECT_ID(N'dbo.' + r.Name, N'U') IS NULL
)
BEGIN
    DECLARE @MissingList nvarchar(max);
    SELECT @MissingList = STRING_AGG(r.Name, N', ')
    FROM @RequiredTables r
    WHERE OBJECT_ID(N'dbo.' + r.Name, N'U') IS NULL;
    
    DECLARE @ErrMsg nvarchar(2048) = N'Missing domain tables: ' + @MissingList;
    THROW 51015, @ErrMsg, 1;
END;

-- Bảng TravelPassports cố ý KHÔNG có trong schema (tính bằng LINQ từ CheckIns & Trips)
IF OBJECT_ID(N'dbo.TravelPassports', N'U') IS NOT NULL
    THROW 51016, 'TravelPassports table is forbidden; it is a calculated view.', 1;

-- 2. Kiểm tra type và stored procedures
IF TYPE_ID(N'dbo.IntIdList') IS NULL
    THROW 51017, 'Missing table type dbo.IntIdList.', 1;

IF OBJECT_ID(N'dbo.sp_GetDistanceMatrix', N'P') IS NULL
    THROW 51018, 'Missing stored procedure dbo.sp_GetDistanceMatrix.', 1;
IF OBJECT_ID(N'dbo.sp_TopPlacesAddedToTrips', N'P') IS NULL
    THROW 51019, 'Missing stored procedure dbo.sp_TopPlacesAddedToTrips.', 1;
IF OBJECT_ID(N'dbo.sp_AvgExpenseByCategory', N'P') IS NULL
    THROW 51020, 'Missing stored procedure dbo.sp_AvgExpenseByCategory.', 1;
GO

-- 3. Kiểm tra dữ liệu Identity & Security
IF COL_LENGTH(N'dbo.Users', N'Password') IS NOT NULL
    THROW 51006, 'Plaintext Password column is forbidden.', 1;

IF (SELECT COUNT(*) FROM dbo.Roles WHERE Name IN (N'Admin', N'Traveler') AND IsActive = 1) <> 2
    THROW 51007, 'Roles seed is invalid.', 1;

IF (SELECT COUNT(*)
    FROM dbo.Users AS u
    INNER JOIN dbo.Roles AS r ON r.RoleId = u.RoleId
    WHERE u.IsActive = 1
      AND ((u.Username = N'admin' AND u.NormalizedUsername = N'ADMIN'
            AND u.Email = N'admin@travility.local' AND u.NormalizedEmail = N'ADMIN@TRAVILITY.LOCAL'
            AND r.Name = N'Admin')
        OR (u.Username = N'traveler' AND u.NormalizedUsername = N'TRAVELER'
            AND u.Email = N'traveler@travility.local' AND u.NormalizedEmail = N'TRAVELER@TRAVILITY.LOCAL'
            AND r.Name = N'Traveler'))) <> 2
    THROW 51008, 'Demo users or role mappings are invalid.', 1;

IF EXISTS (SELECT 1 FROM dbo.Users
           WHERE DATALENGTH(PasswordHash) <> 32 OR DATALENGTH(PasswordSalt) <> 16
              OR PasswordIterations NOT BETWEEN 1 AND 1200000
              OR PasswordAlgorithm COLLATE Latin1_General_100_BIN2 <> N'PBKDF2-HMAC-SHA256'
              OR DATALENGTH(PasswordAlgorithm) <> DATALENGTH(N'PBKDF2-HMAC-SHA256'))
    THROW 51009, 'Stored password metadata is incompatible with the password hasher.', 1;

IF EXISTS (SELECT 1 FROM dbo.Users AS u
           WHERE u.NormalizedUsername IN (N'ADMIN', N'TRAVELER')
             AND (NOT EXISTS (SELECT 1 FROM dbo.UserProfiles AS p WHERE p.UserId = u.UserId)
               OR NOT EXISTS (SELECT 1 FROM dbo.UserPreferences AS p WHERE p.UserId = u.UserId)
               OR NOT EXISTS (SELECT 1 FROM dbo.TravelWallets AS w WHERE w.UserId = u.UserId)))
    THROW 51010, 'A demo profile, preference or wallet is missing.', 1;

IF EXISTS (SELECT 1 FROM dbo.UserPreferences
           WHERE Beach NOT BETWEEN 1 AND 5 OR Food NOT BETWEEN 1 AND 5
              OR History NOT BETWEEN 1 AND 5 OR Culture NOT BETWEEN 1 AND 5
              OR Entertainment NOT BETWEEN 1 AND 5 OR Shopping NOT BETWEEN 1 AND 5
              OR Nature NOT BETWEEN 1 AND 5)
    THROW 51011, 'A user preference score is outside [1,5].', 1;

-- 4. Kiểm tra seed Reference Data: 8 BudgetCategories, 7 PlaceCategories, 5 Achievements
DECLARE @ExpectedBudgetCategories TABLE (Name nvarchar(50) NOT NULL);
INSERT INTO @ExpectedBudgetCategories (Name) VALUES
    (N'Transportation'), (N'Accommodation'), (N'Food'), (N'Activities'),
    (N'LocalTransport'), (N'Shopping'), (N'Reserve'), (N'Other');

IF (SELECT COUNT(*) FROM dbo.BudgetCategories bc
    INNER JOIN @ExpectedBudgetCategories ebc ON ebc.Name = bc.Name
    WHERE bc.IsActive = 1) <> 8
    THROW 51021, 'BudgetCategories seed is invalid or incomplete (must have 8 active categories).', 1;

DECLARE @ExpectedPlaceCategories TABLE (Name nvarchar(100) NOT NULL);
INSERT INTO @ExpectedPlaceCategories (Name) VALUES
    (N'Biển'), (N'Ẩm thực'), (N'Lịch sử'), (N'Văn hóa'),
    (N'Giải trí'), (N'Mua sắm'), (N'Thiên nhiên');

IF (SELECT COUNT(*) FROM dbo.PlaceCategories pc
    INNER JOIN @ExpectedPlaceCategories epc ON epc.Name = pc.Name
    WHERE pc.IsActive = 1) <> 7
    THROW 51022, 'PlaceCategories seed is invalid or incomplete (must have 7 preference categories).', 1;

IF (SELECT COUNT(*) FROM dbo.Achievements WHERE IsActive = 1) < 5
    THROW 51023, 'Achievements seed is incomplete (must have at least 5 achievements).', 1;

-- 5. Kiểm tra tính toàn vẹn Foreign Keys (phải enabled, trusted và NO ACTION)
IF EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE is_disabled = 1 OR is_not_trusted = 1
       OR delete_referential_action <> 0 OR update_referential_action <> 0
)
    THROW 51024, 'All foreign keys must be enabled, trusted and NO ACTION (delete_referential_action = 0).', 1;

-- 6. Kiểm tra các Unique Indexes cốt lõi
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.UserAchievements')
      AND is_unique = 1 AND is_disabled = 0
)
    THROW 51025, 'Unique index on UserAchievements(UserId, AchievementId) is missing.', 1;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Itineraries')
      AND name = N'UX_Itineraries_SelectedPerTrip'
      AND is_unique = 1 AND has_filter = 1 AND is_disabled = 0
)
    THROW 51026, 'Filtered unique index UX_Itineraries_SelectedPerTrip is missing.', 1;

-- 7. Kiểm tra các Check Constraints bắt buộc
DECLARE @RequiredConstraints TABLE (Name sysname NOT NULL);
INSERT INTO @RequiredConstraints (Name) VALUES
    (N'CK_Places_Latitude'), (N'CK_Places_Longitude'), (N'CK_Places_Rating'),
    (N'CK_Trips_Dates'), (N'CK_Trips_People'), (N'CK_DistanceMatrix_Order');

IF EXISTS (
    SELECT 1 FROM @RequiredConstraints rc
    WHERE NOT EXISTS (
        SELECT 1 FROM sys.check_constraints cc
        WHERE cc.name = rc.Name AND cc.is_disabled = 0 AND cc.is_not_trusted = 0
    )
)
    THROW 51027, 'One or more required check constraints are missing, disabled or untrusted.', 1;

PRINT N'Domain smoke tests passed; all 34 tables, procedures, seed and constraints are verified.';
GO
