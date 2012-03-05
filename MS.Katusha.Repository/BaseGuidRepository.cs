using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Repository
{
    public abstract class BaseGuidRepository<T> : BaseRepository<T> where T : BaseGuidModel
    {
        protected BaseGuidRepository(DbContext context) : base(context) { }


        public new T Add(T entity)
        {
            return RepositoryHelper.AddWithGuid(Context, entity);
        }

        public T Add(T entity, Guid guid)
        {
            return RepositoryHelper.AddWithGuid(Context, entity, guid);
        }
        
        public T GetByGuid(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Query(p => p.Guid == guid, includeExpressionParams).FirstOrDefault();
        }
    }
}