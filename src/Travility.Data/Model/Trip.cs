using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class Trip
    {
        public Trip()
        {
            TripMembers = new HashSet<TripMember>();
            TripPlaces = new HashSet<TripPlace>();
            Bookings = new HashSet<Booking>();
            Itineraries = new HashSet<Itinerary>();
            Expenses = new HashSet<Expense>();
            CheckIns = new HashSet<CheckIn>();
            ChatSessions = new HashSet<ChatSession>();
        }

        public int TripId { get; set; }
        public int OwnerUserId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int PeopleCount { get; set; }
        public int RoomCount { get; set; }
        public byte Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual User OwnerUser { get; set; }
        public virtual TripPreference TripPreference { get; set; }
        public virtual TripBudget TripBudget { get; set; }
        public virtual ICollection<TripMember> TripMembers { get; set; }
        public virtual ICollection<TripPlace> TripPlaces { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Itinerary> Itineraries { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<CheckIn> CheckIns { get; set; }
        public virtual ICollection<ChatSession> ChatSessions { get; set; }
    }
}
