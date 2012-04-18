namespace MS.Katusha.Domain.Raven.Entities
{
    public class ConversationCountResult
    {
        public long ToId { get; set; }
        public int Count { get; set; }
        public int UnreadCount { get; set; }
    }
}