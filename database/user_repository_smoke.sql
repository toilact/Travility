USE TravilityDev;
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE NormalizedUsername = N'ADMIN')
    THROW 51100, 'Admin username lookup failed', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE NormalizedEmail = N'ADMIN@TRAVILITY.LOCAL')
    THROW 51101, 'Admin email lookup failed', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE NormalizedUsername = N'TRAVELER')
    THROW 51102, 'Traveler username lookup failed', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE NormalizedEmail = N'TRAVELER@TRAVILITY.LOCAL')
    THROW 51103, 'Traveler email lookup failed', 1;

PRINT 'User repository smoke assertions passed.';
