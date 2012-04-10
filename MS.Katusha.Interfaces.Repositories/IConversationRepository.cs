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
    }
}
