using System;
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
}

