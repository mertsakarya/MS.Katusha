namespace MS.Katusha.Domain.Raven.Entities
{
    public class ConversationResult
    {
        public long FromId { get; set; }
        public long ToId { get; set; }
        public int Count { get; set; }
        public int UnreadCount { get; set; }
    }
}