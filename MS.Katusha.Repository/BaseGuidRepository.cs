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
        protected BaseGuidRepository(DbContext dbContext) : base(dbContext) { }


        public new T Add(T entity)
        {
            return RepositoryHelper.AddWithGuid(DbContext, entity);
        }

        public T Add(T entity, Guid guid)
        {
            return RepositoryHelper.AddWithGuid(DbContext, entity, guid);
        }
        
        public T GetByGuid(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Query(p => p.Guid == guid, includeExpressionParams).FirstOrDefault();
        }
    }
}