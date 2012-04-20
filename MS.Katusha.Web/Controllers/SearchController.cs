using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class SearchController : KatushaController
    {
        private readonly ISearchService _searchService;
        private const int PageSize = DependencyHelper.GlobalPageSize;

        public SearchController(IUserService userService, IProfileService profileService, ISearchService searchService, IStateService stateService, IConversationService conversationService)
            : base(userService, profileService, stateService, conversationService)
        {
            _searchService = searchService;
        }

        public ActionResult Men(int? key, SearchCriteriaModel model) { model.Gender = Sex.Male; return Search(key, model); }
        public ActionResult Girls(int? key, SearchCriteriaModel model) { model.Gender = Sex.Female; return Search(key, model); }

        private ActionResult Search(int? key, SearchCriteriaModel model)
        {
            var data = Mapper.Map<SearchCriteria>(model);
            var pageIndex = (key ?? 1);
            var searchResult = _searchService.Search(data, pageIndex, PageSize);
            if (searchResult.Total > -1) {
                var profiles = searchResult.Profiles;
                var profilesModel = Mapper.Map<IList<ProfileModel>>(profiles);
                var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, PageSize, searchResult.Total);
                var searchResultModel = new SearchResultModel {
                                                                  FacetValues = searchResult.FacetValues,
                                                                  SearchCriteria = Mapper.Map<SearchCriteriaModel>(searchResult.SearchCriteria),
                                                                  Total = searchResult.Total,
                                                                  Profiles = profilesAsIPagedList
                                                              };
                ViewBag.KatushaSearchResult = searchResultModel;
                return View("Search", searchResultModel);
            }
            return View("Search", new SearchResultModel {SearchCriteria = model});
        }
     }
}
