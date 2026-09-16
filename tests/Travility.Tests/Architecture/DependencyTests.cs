using System.Linq;
using NUnit.Framework;

namespace Travility.Tests.Architecture
{
    [TestFixture]
    public sealed class DependencyTests
    {
        [Test]
        public void Phai_KhongThamChieuWinForms_Khi_LoadTravilityCore()
        {
            var references = typeof(Travility.Core.CoreAssemblyMarker)
                .Assembly
                .GetReferencedAssemblies()
                .Select(x => x.Name)
                .ToArray();

            CollectionAssert.DoesNotContain(references, "System.Windows.Forms");
            CollectionAssert.DoesNotContain(references, "Travility.WinForms");
        }
    }
}
