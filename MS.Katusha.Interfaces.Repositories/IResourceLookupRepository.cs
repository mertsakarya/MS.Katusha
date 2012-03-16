using System.Collections.Generic;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IResourceLookupRepository : IRepository<ResourceLookup>
    {
        ResourceLookup[] GetActiveResources();
    }
}
