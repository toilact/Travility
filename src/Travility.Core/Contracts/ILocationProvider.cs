using Travility.Core.Models;

namespace Travility.Core.Contracts
{
    public interface ILocationProvider
    {
        Coordinate GetCurrentLocation();
        double AccuracyMeters { get; }
        string ProviderName { get; }
    }
}
