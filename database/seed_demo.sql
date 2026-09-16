:ON ERROR EXIT
USE [TravilityDev];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() COLLATE Latin1_General_100_BIN2 <> N'TravilityDev'
   OR DATALENGTH(DB_NAME()) <> DATALENGTH(N'TravilityDev')
    THROW 51400, 'Demo seed must run against TravilityDev.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    IF (SELECT COUNT(*) FROM dbo.Roles WHERE Name IN (N'Admin', N'Traveler') AND IsActive = 1) <> 2
        THROW 51401, 'Run seed_reference.sql first; Admin and Traveler must be active.', 1;

    DECLARE @DemoUsers TABLE
    (
        Username nvarchar(50) NOT NULL,
        NormalizedUsername nvarchar(50) NOT NULL,
        Email nvarchar(254) NOT NULL,
        NormalizedEmail nvarchar(254) NOT NULL,
        RoleName nvarchar(50) NOT NULL,
        DisplayName nvarchar(100) NOT NULL,
        PasswordHash varbinary(32) NOT NULL,
        PasswordSalt varbinary(16) NOT NULL
    );

    -- PBKDF2-HMAC-SHA256: 600000 vòng, salt 16 byte, derived key 32 byte.
    -- Mật khẩu demo chỉ được ghi trong README nội bộ, không trong SQL.
    INSERT INTO @DemoUsers
        (Username, NormalizedUsername, Email, NormalizedEmail, RoleName, DisplayName, PasswordHash, PasswordSalt)
    VALUES
        (N'admin', N'ADMIN', N'admin@travility.local', N'ADMIN@TRAVILITY.LOCAL', N'Admin', N'Quản trị viên demo',
         0xA9041EADD01FFF29325410FE8A716DA3E86CC159B7AED88F6E1486B8B80D4D28,
         0x00112233445566778899AABBCCDDEEFF),
        (N'traveler', N'TRAVELER', N'traveler@travility.local', N'TRAVELER@TRAVILITY.LOCAL', N'Traveler', N'Du khách demo',
         0x71F29F3C959444A62FBD6478F40241CF151F66DC4F33D070A68A8B6EE1977919,
         0xFFEEDDCCBBAA99887766554433221100);

    -- Không ghi đè tài khoản khác hoặc nâng quyền một tài khoản đã chiếm identifier demo.
    IF EXISTS (SELECT 1 FROM @DemoUsers AS d
               INNER JOIN dbo.Users AS u WITH (UPDLOCK, HOLDLOCK)
                   ON u.NormalizedUsername = d.NormalizedUsername OR u.NormalizedEmail = d.NormalizedEmail
               INNER JOIN dbo.Roles AS r ON r.RoleId = u.RoleId
               WHERE u.NormalizedUsername <> d.NormalizedUsername OR u.NormalizedEmail <> d.NormalizedEmail
                  OR r.Name <> d.RoleName)
        THROW 51402, 'A demo identifier belongs to an incompatible account; seed did not overwrite it.', 1;

    INSERT INTO dbo.Users
        (RoleId, Username, NormalizedUsername, Email, NormalizedEmail,
         PasswordHash, PasswordSalt, PasswordIterations, PasswordAlgorithm, MustChangePassword, IsActive)
    SELECT r.RoleId, d.Username, d.NormalizedUsername, d.Email, d.NormalizedEmail,
           d.PasswordHash, d.PasswordSalt, 600000, N'PBKDF2-HMAC-SHA256', 0, 1
    FROM @DemoUsers AS d
    INNER JOIN dbo.Roles AS r ON r.Name = d.RoleName
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Users AS u WITH (UPDLOCK, HOLDLOCK)
                      WHERE u.NormalizedUsername = d.NormalizedUsername);

    INSERT INTO dbo.UserProfiles (UserId, DisplayName)
    SELECT u.UserId, d.DisplayName
    FROM @DemoUsers AS d
    INNER JOIN dbo.Users AS u ON u.NormalizedUsername = d.NormalizedUsername
    WHERE NOT EXISTS (SELECT 1 FROM dbo.UserProfiles AS p WITH (UPDLOCK, HOLDLOCK) WHERE p.UserId = u.UserId);

    INSERT INTO dbo.UserPreferences (UserId, Beach, Food, History, Culture, Entertainment, Shopping, Nature)
    SELECT u.UserId, 3, 3, 3, 3, 3, 3, 3
    FROM @DemoUsers AS d
    INNER JOIN dbo.Users AS u ON u.NormalizedUsername = d.NormalizedUsername
    WHERE NOT EXISTS (SELECT 1 FROM dbo.UserPreferences AS p WITH (UPDLOCK, HOLDLOCK) WHERE p.UserId = u.UserId);

    INSERT INTO dbo.TravelWallets (UserId, Balance)
    SELECT u.UserId, 0
    FROM @DemoUsers AS d
    INNER JOIN dbo.Users AS u ON u.NormalizedUsername = d.NormalizedUsername
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TravelWallets AS w WITH (UPDLOCK, HOLDLOCK) WHERE w.UserId = u.UserId);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
