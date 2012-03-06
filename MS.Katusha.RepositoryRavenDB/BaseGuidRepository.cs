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
        protected BaseGuidRepository(string ravenDbConnectionString) : base(ravenDbConnectionString) { }

        public T Add(T entity, Guid guid)
        {
            entity.Guid = guid;
            return Add(entity);
        }
        
        public T GetByGuid(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Query(p => p.Guid == guid, includeExpressionParams).FirstOrDefault();
        }
    }
}