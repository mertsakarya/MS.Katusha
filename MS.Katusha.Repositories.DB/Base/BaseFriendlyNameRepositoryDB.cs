using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Domain;

namespace MS.Katusha.Repositories.DB.Base
{
    public abstract class BaseFriendlyNameRepositoryDB<T> : BaseGuidRepositoryDB<T>, IFriendlyNameRepository<T> where T : BaseFriendlyModel
    {
        protected BaseFriendlyNameRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }
        
        public long GetProfileIdByFriendlyName(string friendlyName)
        {
            var item = Query(p => p.FriendlyName == friendlyName, null, false, null).FirstOrDefault();
            return item != null ? item.Id : 0;
        }

        public long GetProfileIdByGuid(Guid guid) {
            var item = Query(p => p.Guid == guid, null, false, null).FirstOrDefault();
            return item != null ? item.Id : 0;
        }

        public bool CheckIfFriendlyNameExists(string friendlyName, long id = 0)
        {
            if(id <= 0)
                return Query(p => p.FriendlyName == friendlyName, null, false).Any();
            return Query(p => p.FriendlyName == friendlyName && p.Id != id, null, false).Any();
        }
    }

    public abstract class BaseDetailFriendlyNameRepositoryDB<T> : BaseFriendlyNameRepositoryDB<T>, IDetailFriendlyNameRepository<T> where T : BaseFriendlyModel
    {
        protected BaseDetailFriendlyNameRepositoryDB(IKatushaDbContext context) : base(context) { }
        public abstract IList<T> GetAllByKey<TKey>(long id, out int total, int pageNo, int pageSize, Expression<Func<T, TKey>> orderByClause, bool @ascending);
    }
}