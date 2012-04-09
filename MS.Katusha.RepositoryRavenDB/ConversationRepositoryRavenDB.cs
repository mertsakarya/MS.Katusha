using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using Raven.Client;

namespace MS.Katusha.Repositories.RavenDB
{
    public class ConversationRepositoryRavenDB : BaseGuidRepositoryRavenDB<ConversationRaven>, IConversationRepositoryRavenDB
    {
        public ConversationRepositoryRavenDB(IDocumentStore documentStore)
            : base(documentStore) { }
    }
}