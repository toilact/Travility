:ON ERROR EXIT
USE [TravilityDev];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() COLLATE Latin1_General_100_BIN2 <> N'TravilityDev'
   OR DATALENGTH(DB_NAME()) <> DATALENGTH(N'TravilityDev')
    THROW 51200, 'Schema must run against TravilityDev.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    CREATE TABLE dbo.Roles
    (
        RoleId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        Name nvarchar(50) NOT NULL,
        Description nvarchar(250) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT 1,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Roles_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Roles_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_Roles_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
    );
    CREATE UNIQUE INDEX UX_Roles_Name ON dbo.Roles(Name);

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
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId) ON DELETE NO ACTION,
        CONSTRAINT CK_Users_Username_NoAt CHECK (Username NOT LIKE N'%@%'),
        CONSTRAINT CK_Users_Username_Length CHECK (LEN(Username) BETWEEN 3 AND 50),
        CONSTRAINT CK_Users_Identifiers_NotBlank CHECK
            (LEN(LTRIM(RTRIM(NormalizedUsername))) > 0 AND LEN(LTRIM(RTRIM(Email))) > 0
             AND LEN(LTRIM(RTRIM(NormalizedEmail))) > 0),
        CONSTRAINT CK_Users_Iterations_Positive CHECK (PasswordIterations BETWEEN 1 AND 1200000),
        CONSTRAINT CK_Users_PasswordHash_Length CHECK (DATALENGTH(PasswordHash) = 32),
        CONSTRAINT CK_Users_PasswordSalt_Length CHECK (DATALENGTH(PasswordSalt) = 16),
        CONSTRAINT CK_Users_PasswordAlgorithm CHECK
            (PasswordAlgorithm COLLATE Latin1_General_100_BIN2 = N'PBKDF2-HMAC-SHA256'
             AND DATALENGTH(PasswordAlgorithm) = DATALENGTH(N'PBKDF2-HMAC-SHA256'))
    );
    CREATE UNIQUE INDEX UX_Users_NormalizedUsername ON dbo.Users(NormalizedUsername);
    CREATE UNIQUE INDEX UX_Users_NormalizedEmail ON dbo.Users(NormalizedEmail);
    CREATE INDEX IX_Users_RoleId ON dbo.Users(RoleId);

    CREATE TABLE dbo.UserProfiles
    (
        UserProfileId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserProfiles PRIMARY KEY,
        UserId int NOT NULL,
        DisplayName nvarchar(100) NOT NULL,
        Phone nvarchar(30) NULL,
        AvatarPath nvarchar(500) NULL,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_UserProfiles_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_UserProfiles_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_UserProfiles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX UX_UserProfiles_UserId ON dbo.UserProfiles(UserId);

    CREATE TABLE dbo.UserPreferences
    (
        UserPreferenceId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserPreferences PRIMARY KEY,
        UserId int NOT NULL,
        Beach tinyint NOT NULL CONSTRAINT DF_UserPreferences_Beach DEFAULT 3,
        Food tinyint NOT NULL CONSTRAINT DF_UserPreferences_Food DEFAULT 3,
        History tinyint NOT NULL CONSTRAINT DF_UserPreferences_History DEFAULT 3,
        Culture tinyint NOT NULL CONSTRAINT DF_UserPreferences_Culture DEFAULT 3,
        Entertainment tinyint NOT NULL CONSTRAINT DF_UserPreferences_Entertainment DEFAULT 3,
        Shopping tinyint NOT NULL CONSTRAINT DF_UserPreferences_Shopping DEFAULT 3,
        Nature tinyint NOT NULL CONSTRAINT DF_UserPreferences_Nature DEFAULT 3,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_UserPreferences_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_UserPreferences_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_UserPreferences_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION,
        CONSTRAINT CK_UserPreferences_Scores CHECK
            (Beach BETWEEN 1 AND 5 AND Food BETWEEN 1 AND 5 AND History BETWEEN 1 AND 5
             AND Culture BETWEEN 1 AND 5 AND Entertainment BETWEEN 1 AND 5
             AND Shopping BETWEEN 1 AND 5 AND Nature BETWEEN 1 AND 5)
    );
    CREATE UNIQUE INDEX UX_UserPreferences_UserId ON dbo.UserPreferences(UserId);

    CREATE TABLE dbo.TravelWallets
    (
        TravelWalletId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TravelWallets PRIMARY KEY,
        UserId int NOT NULL,
        Balance decimal(18,2) NOT NULL CONSTRAINT DF_TravelWallets_Balance DEFAULT 0,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TravelWallets_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TravelWallets_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_TravelWallets_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX UX_TravelWallets_UserId ON dbo.TravelWallets(UserId);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
