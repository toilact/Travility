using System;
using NUnit.Framework;
using Travility.Core.Engines;
using Travility.Tests.TestData;

namespace Travility.Tests.Engines
{
    [TestFixture]
    public class RoutingEngineTests
    {
        private RoutingEngine _engine;

        [SetUp]
        public void SetUp()
        {
            _engine = new RoutingEngine();
        }

        [Test]
        public void Phai_BangKhong_Khi_HaiToaDoTrungNhau()
        {
            var place = TestPlaceBuilder.Named("Cầu Rồng").At(16.0611, 108.2275).Build();
            Assert.That(_engine.Distance(place, place), Is.EqualTo(0d).Within(0.000001));
        }

        [Test]
        public void Phai_DoiXung_Khi_DoiThuTuDiaDiem()
        {
            var museum = TestPlaceBuilder.Named("Bảo tàng Chăm").At(16.0599, 108.2235).Build();
            var beach = TestPlaceBuilder.Named("Biển Mỹ Khê").At(16.0544, 108.2428).Build();
            Assert.That(_engine.Distance(museum, beach), Is.EqualTo(_engine.Distance(beach, museum)).Within(0.000001));
        }

        [Test]
        public void Phai_XapXiHaiPhayBayChinSauKm_Khi_DiTuBaoTangChamDenMyKhe()
        {
            var museum = TestPlaceBuilder.Named("Bảo tàng Chăm").At(16.0599, 108.2235).Build();
            var beach = TestPlaceBuilder.Named("Biển Mỹ Khê").At(16.0544, 108.2428).Build();
            Assert.That(_engine.Distance(museum, beach), Is.EqualTo(2.79644d).Within(0.001d));
        }

        [Test]
        public void Phai_NemArgumentNull_Khi_ThieuDiaDiem()
        {
            var place = TestPlaceBuilder.Named("Cầu Rồng").At(16.0611, 108.2275).Build();
            Assert.Throws<ArgumentNullException>(() => _engine.Distance(null, place));
            Assert.Throws<ArgumentNullException>(() => _engine.Distance(place, null));
        }
    }
}
