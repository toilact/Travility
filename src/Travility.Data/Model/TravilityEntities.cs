using System.Data.Entity;

namespace Travility.Data.Model
{
    public partial class TravilityEntities : DbContext
    {
        public TravilityEntities()
            : base("name=TravilityEntities")
        {
        }

        public TravilityEntities(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<UserPreference> UserPreferences { get; set; }
        public virtual DbSet<TravelWallet> TravelWallets { get; set; }

        public virtual DbSet<PlaceCategory> PlaceCategories { get; set; }
        public virtual DbSet<PlaceType> PlaceTypes { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<PlaceImage> PlaceImages { get; set; }
        public virtual DbSet<PlaceRating> PlaceRatings { get; set; }
        public virtual DbSet<PlaceReview> PlaceReviews { get; set; }
        public virtual DbSet<ExternalPlaceMapping> ExternalPlaceMappings { get; set; }

        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<TripMember> TripMembers { get; set; }
        public virtual DbSet<TripPreference> TripPreferences { get; set; }
        public virtual DbSet<TransportOption> TransportOptions { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<TripPlace> TripPlaces { get; set; }

        public virtual DbSet<Itinerary> Itineraries { get; set; }
        public virtual DbSet<ItineraryDay> ItineraryDays { get; set; }
        public virtual DbSet<ItineraryItem> ItineraryItems { get; set; }
        public virtual DbSet<RoutePlan> RoutePlans { get; set; }
        public virtual DbSet<RouteSegment> RouteSegments { get; set; }
        public virtual DbSet<DistanceMatrix> DistanceMatrices { get; set; }

        public virtual DbSet<BudgetCategory> BudgetCategories { get; set; }
        public virtual DbSet<TripBudget> TripBudgets { get; set; }
        public virtual DbSet<BudgetAllocation> BudgetAllocations { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }

        public virtual DbSet<CheckIn> CheckIns { get; set; }
        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<UserAchievement> UserAchievements { get; set; }

        public virtual DbSet<ChatSession> ChatSessions { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<ToolExecution> ToolExecutions { get; set; }
    }
}
