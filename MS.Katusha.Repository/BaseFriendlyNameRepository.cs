using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.IRepositories;

namespace MS.Katusha.RepositoryRavenDB
{
    public abstract class BaseFriendlyNameRepository<T> : BaseGuidRepository<T>, IFriendlyNameRepository<T> where T : BaseFriendlyModel
    {
        protected BaseFriendlyNameRepository(DbContext dbContext) : base(dbContext) { }
        
        public T GetByFriendlyName(string friendlyName, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Query(p => p.FriendlyName == friendlyName, includeExpressionParams).FirstOrDefault();
        }

        public bool CheckIfFriendlyNameExsists(string friendlyName)
        {
            return Query(p => p.FriendlyName == friendlyName).Any();
        }
    }
}