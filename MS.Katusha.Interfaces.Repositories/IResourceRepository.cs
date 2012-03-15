using System.Collections.Generic;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IResourceRepository : IRepository<Resource>
    {
        List<Resource> GetActiveResources();
    }
}
