using System.Collections.Generic;

namespace MS.Katusha.Web.Models.Entities
{
    public class ConversationResultModel
    {
        public ProfileModel From { get; set; }
        public ProfileModel To { get; set; }
        public string Subject { get; set; }
        public int Count { get; set; }
        public long UnreadCount { get; set; }
    }
}