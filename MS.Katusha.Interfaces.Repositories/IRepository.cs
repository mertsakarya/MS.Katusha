using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseModel
    {
        T GetById(long id, params Expression<Func<T, object>>[] includeExpressionParams);
        IList<T> GetAll(out int total);
        IList<T> GetAll(out int total, int pageNo, int pageSize);

        IList<T> Query(Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderByClause, bool ascending, params Expression<Func<T, object>>[] includeExpressionParams);
        IList<T> Query<TKey>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, TKey>> orderByClause, bool ascending, params Expression<Func<T, object>>[] includeExpressionParams);

        T Single(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams);
        T SingleAttached(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams);

        T Add(T entity);
        T FullUpdate(T entity);
        T Delete(T entity);
        T SoftDelete(T entity);
        void Save();
    }

    public interface IDetailRepository<T> : IRepository<T> where T : BaseModel
    {
        IList<T> GetAllByKey<TKey>(long id, out int total, int pageNo, int pageSize, Expression<Func<T, TKey>> orderByClause, bool ascending);
    }
}

