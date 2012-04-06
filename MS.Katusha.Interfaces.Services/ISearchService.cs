using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface ISearchService
    {
        SearchResult Search(SearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50);
    }
}
