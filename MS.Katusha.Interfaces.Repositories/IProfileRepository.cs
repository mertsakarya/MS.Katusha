using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using Raven.Abstractions.Data;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IProfileRepository : IFriendlyNameRepository<Profile>
    {
    }

    public interface IProfileRepositoryDB : IProfileRepository
    {
    }

    public interface IProfileRepositoryRavenDB : IProfileRepositoryDB, IRavenFriendlyNameRepository<Profile>
    {
        IList<T> Search<T>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total);
        IDictionary<string, IEnumerable<FacetValue>> FacetSearch<T>(Expression<Func<T, bool>> filter, string facetName);
    }
}
