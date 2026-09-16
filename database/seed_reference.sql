:ON ERROR EXIT
USE [TravilityDev];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() COLLATE Latin1_General_100_BIN2 <> N'TravilityDev'
   OR DATALENGTH(DB_NAME()) <> DATALENGTH(N'TravilityDev')
    THROW 51300, 'Reference seed must run against TravilityDev.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    -- 1. Roles (chỉ thêm role còn thiếu)
    MERGE dbo.Roles WITH (HOLDLOCK) AS target
    USING (VALUES
        (N'Admin', N'Quản trị hệ thống'),
        (N'Traveler', N'Người lập kế hoạch du lịch')
    ) AS source (Name, Description)
    ON target.Name = source.Name
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Name, Description, IsActive)
        VALUES (source.Name, source.Description, 1);

    -- 2. BudgetCategories (đúng 8 nhóm ngân sách theo CONTEXT.md)
    MERGE dbo.BudgetCategories WITH (HOLDLOCK) AS target
    USING (VALUES
        (N'Transportation', 1),
        (N'Accommodation', 2),
        (N'Food', 3),
        (N'Activities', 4),
        (N'LocalTransport', 5),
        (N'Shopping', 6),
        (N'Reserve', 7),
        (N'Other', 8)
    ) AS source (Name, DisplayOrder)
    ON target.Name = source.Name
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Name, DisplayOrder, IsActive)
        VALUES (source.Name, source.DisplayOrder, 1);

    -- 3. PlaceCategories (đúng 7 nhóm sở thích theo CONTEXT.md)
    MERGE dbo.PlaceCategories WITH (HOLDLOCK) AS target
    USING (VALUES
        (N'Biển', N'beach'),
        (N'Ẩm thực', N'food'),
        (N'Lịch sử', N'history'),
        (N'Văn hóa', N'culture'),
        (N'Giải trí', N'entertainment'),
        (N'Mua sắm', N'shopping'),
        (N'Thiên nhiên', N'nature')
    ) AS source (Name, IconKey)
    ON target.Name = source.Name
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Name, IconKey, IsActive)
        VALUES (source.Name, source.IconKey, 1);

    -- 4. Achievements (5 huy hiệu cơ bản theo docs/09-checkin-gamification.md)
    DECLARE @FoodCatId int = (SELECT PlaceCategoryId FROM dbo.PlaceCategories WHERE Name = N'Ẩm thực');
    DECLARE @HistoryCatId int = (SELECT PlaceCategoryId FROM dbo.PlaceCategories WHERE Name = N'Lịch sử');

    MERGE dbo.Achievements WITH (HOLDLOCK) AS target
    USING (VALUES
        (N'FIRST_STEP', N'Khởi đầu vạn dặm', N'Check-in địa điểm đầu tiên', N'CheckInCount', 1, CAST(NULL AS int), N'assets/badges/first_step.png'),
        (N'FOODIE_DANANG', N'Tín đồ ẩm thực', N'Check-in 5 địa điểm ẩm thực', N'CategoryCheckIn', 5, @FoodCatId, N'assets/badges/foodie.png'),
        (N'HERITAGE_EXPLORER', N'Nhà thám hiểm di sản', N'Check-in 3 địa điểm lịch sử', N'CategoryCheckIn', 3, @HistoryCatId, N'assets/badges/heritage.png'),
        (N'BUDGET_MASTER', N'Bậc thầy ngân sách', N'Hoàn thành chuyến đi không vượt ngân sách', N'TripUnderBudget', 1, CAST(NULL AS int), N'assets/badges/budget_master.png'),
        (N'PASSPORT_PRO', N'Hộ chiếu vàng', N'Hoàn thành 3 chuyến đi', N'TripCount', 3, CAST(NULL AS int), N'assets/badges/passport_pro.png')
    ) AS source (Code, Name, Description, ConditionType, Threshold, CategoryId, BadgeImagePath)
    ON target.Code = source.Code
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Code, Name, Description, ConditionType, Threshold, CategoryId, BadgeImagePath, IsActive)
        VALUES (source.Code, source.Name, source.Description, source.ConditionType, source.Threshold, source.CategoryId, source.BadgeImagePath, 1);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
