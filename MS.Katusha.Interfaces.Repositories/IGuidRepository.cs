using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IGuidRepository<T>: IRepository<T> where T : BaseGuidModel
    {
        T Add(T entity, Guid guid);
        T GetByGuid(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams);
    }

    public interface IDetailGuidRepository<T> : IGuidRepository<T>, IDetailRepository<T> where T : BaseGuidModel
    {
        new IList<T> GetAllByKey<TKey>(long id, out int total, int pageNo, int pageSize, Expression<Func<T, TKey>> orderByClause, bool ascending);
    }
}

