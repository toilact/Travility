:ON ERROR EXIT
USE [master];
GO
SET NOCOUNT ON;

-- Lệnh phá huỷ chỉ dùng tên cố định; từ chối cả database trùng tên khác hoa/thường.
IF DB_ID(N'TravilityDev') IS NOT NULL
BEGIN
    IF DB_NAME(DB_ID(N'TravilityDev')) COLLATE Latin1_General_100_BIN2 <> N'TravilityDev'
       OR DATALENGTH(DB_NAME(DB_ID(N'TravilityDev'))) <> DATALENGTH(N'TravilityDev')
        THROW 51100, 'Reset refused: database name must be exactly TravilityDev.', 1;

    ALTER DATABASE [TravilityDev] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TravilityDev];
END;
GO
CREATE DATABASE [TravilityDev];
GO
