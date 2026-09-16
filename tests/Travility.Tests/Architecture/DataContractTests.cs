using System.Linq;
using NUnit.Framework;
using Travility.Data.Contracts;
using Travility.Data.Model;

namespace Travility.Tests.Architecture
{
    [TestFixture]
    public sealed class DataContractTests
    {
        [Test]
        public void Phai_TraEntityCuThe_Khi_DungUserRepositoryContract()
        {
            Assert.That(typeof(IUserRepository).GetMethod("GetById").ReturnType,
                Is.EqualTo(typeof(User)));
            Assert.That(typeof(IUserRepository).GetMethods().Any(m =>
                m.ReturnType.IsGenericType &&
                m.ReturnType.GetGenericTypeDefinition() == typeof(IQueryable<>)), Is.False);
        }

        [Test]
        public void Phai_KhongCoSaveChanges_Khi_DungRepositories()
        {
            var repoTypes = new[]
            {
                typeof(IUserRepository),
                typeof(IPlaceRepository),
                typeof(ITripRepository),
                typeof(IBudgetRepository)
            };

            foreach (var type in repoTypes)
            {
                Assert.That(type.GetMethods().Any(m => m.Name == "SaveChanges"), Is.False,
                    string.Format("Repository {0} khong duoc chua SaveChanges().", type.Name));
            }
        }

        [Test]
        public void Phai_CoTransactionBoundary_Khi_DungDataSession()
        {
            var sessionType = typeof(ITravilityDataSession);
            Assert.That(sessionType.GetMethod("BeginTransaction"), Is.Not.Null);
            Assert.That(sessionType.GetMethod("SaveChanges"), Is.Not.Null);
            Assert.That(sessionType.GetMethod("Commit"), Is.Not.Null);
            Assert.That(sessionType.GetMethod("Rollback"), Is.Not.Null);
            Assert.That(typeof(System.IDisposable).IsAssignableFrom(sessionType), Is.True);
        }
    }
}
