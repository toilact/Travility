using System;

namespace Travility.Core.Contracts
{
    public interface IAppLogger
    {
        void Info(string component, string message);
        void Warning(string component, string message);
        void Error(string component, string errorId, Exception exception);
    }
}
