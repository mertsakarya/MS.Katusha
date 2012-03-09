using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IConversationRepository : IGuidRepository<Conversation>
    {
    }
    public interface IConversationRepositoryDB : IConversationRepository
    {
    }
    public interface IConversationRepositoryRavenDB : IConversationRepository
    {
    }
}
