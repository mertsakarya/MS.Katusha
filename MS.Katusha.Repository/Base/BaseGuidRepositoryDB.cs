using System;
using System.Linq.Expressions;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;


namespace MS.Katusha.Repositories.DB.Base
{
    public abstract class BaseGuidRepositoryDB<T> : BaseRepositoryDB<T>, IGuidRepository<T> where T : BaseGuidModel
    {
        protected BaseGuidRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }


        public new T Add(T entity)
        {
            var e = RepositoryHelper.AddWithGuid(DbContext, entity);
            Save();
            return e;
        }

        public T Add(T entity, Guid guid)
        {
            var e = RepositoryHelper.AddWithGuid(DbContext, entity, guid);
            Save();
            return e;
        }
        
        public T GetByGuid(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Single(p => p.Guid == guid, includeExpressionParams);
        }
    }
}