using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IFriendlyNameRepository<T>: IGuidRepository<T> where T : BaseFriendlyModel
    {
        long GetProfileIdByFriendlyName(string friendlyName);
        long GetProfileIdByGuid(Guid guid);
        bool CheckIfFriendlyNameExists(string friendlyName, long id = 0);
    }

    public interface IDetailFriendlyNameRepository<T> : IFriendlyNameRepository<T>, IDetailRepository<T> where T : BaseFriendlyModel
    {
        new IList<T> GetAllByKey<TKey>(long id, out int total, int pageNo, int pageSize, Expression<Func<T, TKey>> orderByClause, bool ascending);
    }
}

