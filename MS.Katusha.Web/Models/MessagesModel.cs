using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Models
{
    public class MessagesModel
    {
        public MessageType MessageType { get; set; }
        public PagedListModel<ConversationModel> Conversations { get; set; }
        public ConversationCountResult Statistics { get; set; }
    }
}