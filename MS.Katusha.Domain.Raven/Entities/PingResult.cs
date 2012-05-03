namespace MS.Katusha.Domain.Raven.Entities
{
    public class PingResult
    {
        public NewVisits Visits { get; set; }
        public ConversationCountResult Conversations { get; set; }
    }
}