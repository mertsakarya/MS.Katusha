using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MS.Katusha.Interfaces.Services
{
    public interface IDetailGridService<T> : IGridService<T>
    {
        IList<T> GetAllByKey<TKey>(long id, out int total, int page, int pageSize, Expression<Func<T, TKey>> orderByClause, bool ascending);
    }
}