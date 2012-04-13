using System.Collections.Generic;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IConversationRepository : IGuidRepository<MS.Katusha.Domain.Entities.Conversation>
    {
    }
    public interface IConversationRepositoryDB : IConversationRepository
    {
    }
    public interface IConversationRepositoryRavenDB : IRavenGuidRepository<MS.Katusha.Domain.Raven.Entities.Conversation>
    {
        IList<ConversationResult> MyConversations(long profileId, out int total, int pageNo = 1, int pageSize = 20);
    }
}
