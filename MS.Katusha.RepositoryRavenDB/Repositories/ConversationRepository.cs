using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class ConversationRepository : BaseGuidRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(KatushaContext context) : base(context) { }
    }
}