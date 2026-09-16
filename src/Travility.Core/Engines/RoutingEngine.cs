using System;
using Travility.Data.Model;

namespace Travility.Core.Engines
{
    public class RoutingEngine
    {
        private const double EarthRadiusKm = 6371.0088;
        private const double DetourFactor = 1.3;

        public double Distance(Place a, Place b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            var lat1 = DegreesToRadians(a.Latitude);
            var lat2 = DegreesToRadians(b.Latitude);
            var deltaLat = lat2 - lat1;
            var deltaLon = DegreesToRadians(b.Longitude - a.Longitude);

            var h = Math.Pow(Math.Sin(deltaLat / 2d), 2d)
                  + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(deltaLon / 2d), 2d);

            var haversineKm = EarthRadiusKm * 2d * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1d - h));
            return haversineKm * DetourFactor;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180d;
        }
    }
}
