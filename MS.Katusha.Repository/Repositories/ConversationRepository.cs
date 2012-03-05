using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repository.Interfaces;

namespace MS.Katusha.Repository.Repositories
{
    public class ConversationRepository : BaseGuidRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(KatushaContext context) : base(context) { }
    }
}