using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;

namespace MS.Katusha.Repositories.DB
{
    public class ConversationRepositoryDB : BaseGuidRepositoryDB<Conversation>, IConversationRepositoryDB
    {
        public ConversationRepositoryDB(KatushaDbContext dbContext) : base(dbContext) { }
    }
}