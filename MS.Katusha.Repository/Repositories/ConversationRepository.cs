using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class ConversationRepository : BaseGuidRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}