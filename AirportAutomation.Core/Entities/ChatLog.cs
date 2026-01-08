namespace AirportAutomation.Core.Entities
{
    public class ChatLog
    {
        public int Id { get; set; }
        public string UserMessage { get; set; } = string.Empty;
        public string AiResponse { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

