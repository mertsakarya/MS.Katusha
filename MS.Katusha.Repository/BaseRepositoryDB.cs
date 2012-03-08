using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.IRepositories;
using System.Collections.Generic;

namespace MS.Katusha.RepositoryDB
{
    public abstract class BaseRepositoryDB<T> : IRepository<T> where T : BaseModel
    {
        protected readonly DbContext DbContext;

        protected IQueryable<T> QueryableRepository
        {
            get { return DbContext.Set<T>().AsQueryable().AsNoTracking(); }
        }


        protected BaseRepositoryDB(IKatushaDbContext context)
        {
            var dbContext = context as DbContext;
            DbContext = dbContext;
        }

        public T GetById(long id, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Single(p => p.Id == id, includeExpressionParams);
        }

        public IEnumerable<T> GetAll()
        {
            return QueryableRepository.ToArray();
        }

        public IEnumerable<T> GetAll(int pageNo, int pageSize)
        {
            if (pageNo < 1) return GetAll();
            return QueryableRepository.Skip((pageNo - 1) * pageSize).Take(pageSize).ToArray();
        }

        public IEnumerable<T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            IQueryable<T> q = RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q.ToArray();
        }

        public IEnumerable<T> Query(Expression<Func<T, bool>> filter, int pageNo, int pageSize, Expression<Func<T, object>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            IQueryable<T> q = RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q.Skip((pageNo - 1) * pageSize).Take( pageSize ).ToArray();
        }

        public T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams).FirstOrDefault();
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

        public void Save()
        {
            DbContext.SaveChanges();
        }

    }
}

