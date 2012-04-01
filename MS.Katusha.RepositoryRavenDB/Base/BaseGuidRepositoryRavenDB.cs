using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using Raven.Client;

namespace MS.Katusha.Repositories.RavenDB.Base
{
    public abstract class BaseGuidRepositoryRavenDB<T> : BaseRepositoryRavenDB<T>, IGuidRepository<T> where T : BaseGuidModel
    {
        //protected BaseGuidRepositoryRavenDB(string connectionStringName = "KatushaRavenDB") //: base(connectionStringName)
        //{
        //}

        protected BaseGuidRepositoryRavenDB(IDocumentStore documentStore)
            : base(documentStore)
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