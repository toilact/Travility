:ON ERROR EXIT
USE [TravilityDev];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF DB_NAME() COLLATE Latin1_General_100_BIN2 <> N'TravilityDev'
   OR DATALENGTH(DB_NAME()) <> DATALENGTH(N'TravilityDev')
    THROW 51500, 'Procedures must run against TravilityDev.', 1;

-- 1. Table type cho danh sách Id nguyên (dùng cho các stored procedure lọc tập ID)
IF TYPE_ID(N'dbo.IntIdList') IS NULL
BEGIN
    CREATE TYPE dbo.IntIdList AS TABLE
    (
        Id int NOT NULL PRIMARY KEY
    );
END;
GO

-- 2. sp_GetDistanceMatrix: Lấy ma trận khoảng cách cho danh sách PlaceId truyền vào
CREATE OR ALTER PROCEDURE dbo.sp_GetDistanceMatrix
    @PlaceIds dbo.IntIdList READONLY
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dm.DistanceMatrixId,
           dm.FromPlaceId,
           dm.ToPlaceId,
           dm.DistanceKm,
           dm.EstimatedTravelTimeMinutes,
           dm.CalculatedAtUtc
    FROM dbo.DistanceMatrix AS dm
    WHERE dm.FromPlaceId IN (SELECT Id FROM @PlaceIds)
      AND dm.ToPlaceId IN (SELECT Id FROM @PlaceIds);
END;
GO

-- 3. sp_TopPlacesAddedToTrips: Thống kê Admin - Top 20 địa điểm được thêm vào chuyến đi nhiều nhất
CREATE OR ALTER PROCEDURE dbo.sp_TopPlacesAddedToTrips
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (20)
           p.PlaceId,
           p.Name,
           COUNT_BIG(*) AS AddedCount
    FROM dbo.TripPlaces AS tp
    INNER JOIN dbo.Places AS p ON p.PlaceId = tp.PlaceId
    GROUP BY p.PlaceId, p.Name
    ORDER BY AddedCount DESC;
END;
GO

-- 4. sp_AvgExpenseByCategory: Thống kê Admin - Chi tiêu trung bình theo nhóm ngân sách
CREATE OR ALTER PROCEDURE dbo.sp_AvgExpenseByCategory
AS
BEGIN
    SET NOCOUNT ON;

    SELECT bc.BudgetCategoryId,
           bc.Name,
           AVG(CAST(e.Amount AS decimal(18,2))) AS AverageAmount
    FROM dbo.Expenses AS e
    INNER JOIN dbo.BudgetCategories AS bc ON bc.BudgetCategoryId = e.BudgetCategoryId
    GROUP BY bc.BudgetCategoryId, bc.Name;
END;
GO
