using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;

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
        IList<T> Search<T>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, object>> orderByClause, bool ascending = false);
        IDictionary<string, IEnumerable<FacetData>> FacetSearch<T>(Expression<Func<T, bool>> filter, string facetName);
    }
}
