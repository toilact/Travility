using System.Linq;
using NUnit.Framework;
using Travility.Core.Session;

namespace Travility.Tests.Session
{
    [TestFixture]
    public sealed class NavigationPolicyTests
    {
        [Test]
        public void Phai_AnQuanTri_Khi_LaTraveler()
        {
            var items = NavigationPolicy.ForRole("Traveler").Select(x => x.Key).ToArray();
            CollectionAssert.Contains(items, "Map");
            CollectionAssert.DoesNotContain(items, "Users");
        }

        [Test]
        public void Phai_HienQuanTri_Khi_LaAdmin()
        {
            var items = NavigationPolicy.ForRole("Admin").Select(x => x.Key).ToArray();
            CollectionAssert.Contains(items, "Users");
            CollectionAssert.DoesNotContain(items, "Trips");
        }

        [Test]
        public void Phai_CoTroLyAI_Khi_BatFeatureFlagChoTraveler()
        {
            var itemsWithout = NavigationPolicy.ForRole("Traveler", enableAssistant: false).Select(x => x.Key).ToArray();
            CollectionAssert.DoesNotContain(itemsWithout, "Assistant");

            var itemsWith = NavigationPolicy.ForRole("Traveler", enableAssistant: true).Select(x => x.Key).ToArray();
            CollectionAssert.Contains(itemsWith, "Assistant");
        }
    }
}
