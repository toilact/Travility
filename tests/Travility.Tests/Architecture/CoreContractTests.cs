using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Travility.Core.Contracts;
using Travility.Core.Models;

namespace Travility.Tests.Architecture
{
    [TestFixture]
    public sealed class CoreContractTests
    {
        [Test]
        public void Phai_GiuChuKyTopK_Khi_LoadRecommendationContract()
        {
            var method = typeof(IRecommendationEngine).GetMethod("TopK");
            Assert.That(method, Is.Not.Null);
            Assert.That(method.ReturnType, Is.EqualTo(typeof(IList<ScoredPlace>)));
        }

        [Test]
        public void Phai_CoBonAuthUseCase_Khi_LoadAuthenticationContract()
        {
            var names = typeof(IAuthenticationService).GetMethods().Select(x => x.Name).ToArray();
            CollectionAssert.AreEquivalent(
                new[] { "Login", "Register", "SetTemporaryPassword", "ChangePassword" }, names);
        }

        [Test]
        public void Phai_CoDu13ContractsTrongCore()
        {
            var contracts = new[]
            {
                typeof(IAuthenticationService),
                typeof(IAppLogger),
                typeof(ITripService),
                typeof(IBudgetService),
                typeof(IPlaceService),
                typeof(IBookingService),
                typeof(ICheckInService),
                typeof(IAchievementService),
                typeof(IRecommendationEngine),
                typeof(IRoutingEngine),
                typeof(IItineraryEngine),
                typeof(ILocationProvider),
                typeof(IChatProvider)
            };

            foreach (var contract in contracts)
            {
                Assert.That(contract.IsInterface, Is.True, string.Format("{0} phai la interface.", contract.Name));
            }
        }
    }
}
