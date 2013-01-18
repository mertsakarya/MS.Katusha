using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class SearchResult : BaseModel
    {
        public int Total { get; set; }
        public IEnumerable<Profile> Profiles { get; set; }
        public IDictionary<string, IEnumerable<FacetData>> FacetValues { get; set; }
        public ISearchCriteria SearchCriteria { get; set; }
    }

}