using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.IRepositories;
using Raven.Client.Document;

namespace MS.Katusha.RepositoryRavenDB
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel
    {
        private readonly DocumentStore _documentStore ;


        protected IQueryable<T> QueryableRepository
        {
            get
            {
                using (var session = _documentStore.OpenSession())
                {
                    return session.Query<T>().AsQueryable().AsNoTracking();
                }            
            }
        }

        protected BaseRepository(string ravenDbConnectionString)
        {
            _documentStore = new DocumentStore {ConnectionStringName = ravenDbConnectionString};
            _documentStore.Initialize();
        }

        public T GetById(long id, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            return Query(p => p.Id == id, includeExpressionParams).FirstOrDefault();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            var queryable = QueryableRepository;
            //if (includeExpressionParams != null)
            //{
            //    foreach (Expression<Func<T, object>> expression in includeExpressionParams)
            //    {
            //        queryable.Include(expression);
            //    }
            //}
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            return queryable;
        }

        public T Add(T entity)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(entity);
                session.SaveChanges();
                return entity;
            } 
        }

        public T FullUpdate(T entity)
        {
            return entity;
        }

        public T Delete(T entity)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Delete(entity);
                session.SaveChanges();
                return entity;
            }
        }
    }
}

