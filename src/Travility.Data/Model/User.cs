using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class User
    {
        public User()
        {
            Trips = new HashSet<Trip>();
            TripMembers = new HashSet<TripMember>();
            TripPlaces = new HashSet<TripPlace>();
            Expenses = new HashSet<Expense>();
            CheckIns = new HashSet<CheckIn>();
            UserAchievements = new HashSet<UserAchievement>();
            ChatSessions = new HashSet<ChatSession>();
            PlaceReviews = new HashSet<PlaceReview>();
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int PasswordIterations { get; set; }
        public string PasswordAlgorithm { get; set; }
        public bool MustChangePassword { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual Role Role { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual UserPreference UserPreference { get; set; }
        public virtual TravelWallet TravelWallet { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
        public virtual ICollection<TripMember> TripMembers { get; set; }
        public virtual ICollection<TripPlace> TripPlaces { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<CheckIn> CheckIns { get; set; }
        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
        public virtual ICollection<ChatSession> ChatSessions { get; set; }
        public virtual ICollection<PlaceReview> PlaceReviews { get; set; }
    }
}
