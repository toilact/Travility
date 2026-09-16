using System.Collections.Generic;
using Travility.Core.Models;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface ICheckInService
    {
        CheckInResult CheckIn(CheckInRequest request);
        IList<CheckIn> GetByTrip(int tripId);
    }
}
