using System;
using System.Collections.Generic;

namespace Travility.Data.Model
{
    public partial class ChatSession
    {
        public ChatSession()
        {
            ChatMessages = new HashSet<ChatMessage>();
            ToolExecutions = new HashSet<ToolExecution>();
        }

        public int ChatSessionId { get; set; }
        public int UserId { get; set; }
        public int? TripId { get; set; }
        public string ProviderName { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? EndedAtUtc { get; set; }
        public bool IsReplay { get; set; }

        public virtual User User { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<ToolExecution> ToolExecutions { get; set; }
    }
}
