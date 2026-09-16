using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class PlaceType
    {
        public PlaceType()
        {
            Places = new HashSet<Place>();
        }

        public int PlaceTypeId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public virtual PlaceCategory PlaceCategory { get; set; }
        public virtual ICollection<Place> Places { get; set; }
    }
}
