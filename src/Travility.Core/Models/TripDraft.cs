using System;
using System.Collections.Generic;

namespace Travility.Core.Models
{
    public sealed class TripDraft
    {
        public int OwnerUserId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int PeopleCount { get; set; } = 1;
        public int RoomCount { get; set; } = 1;
        public decimal Budget { get; set; }
        public IDictionary<string, byte> Preferences { get; set; } = new Dictionary<string, byte>();
    }
}
