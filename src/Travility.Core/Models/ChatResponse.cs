using System.Collections.Generic;

namespace Travility.Core.Models
{
    public class ChatToolCall
    {
        public string ToolName { get; set; }
        public string ArgumentsJson { get; set; }
    }

    public class ChatResponse
    {
        public string Content { get; set; }
        public IList<ChatToolCall> ToolCalls { get; set; } = new List<ChatToolCall>();
        public bool HasToolCalls => ToolCalls != null && ToolCalls.Count > 0;
    }
}
