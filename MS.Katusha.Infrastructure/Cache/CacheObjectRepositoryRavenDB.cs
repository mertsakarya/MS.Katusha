using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Repositories.RavenDB.Base;

namespace MS.Katusha.Infrastructure.Cache
{
    public class CacheObjectRepositoryRavenDB : BaseRepositoryRavenDB<CacheObject>
    {
        public CacheObjectRepositoryRavenDB(IKatushaRavenStore documentStore)
            : base(documentStore)
        {
            
        }
    }
}