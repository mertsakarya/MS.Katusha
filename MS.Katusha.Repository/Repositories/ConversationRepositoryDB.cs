using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class ConversationRepositoryDB : BaseGuidRepositoryDB<Conversation>, IConversationRepositoryDB
    {
        public ConversationRepositoryDB(KatushaDbContext dbContext) : base(dbContext) { }
    }
}