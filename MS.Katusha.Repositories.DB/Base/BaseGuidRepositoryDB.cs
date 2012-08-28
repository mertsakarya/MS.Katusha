using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Context;

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

    public abstract class BaseDetailGuidRepositoryDB<T> : BaseGuidRepositoryDB<T>, IDetailGuidRepository<T> where T : BaseGuidModel
    {
        protected BaseDetailGuidRepositoryDB(IKatushaDbContext context) : base(context) { }
        public abstract IList<T> GetAllByKey<TKey>(long id, out int total, int pageNo, int pageSize, Expression<Func<T, TKey>> orderByClause, bool @ascending);
    }
}