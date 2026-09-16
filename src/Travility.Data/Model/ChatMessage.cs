using System;

namespace Travility.Data.Model
{
    public partial class ChatMessage
    {
        public int ChatMessageId { get; set; }
        public int ChatSessionId { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public virtual ChatSession ChatSession { get; set; }
    }
}
