using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class ConversationRepositoryDB : BaseGuidRepositoryDB<Conversation>, IConversationRepositoryDB
    {
        public ConversationRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }
    }
}