using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;

namespace MS.Katusha.Repositories.RavenDB.Base
{
    public abstract class BaseGuidRepositoryRavenDB<T> : BaseRepositoryRavenDB<T>, IGuidRepository<T> where T : BaseGuidModel
    {
        protected BaseGuidRepositoryRavenDB(string connectionStringName = "KatushaRavenDB") : base(connectionStringName)
        {
        }

        public T Add(T entity, Guid guid)
        {
            entity.Guid = guid;
            return Add(entity);
        }
        
        public T GetByGuid(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Single(p => p.Guid == guid, includeExpressionParams);
        }
    }
}