using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities.BaseEntities;
using System.Collections.Generic;
using MS.Katusha.Interfaces.Repositories;

namespace MS.Katusha.Repositories.DB.Base
{
    public abstract class BaseRepositoryDB<T> : IRepository<T> where T : BaseModel
    {
        protected readonly DbContext DbContext;

        protected IQueryable<T> QueryableRepository
        {
            get { return DbContext.Set<T>().Where(p => !p.Deleted).AsQueryable().AsNoTracking(); }
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

        public IQueryable<T> GetAll()
        {
            return QueryableRepository;
        }

        public IQueryable<T> GetAll(int pageNo, int pageSize)
        {
            if (pageNo < 1) return GetAll();

            //TODO: IMPLEMENT ORDER BY ON BASE REPOSTORY GRACEFULLY
            return QueryableRepository.OrderByDescending(p=>p.Id).Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            IQueryable<T> q = RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q;
        }

        public IQueryable<T> Query<TKey>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, TKey>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {           
            IQueryable<T> q = RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
            total = q.Count(); 
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q.Skip((pageNo - 1) * pageSize).Take( pageSize );
        }

        public T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams).FirstOrDefault();
        }

        public T SingleAttached(Expression<Func<T, bool>> filter)
        {
            return RepositoryHelper.Query(DbContext.Set<T>().Where(p => !p.Deleted).AsQueryable(), filter, null).FirstOrDefault();
        }

        public T Add(T entity)
        {
            var ent = RepositoryHelper.Add(DbContext, entity);
            Save();
            return ent;
        }

        public T FullUpdate(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            var ent = RepositoryHelper.Update(DbContext, entity);
            Save();
            return ent;
        }

        public T Delete(T entity)
        {
            var t = RepositoryHelper.Delete(DbContext, entity);
            Save();
            return t;
        }

        public T SoftDelete(T entity)
        {
            var t = RepositoryHelper.SoftDelete(DbContext, entity);
            Save();
            return t;
        }

        public void Save()
        {
            DbContext.SaveChanges();
        }

    }
}

