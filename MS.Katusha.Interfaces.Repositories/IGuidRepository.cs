using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IGuidRepository<T>: IRepository<T> where T : BaseGuidModel
    {
        T Add(T entity, Guid guid);
        T GetByGuid(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams);
    }
}

