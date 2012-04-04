using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using Raven.Abstractions.Data;

namespace MS.Katusha.Interfaces.Services
{
    public interface ISearchService
    {
        IList<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total);
        IDictionary<string, IEnumerable<FacetValue>> FacetSearch(Expression<Func<Profile, bool>> filter);
        SearchResult Search(SearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50);
    }
}
