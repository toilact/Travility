:ON ERROR EXIT
USE [TravilityDev];
GO
SET NOCOUNT ON;

IF DB_NAME() COLLATE Latin1_General_100_BIN2 <> N'TravilityDev'
   OR DATALENGTH(DB_NAME()) <> DATALENGTH(N'TravilityDev')
    THROW 51000, 'Smoke tests must run against TravilityDev.', 1;

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
    THROW 51001, 'Missing dbo.Roles.', 1;
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
    THROW 51002, 'Missing dbo.Users.', 1;
IF OBJECT_ID(N'dbo.UserProfiles', N'U') IS NULL
    THROW 51003, 'Missing dbo.UserProfiles.', 1;
IF OBJECT_ID(N'dbo.UserPreferences', N'U') IS NULL
    THROW 51004, 'Missing dbo.UserPreferences.', 1;
IF OBJECT_ID(N'dbo.TravelWallets', N'U') IS NULL
    THROW 51005, 'Missing dbo.TravelWallets.', 1;
GO

-- Batch riêng: báo thiếu bảng trước khi SQL Server biên dịch các truy vấn dưới.
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
    THROW 51011, 'A preference score is outside [1,5].', 1;

IF (SELECT COUNT(*) FROM sys.foreign_keys
    WHERE parent_object_id IN (OBJECT_ID(N'dbo.Users'), OBJECT_ID(N'dbo.UserProfiles'),
                              OBJECT_ID(N'dbo.UserPreferences'), OBJECT_ID(N'dbo.TravelWallets'))
      AND is_disabled = 0 AND is_not_trusted = 0
      AND delete_referential_action = 0 AND update_referential_action = 0) <> 4
    THROW 51012, 'Identity foreign keys must be enabled, trusted and NO ACTION.', 1;

IF (SELECT COUNT(*) FROM sys.indexes
    WHERE is_unique = 1 AND is_disabled = 0 AND has_filter = 0
      AND ((object_id = OBJECT_ID(N'dbo.Roles') AND name = N'UX_Roles_Name')
        OR (object_id = OBJECT_ID(N'dbo.Users') AND name IN (N'UX_Users_NormalizedUsername', N'UX_Users_NormalizedEmail'))
        OR (object_id = OBJECT_ID(N'dbo.UserProfiles') AND name = N'UX_UserProfiles_UserId')
        OR (object_id = OBJECT_ID(N'dbo.UserPreferences') AND name = N'UX_UserPreferences_UserId')
        OR (object_id = OBJECT_ID(N'dbo.TravelWallets') AND name = N'UX_TravelWallets_UserId'))) <> 6
    THROW 51013, 'Identity unique indexes are missing or disabled.', 1;

IF EXISTS (SELECT 1 FROM sys.check_constraints
           WHERE parent_object_id IN (OBJECT_ID(N'dbo.Users'), OBJECT_ID(N'dbo.UserPreferences'))
             AND (is_disabled = 1 OR is_not_trusted = 1))
    THROW 51014, 'An Identity check constraint is disabled or untrusted.', 1;

PRINT N'Identity smoke tests passed; no data was changed.';
GO
