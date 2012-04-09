namespace MS.Katusha.Domain.Raven.Entities
{
    public class ConversationResult
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public int Count { get; set; }
        public int UnreadCount { get; set; }
    }
}