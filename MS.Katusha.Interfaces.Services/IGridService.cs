using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MS.Katusha.Interfaces.Services
{
    public interface IGridService<T>
    {
        IList<T> GetAll(out int total, int page, int pageSize);
        void Add(T entity);
        T GetById(long id);
        void Update(T entity);
        void Delete(T entity);
    }

    public interface IDetailGridService<T> : IGridService<T>
    {
        IList<T> GetAllByKey<TKey>(long id, out int total, int page, int pageSize, Expression<Func<T, TKey>> orderByClause, bool ascending);
    }
}
