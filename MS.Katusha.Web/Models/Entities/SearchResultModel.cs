using System.Collections.Generic;
using MS.Katusha.Domain.Entities.BaseEntities;
using PagedList;
using Raven.Abstractions.Data;

namespace MS.Katusha.Web.Models.Entities
{
    public class SearchResultModel : BaseModel
    {
        public SearchCriteriaModel SearchCriteria { get; set; }
        public int Total { get; set; }
        public StaticPagedList<ProfileModel> Profiles { get; set; }
        public IDictionary<string, IEnumerable<FacetValue>> FacetValues { get; set; }
    }
}