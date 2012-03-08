using MS.Katusha.Domain.Entities;

namespace MS.Katusha.IRepositories.Interfaces
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
