using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Repository
{
    public abstract class BaseFriendlyNameRepository<T> : BaseGuidRepository<T> where T : BaseFriendlyModel
    {
        protected BaseFriendlyNameRepository(DbContext context) : base(context) { }
        
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