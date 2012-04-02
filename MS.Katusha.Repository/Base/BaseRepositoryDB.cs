using System;
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
        private static Logger logger = LogManager.GetLogger("RepositoryDB");

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
#if DEBUG
            logger.Info(String.Format("GetById<{1}>({0})", id, typeof(T).Name));
#endif
            return Single(p => p.Id == id, includeExpressionParams);
        }

        public IQueryable<T> GetAll()
        {
#if DEBUG
            logger.Info(String.Format("GetAll<{0}>()", typeof(T)));
#endif
            return QueryableRepository;
        }

        public IQueryable<T> GetAll(int pageNo, int pageSize)
        {
#if DEBUG
            logger.Info(String.Format("GetByAll<{0}>({1}, {2})", typeof(T).Name, pageNo, pageSize));
#endif
            if (pageNo < 1) return GetAll();

            //TODO: IMPLEMENT ORDER BY ON BASE REPOSTORY GRACEFULLY
            return QueryableRepository.OrderByDescending(p=>p.Id).Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("Query<{0}>({1}, {2})", typeof(T).Name, filter, orderByClause));
#endif
            IQueryable<T> q = RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q;
        }

        public IQueryable<T> Query<TKey>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, TKey>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("Query<{0}>({1}, {2}, {3}, {4})", typeof(T).Name, filter, pageNo, pageSize, orderByClause));
#endif
            IQueryable<T> q = RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams);
            total = q.Count(); 
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q.Skip((pageNo - 1) * pageSize).Take( pageSize );
        }

        public T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("Single<{0}>({1})", typeof(T), filter));
#endif            
            return RepositoryHelper.Query(QueryableRepository, filter, includeExpressionParams).FirstOrDefault();
        }

        public T SingleAttached(Expression<Func<T, bool>> filter)
        {
#if DEBUG
            logger.Info(String.Format("SingleAttached<{0}>({1})", typeof(T).Name, filter));
#endif
            return RepositoryHelper.Query(DbContext.Set<T>().Where(p => !p.Deleted).AsQueryable(), filter, null).FirstOrDefault();
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

