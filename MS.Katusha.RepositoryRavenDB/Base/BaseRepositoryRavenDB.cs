using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using NLog;
using Raven.Abstractions.Data;
using Raven.Client;


namespace MS.Katusha.Repositories.RavenDB.Base
{
    public abstract class BaseRepositoryRavenDB<T> : IRavenRepository<T> where T : BaseModel
    {
        private static Logger logger = LogManager.GetLogger("MS.Katusha.Repository.Raven");

        protected BaseRepositoryRavenDB(IDocumentStore documentStore) { DocumentStore = documentStore; }

        protected IDocumentStore DocumentStore { get; private set; }

        private IQueryable<T> QueryableRepository(bool withTracking = false)
        {
            using (var session = DocumentStore.OpenSession()) {
                return withTracking ? session.Query<T>().Where(p => p.Deleted == false).AsQueryable() : session.Query<T>().Where(p => p.Deleted == false).AsQueryable().AsNoTracking();
            }
        }

        public T GetById(long id, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Single(p => p.Id == id, includeExpressionParams);
        }

        public IQueryable<T> GetAll()
        {
            return QueryableRepository();
        }

        public IQueryable<T> GetAll(int pageNo, int pageSize)
        {
            if (pageNo < 1) return GetAll();
            return QueryableRepository().Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        private IQueryable<T> QueryHelper(Expression<Func<T, bool>> filter, bool withTracking = false)
        {
            var queryable = QueryableRepository(withTracking);
            if (filter != null) queryable = queryable.Where(filter);
            return queryable;
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderByClause, bool ascending, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            IQueryable<T> q = QueryHelper(filter);
            if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
#if DEBUG
            logger.Info(String.Format("Query<{0}>({1}, {2})", typeof(T).Name, filter, orderByClause));
#endif
            return q;
        }

        public IQueryable<T> Query<TKey>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, TKey>> orderByClause, bool ascending, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            IQueryable<T> q = QueryHelper(filter);
            total = q.Count();
            if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
#if DEBUG
            logger.Info(String.Format("Query<{0}>({1}, {2}, {3}, {4})", typeof(T).Name, filter, pageNo, pageSize, orderByClause));
#endif
            return q.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        public T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("Single<{0}>({1})", typeof(T), filter));
#endif   
            return QueryHelper(filter).FirstOrDefault();
        }

        public T SingleAttached(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
#if DEBUG
            logger.Info(String.Format("SingleAttached<{0}>({1})", typeof(T).Name, filter));
#endif
            return QueryHelper(filter, true).FirstOrDefault();
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
            entity.ModifiedDate = DateTime.UtcNow;
            entity.CreationDate = entity.ModifiedDate;
            entity.DeletionDate = new DateTime(1900, 1, 1, 0, 0, 0);
            entity.Deleted = false;
            return AddRavenDB(entity);
        }

        public T FullUpdate(T entity)
        {
#if DEBUG
            logger.Info(String.Format("FullUpdate<{0}>({1})", typeof(T).Name, entity));
#endif
            entity.ModifiedDate = DateTime.UtcNow;
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
            entity.DeletionDate = DateTime.UtcNow;
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