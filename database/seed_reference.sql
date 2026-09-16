:ON ERROR EXIT
USE [TravilityDev];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() COLLATE Latin1_General_100_BIN2 <> N'TravilityDev'
   OR DATALENGTH(DB_NAME()) <> DATALENGTH(N'TravilityDev')
    THROW 51300, 'Reference seed must run against TravilityDev.', 1;

-- Chỉ thêm role còn thiếu; không kích hoạt lại role mà quản trị viên đã vô hiệu hóa.
MERGE dbo.Roles WITH (HOLDLOCK) AS target
USING (VALUES
    (N'Admin', N'Quản trị hệ thống'),
    (N'Traveler', N'Người lập kế hoạch du lịch')
) AS source (Name, Description)
ON target.Name = source.Name
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Name, Description, IsActive)
    VALUES (source.Name, source.Description, 1);
GO
