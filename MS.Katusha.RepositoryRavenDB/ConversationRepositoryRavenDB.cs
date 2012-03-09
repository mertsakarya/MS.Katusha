using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;

namespace MS.Katusha.Repositories.RavenDB
{
    public class ConversationRepositoryRavenDB : BaseGuidRepositoryRavenDB<Conversation>, IConversationRepositoryRavenDB
    {
        public ConversationRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }
    }
}