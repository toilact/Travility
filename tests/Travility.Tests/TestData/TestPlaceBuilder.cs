using Travility.Data.Model;

namespace Travility.Tests.TestData
{
    public sealed class TestPlaceBuilder
    {
        private string _name = "Địa điểm thử nghiệm";
        private double _lat;
        private double _lon;
        private int _placeId = 1;

        public static TestPlaceBuilder Named(string name)
        {
            return new TestPlaceBuilder { _name = name };
        }

        public TestPlaceBuilder At(double latitude, double longitude)
        {
            _lat = latitude;
            _lon = longitude;
            return this;
        }

        public TestPlaceBuilder WithId(int id)
        {
            _placeId = id;
            return this;
        }

        public Place Build()
        {
            return new Place
            {
                PlaceId = _placeId,
                Name = _name,
                Latitude = _lat,
                Longitude = _lon,
                IsActive = true
            };
        }
    }
}
