namespace Models
{
    public class ChatRequest
    {
        public required string Message { get; set; }
        public List<ChatHistoryItem> ChatHistory { get; set; } = [];
    }

    public class ChatHistoryItem
    {
        public required string Sender { get; set; }
        public required string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}