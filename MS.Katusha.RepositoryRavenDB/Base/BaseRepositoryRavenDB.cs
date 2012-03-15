using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using Raven.Client.Document;

namespace MS.Katusha.Repositories.RavenDB.Base
{
    public abstract class BaseRepositoryRavenDB<T> : IRepository<T> where T : BaseModel
    {
        private readonly DocumentStore _documentStore ;


        public BaseRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
        {
            _documentStore = new DocumentStore { ConnectionStringName = connectionStringName };
            _documentStore.Initialize();
        }
        
        protected IQueryable<T> QueryableRepository
        {
            get
            {
                using (var session = _documentStore.OpenSession())
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

        public IQueryable<T> Query(Expression<Func<T, bool>> filter, int pageNo, int pageSize, Expression<Func<T, object>> orderByClause, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            IQueryable<T> q = QueryHelper(filter);
            if (orderByClause != null) q = q.OrderBy(orderByClause);
            return q.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        public T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return QueryHelper(filter).FirstOrDefault();
        }

        private T AddRavenDB(T entity)
        {
            using (var session = _documentStore.OpenSession())
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
            _documentStore.DatabaseCommands.Delete(name, null);
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