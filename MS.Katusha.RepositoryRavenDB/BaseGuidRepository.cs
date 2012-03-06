using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.IRepositories;

namespace MS.Katusha.RepositoryRavenDB
{
    public abstract class BaseGuidRepository<T> : BaseRepository<T>, IGuidRepository<T> where T : BaseGuidModel
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