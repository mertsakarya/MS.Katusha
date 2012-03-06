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
        protected readonly DbContext Context;

        protected IQueryable<T> QueryableRepository
        {
            get { return Context.Set<T>().AsQueryable().AsNoTracking(); }
        }

        protected BaseRepository(DbContext context)
        {
            Context = context;
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
            return RepositoryHelper.Add(Context, entity);
        }

        public T FullUpdate(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return RepositoryHelper.Update(Context, entity);
        }

        public T Delete(T entity)
        {
            return RepositoryHelper.Delete(Context, entity);
        }
    }
}

