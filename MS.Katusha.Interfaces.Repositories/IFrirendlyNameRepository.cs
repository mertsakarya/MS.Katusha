using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IFriendlyNameRepository<T>: IGuidRepository<T> where T : BaseFriendlyModel
    {
        T GetByFriendlyName(string friendlyName, params Expression<Func<T, object>>[] includeExpressionParams);
        bool CheckIfFriendlyNameExists(string friendlyName, long id = 0);
    }
}

