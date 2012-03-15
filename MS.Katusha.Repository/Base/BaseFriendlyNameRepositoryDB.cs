using System;
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
        
        public T GetByFriendlyName(string friendlyName, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Query(p => p.FriendlyName == friendlyName, null, includeExpressionParams).FirstOrDefault();
        }

        public bool CheckIfFriendlyNameExists(string friendlyName, long id = 0)
        {
            if(id <= 0)
                return Query(p => p.FriendlyName == friendlyName, null).Any();
            return Query(p => p.FriendlyName == friendlyName && p.Id != id, null).Any();
        }
    }
}