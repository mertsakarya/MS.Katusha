using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface ISearchService
    {
        SearchProfileResult SearchProfiles(SearchProfileCriteria searchCriteria, int pageNo = 1, int pageSize = 50);
        SearchStateResult SearchStates(SearchStateCriteria searchCriteria, int pageNo = 1, int pageSize = 50);
    }
}
