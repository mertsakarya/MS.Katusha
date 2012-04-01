using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace MS.Katusha.Repositories.RavenDB.Base
{
    public abstract class BaseRepositoryRavenDB<T> : IRepository<T> where T : BaseModel
    {
        protected BaseRepositoryRavenDB(IDocumentStore documentStore) { DocumentStore = documentStore; }

        protected IDocumentStore DocumentStore { get; private set; }

        protected IQueryable<T> QueryableRepository
        {
            get
            {
                using (var session = DocumentStore.OpenSession())
                {
                    return session.Query<T>().Where(p => !p.Deleted).AsQueryable().AsNoTracking();
                }            
            }
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
            return QueryableRepository.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        private IQueryable<T> QueryHelper(Expression<Func<T, bool>> filter)
        {
            var queryable = QueryableRepository;
            if (filter != null) queryable = queryable.Where(filter);
            return queryable;
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            IQueryable<T> q = QueryHelper(filter);
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q;
        }

        public IQueryable<T> Query<TKey>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, TKey>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            IQueryable<T> q = QueryHelper(filter);
            total = q.Count();
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        public T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return QueryHelper(filter).FirstOrDefault();
        }

        public T SingleAttached(Expression<Func<T, bool>> filter)
        {
            //TODO: implement correcting AsNoTracking issue
            return Single(filter, null);
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
            entity.ModifiedDate = DateTime.Now.ToUniversalTime();
            return AddRavenDB(entity);
        }

        public T FullUpdate(T entity)
        {
            return Add(entity);
        }

        public T Delete(T entity)
        {
            var name = String.Format("{0}s/{1}", typeof (T).Name.ToLower(CultureInfo.CreateSpecificCulture("en-US")), entity.Id);
            DocumentStore.DatabaseCommands.Delete(name, null);
            return entity;
        }

        public T SoftDelete(T entity)
        {
            entity.Deleted = true;
            entity.DeletionDate = DateTime.Now.ToUniversalTime();
            return Add(entity);
        }

        public void Save()
        {
        }
    }
}