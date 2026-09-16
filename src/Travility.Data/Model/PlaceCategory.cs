using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class PlaceCategory
    {
        public PlaceCategory()
        {
            PlaceTypes = new HashSet<PlaceType>();
            Places = new HashSet<Place>();
            Achievements = new HashSet<Achievement>();
        }

        public int PlaceCategoryId { get; set; }
        public string Name { get; set; }
        public string IconKey { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual ICollection<PlaceType> PlaceTypes { get; set; }
        public virtual ICollection<Place> Places { get; set; }
        public virtual ICollection<Achievement> Achievements { get; set; }
    }
}
