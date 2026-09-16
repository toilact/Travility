using System.Collections.Generic;
using Travility.Core.Models;
using Travility.Data.Model;

namespace Travility.Core.Contracts
{
    public interface IChatProvider
    {
        ChatResponse Send(IList<ChatMessage> history, IList<ToolDefinition> tools);
        string ProviderName { get; }
    }
}
