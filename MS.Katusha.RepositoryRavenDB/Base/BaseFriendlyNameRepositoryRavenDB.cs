using System;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;

namespace MS.Katusha.Repositories.RavenDB.Base
{
    public abstract class BaseFriendlyNameRepositoryRavenDB<T> : BaseGuidRepositoryRavenDB<T>, IFriendlyNameRepository<T> where T : BaseFriendlyModel
    {
        public BaseFriendlyNameRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }

        public T GetByFriendlyName(string friendlyName, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Single(p => p.FriendlyName == friendlyName);
        }

        public bool CheckIfFriendlyNameExists(string friendlyName, long id = 0)
        {
            if(id <= 0)
                return Query(p => p.FriendlyName == friendlyName, null).Any();
            return Query(p => p.FriendlyName == friendlyName && p.Id != id, null).Any();
        }
    }
}