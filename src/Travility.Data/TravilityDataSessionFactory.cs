using Travility.Data.Contracts;
using Travility.Data.Model;

namespace Travility.Data
{
    public sealed class TravilityDataSessionFactory : ITravilityDataSessionFactory
    {
        public ITravilityDataSession Create()
        {
            return new TravilityDataSession(new TravilityEntities());
        }
    }
}
