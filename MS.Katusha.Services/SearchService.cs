using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using Raven.Abstractions.Data;

namespace MS.Katusha.Services
{
    public class SearchService : ISearchService
    {

        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;
        private readonly IStateRepositoryRavenDB _stateRepositoryRaven;
        private readonly IProfileService _profileService;

        public SearchService(IProfileRepositoryRavenDB profileRepositoryRaven, IStateRepositoryRavenDB stateRepositoryRaven, IProfileService profileService)
        {
            _profileRepositoryRaven = profileRepositoryRaven;
            _stateRepositoryRaven = stateRepositoryRaven;
            _profileService = profileService;
        }

        private IEnumerable<Profile> SearchProfiles(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total)
        {
            return _profileRepositoryRaven.Search(filter, pageNo, pageSize, out total);
        }

        private IEnumerable<Profile> SearchStates(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total)
        {
            var stateList = _stateRepositoryRaven.Search(filter, pageNo, pageSize, out total);
            var profileList = new List<Profile>(stateList.Count);
            profileList.AddRange(stateList.Select(item => _profileService.GetProfile(item.ProfileId)));
            return profileList;
        }

        private IDictionary<string, IEnumerable<FacetValue>> FacetSearch<T>(Expression<Func<T, bool>> filter, string facetName)
        {
            return _profileRepositoryRaven.FacetSearch(filter, facetName);
        }

        public SearchResult SearchProfiles(ISearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50)
        {
            //if (searchCriteria.CanSearch) {
                int total;
                var filter = GetFilter<Profile>(searchCriteria);
                var facetFilter = GetFilter<ProfileFacet>(searchCriteria);
                var facetSearch = FacetSearch(facetFilter, "ProfileFacets");
                var profiles = SearchProfiles(filter, pageNo, pageSize, out total);
                return new SearchResult { Profiles = profiles, FacetValues = facetSearch, SearchCriteria = searchCriteria, Total = total };
            //}
            //return new SearchResult { Profiles = null, FacetValues = null, SearchCriteria = searchCriteria, Total = -1 };
        }

        public SearchResult SearchStates(ISearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50)
        {
            //if (searchCriteria.CanSearch) {
                int total;
                var filter = GetFilter<State>(searchCriteria);
                var facetFilter = GetFilter<StateFacet>(searchCriteria);
                var facetSearch = FacetSearch(facetFilter, "StateFacets");
                var profiles = SearchStates(filter, pageNo, pageSize, out total);
                return new SearchResult { Profiles = profiles, FacetValues = facetSearch, SearchCriteria = searchCriteria, Total = total };
            //}
            //return new SearchResult { Profiles = null, FacetValues = null, SearchCriteria = searchCriteria, Total = -1 };
        }

        private static Expression<Func<T, bool>> GetFilter<T>(ISearchCriteria searchExpression)
        {
            var type = typeof (T);
            var argParam = Expression.Parameter(type, "p");
            var expression = searchExpression.GetFilter(argParam);
            var filter = Expression.Lambda<Func<T, bool>>(expression, argParam);
            return filter;
        }
    }
}
