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

    -- =========================================================================
    -- 1. IDENTITY & USERS
    -- =========================================================================
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

    -- =========================================================================
    -- 2. PLACES & CATEGORIES
    -- =========================================================================
    CREATE TABLE dbo.PlaceCategories
    (
        PlaceCategoryId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PlaceCategories PRIMARY KEY,
        Name nvarchar(100) NOT NULL,
        IconKey nvarchar(50) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_PlaceCategories_IsActive DEFAULT 1,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_PlaceCategories_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_PlaceCategories_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_PlaceCategories_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
    );
    CREATE UNIQUE INDEX UX_PlaceCategories_Name ON dbo.PlaceCategories(Name);

    CREATE TABLE dbo.PlaceTypes
    (
        PlaceTypeId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PlaceTypes PRIMARY KEY,
        CategoryId int NOT NULL,
        Name nvarchar(100) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_PlaceTypes_IsActive DEFAULT 1,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_PlaceTypes_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_PlaceTypes_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_PlaceTypes_PlaceCategories FOREIGN KEY (CategoryId) REFERENCES dbo.PlaceCategories(PlaceCategoryId) ON DELETE NO ACTION,
        CONSTRAINT CK_PlaceTypes_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
    );
    CREATE UNIQUE INDEX UX_PlaceTypes_CategoryId_Name ON dbo.PlaceTypes(CategoryId, Name);
    CREATE INDEX IX_PlaceTypes_CategoryId ON dbo.PlaceTypes(CategoryId);

    CREATE TABLE dbo.Places
    (
        PlaceId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Places PRIMARY KEY,
        CategoryId int NOT NULL,
        PlaceTypeId int NULL,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(max) NULL,
        Address nvarchar(300) NOT NULL,
        Latitude float NOT NULL,
        Longitude float NOT NULL,
        Rating decimal(3,2) NOT NULL CONSTRAINT DF_Places_Rating DEFAULT 0,
        ReviewCount int NOT NULL CONSTRAINT DF_Places_ReviewCount DEFAULT 0,
        TicketPrice decimal(18,2) NOT NULL CONSTRAINT DF_Places_TicketPrice DEFAULT 0,
        OpenTime time(0) NULL,
        CloseTime time(0) NULL,
        VisitDuration int NOT NULL CONSTRAINT DF_Places_VisitDuration DEFAULT 60,
        PreferenceTags nvarchar(100) NOT NULL,
        IsMealPlace bit NOT NULL CONSTRAINT DF_Places_IsMealPlace DEFAULT 0,
        PricingUnit tinyint NOT NULL CONSTRAINT DF_Places_PricingUnit DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_Places_IsActive DEFAULT 1,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Places_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Places_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Places_PlaceCategories FOREIGN KEY (CategoryId) REFERENCES dbo.PlaceCategories(PlaceCategoryId) ON DELETE NO ACTION,
        CONSTRAINT FK_Places_PlaceTypes FOREIGN KEY (PlaceTypeId) REFERENCES dbo.PlaceTypes(PlaceTypeId) ON DELETE NO ACTION,
        CONSTRAINT CK_Places_Latitude CHECK (Latitude BETWEEN -90 AND 90),
        CONSTRAINT CK_Places_Longitude CHECK (Longitude BETWEEN -180 AND 180),
        CONSTRAINT CK_Places_Rating CHECK (Rating BETWEEN 0 AND 5),
        CONSTRAINT CK_Places_ReviewCount CHECK (ReviewCount >= 0),
        CONSTRAINT CK_Places_TicketPrice CHECK (TicketPrice >= 0),
        CONSTRAINT CK_Places_VisitDuration CHECK (VisitDuration > 0)
    );
    CREATE INDEX IX_Places_CategoryId ON dbo.Places(CategoryId);
    CREATE INDEX IX_Places_PlaceTypeId ON dbo.Places(PlaceTypeId);

    CREATE TABLE dbo.PlaceImages
    (
        PlaceImageId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PlaceImages PRIMARY KEY,
        PlaceId int NOT NULL,
        ImagePath nvarchar(500) NOT NULL,
        SortOrder int NOT NULL CONSTRAINT DF_PlaceImages_SortOrder DEFAULT 0,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_PlaceImages_Created DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_PlaceImages_Places FOREIGN KEY (PlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION
    );
    CREATE INDEX IX_PlaceImages_PlaceId ON dbo.PlaceImages(PlaceId);

    CREATE TABLE dbo.PlaceRatings
    (
        PlaceRatingId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PlaceRatings PRIMARY KEY,
        PlaceId int NOT NULL,
        AverageRating decimal(3,2) NOT NULL,
        ReviewCount int NOT NULL,
        CalculatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_PlaceRatings_Calculated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_PlaceRatings_Places FOREIGN KEY (PlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT CK_PlaceRatings_AverageRating CHECK (AverageRating BETWEEN 0 AND 5),
        CONSTRAINT CK_PlaceRatings_ReviewCount CHECK (ReviewCount >= 0)
    );
    CREATE UNIQUE INDEX UX_PlaceRatings_PlaceId ON dbo.PlaceRatings(PlaceId);

    CREATE TABLE dbo.PlaceReviews
    (
        PlaceReviewId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PlaceReviews PRIMARY KEY,
        PlaceId int NOT NULL,
        UserId int NOT NULL,
        Rating decimal(3,2) NOT NULL,
        Content nvarchar(max) NULL,
        ReviewedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_PlaceReviews_Reviewed DEFAULT SYSUTCDATETIME(),
        IsActive bit NOT NULL CONSTRAINT DF_PlaceReviews_IsActive DEFAULT 1,
        CONSTRAINT FK_PlaceReviews_Places FOREIGN KEY (PlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT FK_PlaceReviews_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION,
        CONSTRAINT CK_PlaceReviews_Rating CHECK (Rating BETWEEN 0 AND 5)
    );
    CREATE INDEX IX_PlaceReviews_PlaceId ON dbo.PlaceReviews(PlaceId);
    CREATE INDEX IX_PlaceReviews_UserId ON dbo.PlaceReviews(UserId);

    CREATE TABLE dbo.ExternalPlaceMappings
    (
        ExternalPlaceMappingId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalPlaceMappings PRIMARY KEY,
        PlaceId int NOT NULL,
        ProviderName nvarchar(50) NOT NULL,
        ExternalPlaceId nvarchar(100) NOT NULL,
        LastSyncedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_ExternalPlaceMappings_LastSynced DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_ExternalPlaceMappings_Places FOREIGN KEY (PlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX UX_ExternalPlaceMappings_Provider_ExternalId ON dbo.ExternalPlaceMappings(ProviderName, ExternalPlaceId);
    CREATE INDEX IX_ExternalPlaceMappings_PlaceId ON dbo.ExternalPlaceMappings(PlaceId);

    -- =========================================================================
    -- 3. TRIPS, TRAVEL & BOOKINGS
    -- =========================================================================
    CREATE TABLE dbo.Trips
    (
        TripId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Trips PRIMARY KEY,
        OwnerUserId int NOT NULL,
        Origin nvarchar(100) NOT NULL,
        Destination nvarchar(100) NOT NULL,
        DepartDate date NOT NULL,
        ReturnDate date NOT NULL,
        PeopleCount int NOT NULL CONSTRAINT DF_Trips_PeopleCount DEFAULT 1,
        RoomCount int NOT NULL CONSTRAINT DF_Trips_RoomCount DEFAULT 1,
        Status tinyint NOT NULL CONSTRAINT DF_Trips_Status DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_Trips_IsActive DEFAULT 1,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Trips_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Trips_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Trips_Users FOREIGN KEY (OwnerUserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION,
        CONSTRAINT CK_Trips_Dates CHECK (ReturnDate >= DepartDate),
        CONSTRAINT CK_Trips_People CHECK (PeopleCount >= 1),
        CONSTRAINT CK_Trips_Rooms CHECK (RoomCount >= 1)
    );
    CREATE INDEX IX_Trips_OwnerUserId ON dbo.Trips(OwnerUserId);

    CREATE TABLE dbo.TripMembers
    (
        TripMemberId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TripMembers PRIMARY KEY,
        TripId int NOT NULL,
        UserId int NOT NULL,
        MemberRole nvarchar(50) NOT NULL CONSTRAINT DF_TripMembers_MemberRole DEFAULT N'Member',
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TripMembers_Created DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_TripMembers_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION,
        CONSTRAINT FK_TripMembers_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX UX_TripMembers_TripId_UserId ON dbo.TripMembers(TripId, UserId);
    CREATE INDEX IX_TripMembers_UserId ON dbo.TripMembers(UserId);

    CREATE TABLE dbo.TripPreferences
    (
        TripPreferenceId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TripPreferences PRIMARY KEY,
        TripId int NOT NULL,
        Beach tinyint NOT NULL CONSTRAINT DF_TripPreferences_Beach DEFAULT 3,
        Food tinyint NOT NULL CONSTRAINT DF_TripPreferences_Food DEFAULT 3,
        History tinyint NOT NULL CONSTRAINT DF_TripPreferences_History DEFAULT 3,
        Culture tinyint NOT NULL CONSTRAINT DF_TripPreferences_Culture DEFAULT 3,
        Entertainment tinyint NOT NULL CONSTRAINT DF_TripPreferences_Entertainment DEFAULT 3,
        Shopping tinyint NOT NULL CONSTRAINT DF_TripPreferences_Shopping DEFAULT 3,
        Nature tinyint NOT NULL CONSTRAINT DF_TripPreferences_Nature DEFAULT 3,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TripPreferences_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TripPreferences_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_TripPreferences_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION,
        CONSTRAINT CK_TripPreferences_Scores CHECK
            (Beach BETWEEN 1 AND 5 AND Food BETWEEN 1 AND 5 AND History BETWEEN 1 AND 5
             AND Culture BETWEEN 1 AND 5 AND Entertainment BETWEEN 1 AND 5
             AND Shopping BETWEEN 1 AND 5 AND Nature BETWEEN 1 AND 5)
    );
    CREATE UNIQUE INDEX UX_TripPreferences_TripId ON dbo.TripPreferences(TripId);

    CREATE TABLE dbo.TransportOptions
    (
        TransportOptionId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TransportOptions PRIMARY KEY,
        BookingType tinyint NOT NULL,
        ProviderName nvarchar(100) NOT NULL,
        Origin nvarchar(100) NOT NULL,
        Destination nvarchar(100) NOT NULL,
        DepartAt datetime2(0) NOT NULL,
        ArriveAt datetime2(0) NOT NULL,
        UnitPrice decimal(18,2) NOT NULL,
        PricingUnit tinyint NOT NULL CONSTRAINT DF_TransportOptions_PricingUnit DEFAULT 0,
        AvailableQuantity int NOT NULL CONSTRAINT DF_TransportOptions_AvailableQuantity DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_TransportOptions_IsActive DEFAULT 1,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TransportOptions_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TransportOptions_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_TransportOptions_UnitPrice CHECK (UnitPrice >= 0),
        CONSTRAINT CK_TransportOptions_Quantity CHECK (AvailableQuantity >= 0)
    );

    CREATE TABLE dbo.Bookings
    (
        BookingId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Bookings PRIMARY KEY,
        TripId int NOT NULL,
        TransportOptionId int NULL,
        PlaceId int NULL,
        BookingType tinyint NOT NULL,
        BookingStatus tinyint NOT NULL,
        Quantity int NOT NULL CONSTRAINT DF_Bookings_Quantity DEFAULT 1,
        UnitPrice decimal(18,2) NOT NULL,
        TotalCost decimal(18,2) NOT NULL,
        PricingUnit tinyint NOT NULL,
        ReferenceCode nvarchar(50) NOT NULL,
        BookedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Bookings_BookedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Bookings_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION,
        CONSTRAINT FK_Bookings_TransportOptions FOREIGN KEY (TransportOptionId) REFERENCES dbo.TransportOptions(TransportOptionId) ON DELETE NO ACTION,
        CONSTRAINT FK_Bookings_Places FOREIGN KEY (PlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT CK_Bookings_ExclusiveSource CHECK
            ((TransportOptionId IS NOT NULL AND PlaceId IS NULL) OR (TransportOptionId IS NULL AND PlaceId IS NOT NULL)),
        CONSTRAINT CK_Bookings_Quantity CHECK (Quantity >= 1),
        CONSTRAINT CK_Bookings_UnitPrice CHECK (UnitPrice >= 0),
        CONSTRAINT CK_Bookings_TotalCost CHECK (TotalCost >= 0)
    );
    CREATE INDEX IX_Bookings_TripId ON dbo.Bookings(TripId);
    CREATE INDEX IX_Bookings_TransportOptionId ON dbo.Bookings(TransportOptionId);
    CREATE INDEX IX_Bookings_PlaceId ON dbo.Bookings(PlaceId);

    CREATE TABLE dbo.TripPlaces
    (
        TripPlaceId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TripPlaces PRIMARY KEY,
        TripId int NOT NULL,
        PlaceId int NOT NULL,
        AddedByUserId int NOT NULL,
        AddedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TripPlaces_AddedAt DEFAULT SYSUTCDATETIME(),
        PreferredDay int NULL,
        CONSTRAINT FK_TripPlaces_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION,
        CONSTRAINT FK_TripPlaces_Places FOREIGN KEY (PlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT FK_TripPlaces_Users FOREIGN KEY (AddedByUserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX UX_TripPlaces_TripId_PlaceId ON dbo.TripPlaces(TripId, PlaceId);
    CREATE INDEX IX_TripPlaces_PlaceId ON dbo.TripPlaces(PlaceId);
    CREATE INDEX IX_TripPlaces_AddedByUserId ON dbo.TripPlaces(AddedByUserId);

    -- =========================================================================
    -- 4. ITINERARY, ROUTING & DISTANCE MATRIX
    -- =========================================================================
    CREATE TABLE dbo.Itineraries
    (
        ItineraryId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Itineraries PRIMARY KEY,
        TripId int NOT NULL,
        TravelStyle tinyint NOT NULL,
        TotalCost decimal(18,2) NOT NULL CONSTRAINT DF_Itineraries_TotalCost DEFAULT 0,
        TotalDistanceKm float NOT NULL CONSTRAINT DF_Itineraries_TotalDistance DEFAULT 0,
        TotalTravelTimeMinutes int NOT NULL CONSTRAINT DF_Itineraries_TotalTime DEFAULT 0,
        ExperienceScore decimal(5,2) NOT NULL CONSTRAINT DF_Itineraries_ExperienceScore DEFAULT 0,
        IsSelected bit NOT NULL CONSTRAINT DF_Itineraries_IsSelected DEFAULT 0,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Itineraries_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Itineraries_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Itineraries_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION,
        CONSTRAINT CK_Itineraries_TotalCost CHECK (TotalCost >= 0),
        CONSTRAINT CK_Itineraries_TotalDistance CHECK (TotalDistanceKm >= 0),
        CONSTRAINT CK_Itineraries_TotalTravelTime CHECK (TotalTravelTimeMinutes >= 0)
    );
    CREATE UNIQUE INDEX UX_Itineraries_SelectedPerTrip ON dbo.Itineraries(TripId) WHERE IsSelected = 1;
    CREATE INDEX IX_Itineraries_TripId ON dbo.Itineraries(TripId);

    CREATE TABLE dbo.ItineraryDays
    (
        ItineraryDayId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItineraryDays PRIMARY KEY,
        ItineraryId int NOT NULL,
        DayNumber int NOT NULL,
        Date date NOT NULL,
        CONSTRAINT FK_ItineraryDays_Itineraries FOREIGN KEY (ItineraryId) REFERENCES dbo.Itineraries(ItineraryId) ON DELETE NO ACTION,
        CONSTRAINT CK_ItineraryDays_DayNumber CHECK (DayNumber >= 1)
    );
    CREATE UNIQUE INDEX UX_ItineraryDays_ItineraryId_DayNumber ON dbo.ItineraryDays(ItineraryId, DayNumber);
    CREATE INDEX IX_ItineraryDays_ItineraryId ON dbo.ItineraryDays(ItineraryId);

    CREATE TABLE dbo.ItineraryItems
    (
        ItineraryItemId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItineraryItems PRIMARY KEY,
        ItineraryDayId int NOT NULL,
        PlaceId int NOT NULL,
        SequenceNumber int NOT NULL,
        StartTime time(0) NULL,
        EndTime time(0) NULL,
        VisitDuration int NOT NULL,
        PlannedCost decimal(18,2) NOT NULL CONSTRAINT DF_ItineraryItems_PlannedCost DEFAULT 0,
        CONSTRAINT FK_ItineraryItems_ItineraryDays FOREIGN KEY (ItineraryDayId) REFERENCES dbo.ItineraryDays(ItineraryDayId) ON DELETE NO ACTION,
        CONSTRAINT FK_ItineraryItems_Places FOREIGN KEY (PlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT CK_ItineraryItems_SequenceNumber CHECK (SequenceNumber >= 1),
        CONSTRAINT CK_ItineraryItems_VisitDuration CHECK (VisitDuration >= 0),
        CONSTRAINT CK_ItineraryItems_PlannedCost CHECK (PlannedCost >= 0)
    );
    CREATE UNIQUE INDEX UX_ItineraryItems_ItineraryDayId_SequenceNumber ON dbo.ItineraryItems(ItineraryDayId, SequenceNumber);
    CREATE INDEX IX_ItineraryItems_PlaceId ON dbo.ItineraryItems(PlaceId);

    CREATE TABLE dbo.RoutePlans
    (
        RoutePlanId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_RoutePlans PRIMARY KEY,
        ItineraryDayId int NOT NULL,
        TotalDistanceKm float NOT NULL CONSTRAINT DF_RoutePlans_TotalDistance DEFAULT 0,
        TotalTravelTimeMinutes int NOT NULL CONSTRAINT DF_RoutePlans_TotalTime DEFAULT 0,
        TotalTransportCost decimal(18,2) NOT NULL CONSTRAINT DF_RoutePlans_TotalCost DEFAULT 0,
        CONSTRAINT FK_RoutePlans_ItineraryDays FOREIGN KEY (ItineraryDayId) REFERENCES dbo.ItineraryDays(ItineraryDayId) ON DELETE NO ACTION,
        CONSTRAINT CK_RoutePlans_TotalDistance CHECK (TotalDistanceKm >= 0),
        CONSTRAINT CK_RoutePlans_TotalTravelTime CHECK (TotalTravelTimeMinutes >= 0),
        CONSTRAINT CK_RoutePlans_TotalCost CHECK (TotalTransportCost >= 0)
    );
    CREATE UNIQUE INDEX UX_RoutePlans_ItineraryDayId ON dbo.RoutePlans(ItineraryDayId);

    CREATE TABLE dbo.RouteSegments
    (
        RouteSegmentId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_RouteSegments PRIMARY KEY,
        RoutePlanId int NOT NULL,
        SequenceNumber int NOT NULL,
        FromPlaceId int NOT NULL,
        ToPlaceId int NOT NULL,
        DistanceKm float NOT NULL,
        TravelTimeMinutes int NOT NULL,
        TransportCost decimal(18,2) NOT NULL CONSTRAINT DF_RouteSegments_Cost DEFAULT 0,
        CONSTRAINT FK_RouteSegments_RoutePlans FOREIGN KEY (RoutePlanId) REFERENCES dbo.RoutePlans(RoutePlanId) ON DELETE NO ACTION,
        CONSTRAINT FK_RouteSegments_FromPlace FOREIGN KEY (FromPlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT FK_RouteSegments_ToPlace FOREIGN KEY (ToPlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT CK_RouteSegments_SequenceNumber CHECK (SequenceNumber >= 1),
        CONSTRAINT CK_RouteSegments_Distance CHECK (DistanceKm >= 0),
        CONSTRAINT CK_RouteSegments_TravelTime CHECK (TravelTimeMinutes >= 0),
        CONSTRAINT CK_RouteSegments_Cost CHECK (TransportCost >= 0)
    );
    CREATE UNIQUE INDEX UX_RouteSegments_RoutePlanId_SequenceNumber ON dbo.RouteSegments(RoutePlanId, SequenceNumber);
    CREATE INDEX IX_RouteSegments_FromPlace ON dbo.RouteSegments(FromPlaceId);
    CREATE INDEX IX_RouteSegments_ToPlace ON dbo.RouteSegments(ToPlaceId);

    CREATE TABLE dbo.DistanceMatrix
    (
        DistanceMatrixId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_DistanceMatrix PRIMARY KEY,
        FromPlaceId int NOT NULL,
        ToPlaceId int NOT NULL,
        DistanceKm float NOT NULL,
        EstimatedTravelTimeMinutes int NOT NULL,
        CalculatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_DistanceMatrix_Calculated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_DistanceMatrix_FromPlace FOREIGN KEY (FromPlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT FK_DistanceMatrix_ToPlace FOREIGN KEY (ToPlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT CK_DistanceMatrix_Order CHECK (FromPlaceId < ToPlaceId),
        CONSTRAINT CK_DistanceMatrix_Distance CHECK (DistanceKm >= 0),
        CONSTRAINT CK_DistanceMatrix_Time CHECK (EstimatedTravelTimeMinutes >= 0)
    );
    CREATE UNIQUE INDEX UX_DistanceMatrix_Pair ON dbo.DistanceMatrix(FromPlaceId, ToPlaceId);
    CREATE INDEX IX_DistanceMatrix_ToPlace ON dbo.DistanceMatrix(ToPlaceId);

    -- =========================================================================
    -- 5. FINANCE, BUDGETS & EXPENSES
    -- =========================================================================
    CREATE TABLE dbo.BudgetCategories
    (
        BudgetCategoryId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BudgetCategories PRIMARY KEY,
        Name nvarchar(50) NOT NULL,
        DisplayOrder int NOT NULL CONSTRAINT DF_BudgetCategories_Order DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_BudgetCategories_IsActive DEFAULT 1,
        CONSTRAINT CK_BudgetCategories_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
    );
    CREATE UNIQUE INDEX UX_BudgetCategories_Name ON dbo.BudgetCategories(Name);

    CREATE TABLE dbo.TripBudgets
    (
        TripBudgetId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TripBudgets PRIMARY KEY,
        TripId int NOT NULL,
        Budget decimal(18,2) NOT NULL,
        PlannedCost decimal(18,2) NOT NULL CONSTRAINT DF_TripBudgets_PlannedCost DEFAULT 0,
        ActualSpent decimal(18,2) NOT NULL CONSTRAINT DF_TripBudgets_ActualSpent DEFAULT 0,
        UpdatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_TripBudgets_Updated DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_TripBudgets_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION,
        CONSTRAINT CK_TripBudgets_Budget CHECK (Budget >= 0),
        CONSTRAINT CK_TripBudgets_PlannedCost CHECK (PlannedCost >= 0),
        CONSTRAINT CK_TripBudgets_ActualSpent CHECK (ActualSpent >= 0)
    );
    CREATE UNIQUE INDEX UX_TripBudgets_TripId ON dbo.TripBudgets(TripId);

    CREATE TABLE dbo.BudgetAllocations
    (
        BudgetAllocationId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BudgetAllocations PRIMARY KEY,
        TripBudgetId int NOT NULL,
        BudgetCategoryId int NOT NULL,
        Ratio decimal(5,4) NOT NULL,
        AllocatedAmount decimal(18,2) NOT NULL,
        CONSTRAINT FK_BudgetAllocations_TripBudgets FOREIGN KEY (TripBudgetId) REFERENCES dbo.TripBudgets(TripBudgetId) ON DELETE NO ACTION,
        CONSTRAINT FK_BudgetAllocations_BudgetCategories FOREIGN KEY (BudgetCategoryId) REFERENCES dbo.BudgetCategories(BudgetCategoryId) ON DELETE NO ACTION,
        CONSTRAINT CK_BudgetAllocations_Ratio CHECK (Ratio BETWEEN 0 AND 1),
        CONSTRAINT CK_BudgetAllocations_Amount CHECK (AllocatedAmount >= 0)
    );
    CREATE UNIQUE INDEX UX_BudgetAllocations_TripBudgetId_CategoryId ON dbo.BudgetAllocations(TripBudgetId, BudgetCategoryId);
    CREATE INDEX IX_BudgetAllocations_CategoryId ON dbo.BudgetAllocations(BudgetCategoryId);

    CREATE TABLE dbo.Expenses
    (
        ExpenseId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Expenses PRIMARY KEY,
        TripId int NOT NULL,
        UserId int NOT NULL,
        BudgetCategoryId int NOT NULL,
        Amount decimal(18,2) NOT NULL,
        Description nvarchar(255) NULL,
        SpentAtUtc datetime2(0) NOT NULL CONSTRAINT DF_Expenses_SpentAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Expenses_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION,
        CONSTRAINT FK_Expenses_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION,
        CONSTRAINT FK_Expenses_BudgetCategories FOREIGN KEY (BudgetCategoryId) REFERENCES dbo.BudgetCategories(BudgetCategoryId) ON DELETE NO ACTION,
        CONSTRAINT CK_Expenses_Amount CHECK (Amount > 0)
    );
    CREATE INDEX IX_Expenses_TripId ON dbo.Expenses(TripId);
    CREATE INDEX IX_Expenses_UserId ON dbo.Expenses(UserId);
    CREATE INDEX IX_Expenses_BudgetCategoryId ON dbo.Expenses(BudgetCategoryId);

    -- =========================================================================
    -- 6. GAMIFICATION & CHECK-IN
    -- =========================================================================
    CREATE TABLE dbo.CheckIns
    (
        CheckInId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CheckIns PRIMARY KEY,
        UserId int NOT NULL,
        TripId int NULL,
        PlaceId int NOT NULL,
        CheckInTimeUtc datetime2(0) NOT NULL CONSTRAINT DF_CheckIns_Time DEFAULT SYSUTCDATETIME(),
        Latitude float NOT NULL,
        Longitude float NOT NULL,
        DistanceFromPlace float NOT NULL,
        IsSimulated bit NOT NULL CONSTRAINT DF_CheckIns_IsSimulated DEFAULT 0,
        CONSTRAINT FK_CheckIns_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION,
        CONSTRAINT FK_CheckIns_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION,
        CONSTRAINT FK_CheckIns_Places FOREIGN KEY (PlaceId) REFERENCES dbo.Places(PlaceId) ON DELETE NO ACTION,
        CONSTRAINT CK_CheckIns_Latitude CHECK (Latitude BETWEEN -90 AND 90),
        CONSTRAINT CK_CheckIns_Longitude CHECK (Longitude BETWEEN -180 AND 180),
        CONSTRAINT CK_CheckIns_Distance CHECK (DistanceFromPlace >= 0)
    );
    CREATE INDEX IX_CheckIns_UserId ON dbo.CheckIns(UserId);
    CREATE INDEX IX_CheckIns_TripId ON dbo.CheckIns(TripId);
    CREATE INDEX IX_CheckIns_PlaceId ON dbo.CheckIns(PlaceId);

    CREATE TABLE dbo.Achievements
    (
        AchievementId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Achievements PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(100) NOT NULL,
        Description nvarchar(255) NOT NULL,
        ConditionType nvarchar(50) NOT NULL,
        Threshold int NOT NULL,
        CategoryId int NULL,
        BadgeImagePath nvarchar(500) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Achievements_IsActive DEFAULT 1,
        CONSTRAINT FK_Achievements_PlaceCategories FOREIGN KEY (CategoryId) REFERENCES dbo.PlaceCategories(PlaceCategoryId) ON DELETE NO ACTION,
        CONSTRAINT CK_Achievements_Threshold CHECK (Threshold >= 1)
    );
    CREATE UNIQUE INDEX UX_Achievements_Code ON dbo.Achievements(Code);
    CREATE INDEX IX_Achievements_CategoryId ON dbo.Achievements(CategoryId);

    CREATE TABLE dbo.UserAchievements
    (
        UserAchievementId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserAchievements PRIMARY KEY,
        UserId int NOT NULL,
        AchievementId int NOT NULL,
        AwardedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_UserAchievements_AwardedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_UserAchievements_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION,
        CONSTRAINT FK_UserAchievements_Achievements FOREIGN KEY (AchievementId) REFERENCES dbo.Achievements(AchievementId) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX UX_UserAchievements_UserId_AchievementId ON dbo.UserAchievements(UserId, AchievementId);
    CREATE INDEX IX_UserAchievements_AchievementId ON dbo.UserAchievements(AchievementId);

    -- =========================================================================
    -- 7. AI ASSISTANT & TOOL EXECUTIONS
    -- =========================================================================
    CREATE TABLE dbo.ChatSessions
    (
        ChatSessionId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChatSessions PRIMARY KEY,
        UserId int NOT NULL,
        TripId int NULL,
        ProviderName nvarchar(50) NOT NULL,
        StartedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_ChatSessions_Started DEFAULT SYSUTCDATETIME(),
        EndedAtUtc datetime2(0) NULL,
        IsReplay bit NOT NULL CONSTRAINT DF_ChatSessions_IsReplay DEFAULT 0,
        CONSTRAINT FK_ChatSessions_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE NO ACTION,
        CONSTRAINT FK_ChatSessions_Trips FOREIGN KEY (TripId) REFERENCES dbo.Trips(TripId) ON DELETE NO ACTION
    );
    CREATE INDEX IX_ChatSessions_UserId ON dbo.ChatSessions(UserId);
    CREATE INDEX IX_ChatSessions_TripId ON dbo.ChatSessions(TripId);

    CREATE TABLE dbo.ChatMessages
    (
        ChatMessageId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChatMessages PRIMARY KEY,
        ChatSessionId int NOT NULL,
        Role nvarchar(20) NOT NULL,
        Content nvarchar(max) NOT NULL,
        CreatedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_ChatMessages_Created DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_ChatMessages_ChatSessions FOREIGN KEY (ChatSessionId) REFERENCES dbo.ChatSessions(ChatSessionId) ON DELETE NO ACTION
    );
    CREATE INDEX IX_ChatMessages_ChatSessionId ON dbo.ChatMessages(ChatSessionId);

    CREATE TABLE dbo.ToolExecutions
    (
        ToolExecutionId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ToolExecutions PRIMARY KEY,
        ChatSessionId int NOT NULL,
        ToolName nvarchar(100) NOT NULL,
        Arguments nvarchar(max) NOT NULL,
        Result nvarchar(max) NULL,
        ExecutedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_ToolExecutions_ExecutedAt DEFAULT SYSUTCDATETIME(),
        Status nvarchar(20) NOT NULL,
        ErrorMessage nvarchar(max) NULL,
        CONSTRAINT FK_ToolExecutions_ChatSessions FOREIGN KEY (ChatSessionId) REFERENCES dbo.ChatSessions(ChatSessionId) ON DELETE NO ACTION
    );
    CREATE INDEX IX_ToolExecutions_ChatSessionId ON dbo.ToolExecutions(ChatSessionId);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
