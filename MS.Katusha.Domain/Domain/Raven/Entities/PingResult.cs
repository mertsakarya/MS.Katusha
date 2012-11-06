using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class PingResult
    {
        public NewVisits Visits { get; set; }
        public State State { get; set; }
        public ConversationCountResult Conversations { get; set; }
    }
}