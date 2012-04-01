using MS.Katusha.Repositories.RavenDB.Base;
using Raven.Client;

namespace MS.Katusha.Infrastructure.Cache
{
    public class CacheObjectRepositoryRavenDB : BaseRepositoryRavenDB<CacheObject>
    {
        public CacheObjectRepositoryRavenDB(IDocumentStore documentStore) : base(documentStore) {
            
        }
    }
}