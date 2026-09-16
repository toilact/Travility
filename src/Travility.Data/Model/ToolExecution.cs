using System;

namespace Travility.Data.Model
{
    public partial class ToolExecution
    {
        public int ToolExecutionId { get; set; }
        public int ChatSessionId { get; set; }
        public string ToolName { get; set; }
        public string Arguments { get; set; }
        public string Result { get; set; }
        public DateTime ExecutedAtUtc { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        public virtual ChatSession ChatSession { get; set; }
    }
}
