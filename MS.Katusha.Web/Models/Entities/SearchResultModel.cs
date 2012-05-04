using System.Collections.Generic;
using MS.Katusha.Domain.Entities.BaseEntities;
using PagedList;
using Raven.Abstractions.Data;

namespace MS.Katusha.Web.Models.Entities
{
    public abstract class BaseSearchResultModel : BaseModel
    {
        public int Total { get; set; }
        public StaticPagedList<ProfileModel> Profiles { get; set; }
        public IDictionary<string, IEnumerable<FacetValue>> FacetValues { get; set; }
        public abstract BaseSearchCriteriaModel GetSearchCriteria();
    }
    public class SearchStateResultModel : BaseSearchResultModel
    {
        public SearchStateCriteriaModel SearchCriteria { get; set; }
        public override BaseSearchCriteriaModel GetSearchCriteria() { return SearchCriteria; }
    }

    public class SearchProfileResultModel : BaseSearchResultModel
    {
        public SearchProfileCriteriaModel SearchCriteria { get; set; }
        public override BaseSearchCriteriaModel GetSearchCriteria() { return SearchCriteria; }
    }
}