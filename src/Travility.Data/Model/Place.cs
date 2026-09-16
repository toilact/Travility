using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class Place
    {
        public Place()
        {
            PlaceImages = new HashSet<PlaceImage>();
            PlaceReviews = new HashSet<PlaceReview>();
            ExternalPlaceMappings = new HashSet<ExternalPlaceMapping>();
            Bookings = new HashSet<Booking>();
            TripPlaces = new HashSet<TripPlace>();
            ItineraryItems = new HashSet<ItineraryItem>();
            FromRouteSegments = new HashSet<RouteSegment>();
            ToRouteSegments = new HashSet<RouteSegment>();
            FromDistanceMatrix = new HashSet<DistanceMatrix>();
            ToDistanceMatrix = new HashSet<DistanceMatrix>();
            CheckIns = new HashSet<CheckIn>();
        }

        public int PlaceId { get; set; }
        public int CategoryId { get; set; }
        public int? PlaceTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public decimal TicketPrice { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public int VisitDuration { get; set; }
        public string PreferenceTags { get; set; }
        public bool IsMealPlace { get; set; }
        public byte PricingUnit { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual PlaceCategory PlaceCategory { get; set; }
        public virtual PlaceType PlaceType { get; set; }
        public virtual PlaceRating PlaceRating { get; set; }
        public virtual ICollection<PlaceImage> PlaceImages { get; set; }
        public virtual ICollection<PlaceReview> PlaceReviews { get; set; }
        public virtual ICollection<ExternalPlaceMapping> ExternalPlaceMappings { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<TripPlace> TripPlaces { get; set; }
        public virtual ICollection<ItineraryItem> ItineraryItems { get; set; }
        public virtual ICollection<RouteSegment> FromRouteSegments { get; set; }
        public virtual ICollection<RouteSegment> ToRouteSegments { get; set; }
        public virtual ICollection<DistanceMatrix> FromDistanceMatrix { get; set; }
        public virtual ICollection<DistanceMatrix> ToDistanceMatrix { get; set; }
        public virtual ICollection<CheckIn> CheckIns { get; set; }
    }
}
