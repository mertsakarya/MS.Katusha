namespace MS.Katusha.Web.Models.Entities
{
    public class ConversationResultModel
    {
        public ProfileModel From { get; set; }
        public ProfileModel To { get; set; }
        public int Count { get; set; }
        public int UnreadCount { get; set; }
    }
}