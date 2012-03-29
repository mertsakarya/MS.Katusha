using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using Raven.Abstractions.Data;

namespace MS.Katusha.Services
{
    public class SearchService : ISearchService
    {

        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;

        public SearchService(IProfileRepositoryRavenDB profileRepositoryRaven)
        {
            _profileRepositoryRaven = profileRepositoryRaven;
        }

        public IList<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total) { return _profileRepositoryRaven.Search(filter, pageNo, pageSize, out total); }
        public IDictionary<string, IEnumerable<FacetValue>> FacetSearch(Expression<Func<Profile, bool>> filter) { return _profileRepositoryRaven.FacetSearch(filter); }
    }
}
