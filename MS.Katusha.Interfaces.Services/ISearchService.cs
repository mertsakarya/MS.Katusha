using MS.Katusha.Domain.Raven;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface ISearchService
    {
        SearchResult SearchProfiles(ISearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50);
        SearchResult SearchStates(ISearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50);
    }
}
