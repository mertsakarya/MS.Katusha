using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using NLog;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB.Base
{
    public abstract class BaseRepositoryRavenDB<T> : IRavenRepository<T> where T : BaseModel
    {
        private static Logger logger = LogManager.GetLogger("MS.Katusha.Repository.Raven");

        protected BaseRepositoryRavenDB(IKatushaRavenStore documentStore) { DocumentStore = documentStore; }

        protected IKatushaRavenStore DocumentStore { get; private set; }

        private IQueryable<T> QueryableRepository(out RavenQueryStatistics stats, bool withTracking = false)
        {
            using (var session = DocumentStore.OpenSession()) {
                return withTracking ? Queryable.Where(session.Query<T>().Statistics(out stats), p => p.Deleted == false).AsQueryable() : Queryable.Where(session.Query<T>().Statistics(out stats), p => p.Deleted == false).AsQueryable().AsNoTracking();
            }
        }

        public T GetById(long id, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            //return Single(p => p.Id == id, includeExpressionParams);
            using (var session = DocumentStore.OpenSession()) {
                var t =  session.Load<T>(id);
                return t.Deleted ? null : t;
            }
        }

        public IList<T> GetAll(out int total)
        {
            RavenQueryStatistics stats;
            var q = QueryableRepository(out stats).ToList();
            total = stats.TotalResults;
            return q;
        }

        public IList<T> GetAll(out int total, int pageNo, int pageSize)
        {
            RavenQueryStatistics stats;
            if (pageNo < 1) return GetAll(out total);
            var result =  QueryableRepository(out stats).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            total = stats.TotalResults;
            return result;
        }

        private IQueryable<T> QueryHelper(Expression<Func<T, bool>> filter, out RavenQueryStatistics stats, bool withTracking = false)
        {
            var queryable = QueryableRepository(out stats, withTracking);
            if (filter != null) queryable = queryable.Where(filter);
            return queryable;
        }

        public IList<T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderByClause, bool ascending, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            RavenQueryStatistics stats;
            IQueryable<T> q = QueryHelper(filter, out stats);
            if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
#if DEBUG
            logger.Info(String.Format("Query<{0}>({1}, {2})", typeof(T).Name, filter, orderByClause));
#endif
            return q.ToList();
        }

        public IList<T> Query<TKey>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, TKey>> orderByClause, bool ascending, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            RavenQueryStatistics stats;
            IQueryable<T> q = QueryHelper(filter, out stats);
            if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
#if DEBUG
            logger.Info(String.Format("Query<{0}>({1}, {2}, {3}, {4})", typeof(T).Name, filter, pageNo, pageSize, orderByClause));
#endif
            var result =  q.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            total = stats.TotalResults;
            return result;
        }

        public T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("Single<{0}>({1})", typeof(T), filter));
#endif   
            RavenQueryStatistics stats;
            return QueryHelper(filter, out stats).FirstOrDefault();
        }

        public T SingleAttached(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("SingleAttached<{0}>({1})", typeof(T).Name, filter));
#endif
            RavenQueryStatistics stats;
            return QueryHelper(filter, out stats, true).FirstOrDefault();
        }

        private T AddRavenDB(T entity)
        {
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(entity);
                session.SaveChanges();
                return entity;
            }
        }

        public T Add(T entity)
        {
#if DEBUG
            logger.Info(String.Format("Add<{0}>({1})", typeof(T).Name, entity));
#endif
            entity.ModifiedDate = DateTime.Now;
            entity.CreationDate = entity.ModifiedDate;
            entity.DeletionDate = new DateTime(1900, 1, 1);
            entity.Deleted = false;
            return AddRavenDB(entity);
        }

        public T FullUpdate(T entity)
        {
#if DEBUG
            logger.Info(String.Format("FullUpdate<{0}>({1})", typeof(T).Name, entity));
#endif
            entity.ModifiedDate = DateTime.Now;
            return AddRavenDB(entity);
        }

        public T Delete(T entity)
        {
#if DEBUG
            logger.Info(String.Format("Delete<{0}>({1})", typeof(T).Name, entity));
#endif
            var name = String.Format("{0}s/{1}", typeof (T).Name.ToLower(CultureInfo.CreateSpecificCulture("en-US")), entity.Id);
            DocumentStore.DatabaseCommands.Delete(name, null);
            return entity;
        }

        public T SoftDelete(T entity)
        {
#if DEBUG
            logger.Info(String.Format("SoftDelete<{0}>({1})", typeof(T).Name, entity));
#endif
            entity.Deleted = true;
            entity.DeletionDate = DateTime.Now;
            return Add(entity);
        }

        public void Save()
        {
#if DEBUG
            logger.Info(String.Format("Save<{0}>()", typeof(T).Name));
#endif
        }

        public void Patch(long id, PatchRequest[] patchRequests)
        {
#if DEBUG
            logger.Info(String.Format("Patch<{0}>({1})", typeof(T).Name, id));
#endif
            DocumentStore.DatabaseCommands.Patch(id.ToString(), patchRequests);
        }
    }
}