using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;
using Raven.Abstractions.Data;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class SearchResult : BaseModel
    {
        public SearchCriteria SearchCriteria { get; set; }
        public int Total { get; set; }
        public IEnumerable<Profile> Profiles { get; set; }
        public IDictionary<string, IEnumerable<FacetValue>> FacetValues { get; set; }
    }
}