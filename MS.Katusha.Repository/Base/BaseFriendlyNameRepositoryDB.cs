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

        public bool CheckIfFriendlyNameExsists(string friendlyName)
        {
            return Query(p => p.FriendlyName == friendlyName, null).Any();
        }
    }
}