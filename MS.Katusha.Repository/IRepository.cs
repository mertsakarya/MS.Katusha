using System;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Repository
{
    public interface IRepository<T> where T : BaseModel
    {
        T GetById(long id, params Expression<Func<T, object>>[] includeExpressionParams);

        IQueryable<T> Query(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams);

        T Add(T entity);
        T FullUpdate(T entity);
        T Delete(T entity);
    }
}

