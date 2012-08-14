using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using NLog;

namespace MS.Katusha.Repositories.DB.Base
{
    public abstract class BaseRepositoryDB<T> : IRepository<T> where T : BaseModel
    {
        protected readonly DbContext DbContext;
        private static Logger logger = LogManager.GetLogger("MS.Katusha.Repository.DB");

        protected IQueryable<T> QueryableRepository
        {
            get { 
                return DbContext.Set<T>().Where(p => !p.Deleted).AsQueryable().AsNoTracking(); 
            } 
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

        public IList<T> GetAll(out int total)
        {
            var q = QueryableRepository.ToList();
            total = q.Count();
            return q;
        }

        public IList<T> GetAll(out int total, int pageNo, int pageSize)
        {
            if(pageNo < 1) return GetAll(out total);
            var q = QueryableRepository;
            total = q.Count();
            return q.OrderByDescending(p => p.Id).Skip((pageNo - 1)*pageSize).Take(pageSize).ToList();
        }

        public IList<T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderByClause, bool ascending, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("Query<{0}>({1}, {2})", typeof(T).Name, filter, orderByClause));
#endif
            IQueryable<T> q = RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
            if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
            return q.ToList();
        }

        public IList<T> Query<TKey>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, TKey>> orderByClause, bool ascending, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("Query<{0}>({1}, {2}, {3}, {4})", typeof(T).Name, filter, pageNo, pageSize, orderByClause));
#endif
            IQueryable<T> q = RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
            total = q.Count();
            if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
            return q.Skip((pageNo - 1) * pageSize).Take( pageSize ).ToList();
        }

        public T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("Single<{0}>({1})", typeof(T), filter));
#endif            
            return RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams).FirstOrDefault();
        }

        public T SingleAttached(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("SingleAttached<{0}>({1})", typeof(T).Name, filter));
#endif
            return RepositoryHelper.Query(DbContext.Set<T>().Where(p => !p.Deleted).AsQueryable(), filter, includeExpressionParams).FirstOrDefault();
        }

        public T Add(T entity)
        {
#if DEBUG
            logger.Info(String.Format("Add<{0}>({1})", typeof(T).Name, entity));
#endif
            var ent = RepositoryHelper.Add(DbContext, entity);
            Save();
            return ent;
        }

        public T FullUpdate(T entity)
        {
#if DEBUG
            logger.Info(String.Format("FullUpdate<{0}>({1})", typeof(T).Name, entity));
#endif
            DbContext.Entry(entity).State = EntityState.Modified;
            //DbContext.Entry(oldEntity).CurrentValues.SetValues(newEntity);
            var ent = RepositoryHelper.Update(DbContext, entity);
            Save();
            return ent;
        }

        public T Delete(T entity)
        {
#if DEBUG
            logger.Info(String.Format("Delete<{0}>({1})", typeof(T).Name, entity));
#endif
            var t = RepositoryHelper.Delete(DbContext, entity);
            Save();
            return t;
        }

        public T SoftDelete(T entity)
        {
#if DEBUG
            logger.Info(String.Format("SoftDelete<{0}>({1})", typeof(T).Name, entity));
#endif
            var t = RepositoryHelper.SoftDelete(DbContext, entity);
            Save();
            return t;
        }

        public void Save()
        {
#if DEBUG
            logger.Info(String.Format("Save<{0}>()", typeof(T).Name));
#endif

            DbContext.SaveChanges();
        }
    }
}

