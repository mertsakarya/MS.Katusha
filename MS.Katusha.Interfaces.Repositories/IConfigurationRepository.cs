using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IConfigurationRepository<T>: IRepository<T> where T: BaseModel
    {
        T[] GetActiveValues();
    }
}
