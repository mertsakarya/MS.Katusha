using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.IRepositories;

namespace MS.Katusha.RepositoryRavenDB
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel
    {
        protected readonly DbContext DbContext;

        protected IQueryable<T> QueryableRepository
        {
            get { return DbContext.Set<T>().AsQueryable().AsNoTracking(); }
        }

        protected BaseRepository(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public T GetById(long id, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Query(p => p.Id == id, includeExpressionParams).FirstOrDefault();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
        }

        public T Add(T entity)
        {
            return RepositoryHelper.Add(DbContext, entity);
        }

        public T FullUpdate(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            return RepositoryHelper.Update(DbContext, entity);
        }

        public T Delete(T entity)
        {
            return RepositoryHelper.Delete(DbContext, entity);
        }

    }
}

