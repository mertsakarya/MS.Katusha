using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models.Entities;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class SearchController : KatushaController
    {
        private readonly ISearchService _searchService;
        private const int PageSize = DependencyConfig.GlobalPageSize;

        public SearchController(IResourceService resourceService, IUserService userService, IProfileService profileService, ISearchService searchService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
            _searchService = searchService;
            ResourceService = resourceService;

        }

        public ActionResult Men(int? key, SearchProfileCriteriaModel model)
        {
            model.Gender = Sex.Male; return SearchProfile(key, model);
        }
        public ActionResult Girls(int? key, SearchProfileCriteriaModel model)
        {
            model.Gender = Sex.Female; return SearchProfile(key, model);
        }

        public ActionResult MenOnline(int? key, SearchStateCriteriaModel model)
        {
            model.Gender = Sex.Male; return SearchState(key, model);
        }

        public ActionResult GirlsOnline(int? key, SearchStateCriteriaModel model)
        {
            model.Gender = Sex.Female; return SearchState(key, model);
        }

        private ActionResult SearchProfile(int? key, SearchProfileCriteriaModel model)
        {
            var data = Mapper.Map<SearchProfileCriteria>(model);
            var pageIndex = (key ?? 1);
            var searchResult = _searchService.SearchProfiles(data, pageIndex, PageSize);
            if (searchResult.Total > -1) {
                var profiles = searchResult.Profiles;
                var profilesModel = Mapper.Map<IList<ProfileModel>>(profiles);
                var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, PageSize, searchResult.Total);
                var searchResultModel = new SearchProfileResultModel {
                    FacetValues = searchResult.FacetValues,
                    SearchCriteria = Mapper.Map<SearchProfileCriteriaModel>(searchResult.SearchCriteria),
                    Total = searchResult.Total,
                    Profiles = profilesAsIPagedList
                };
                ViewBag.KatushaSearchResult = searchResultModel;
                return View("Search", searchResultModel);
            }
            return View("Search", new SearchProfileResultModel {SearchCriteria = model});
        }

        private ActionResult SearchState(int? key, SearchStateCriteriaModel model)
        {
            var data = Mapper.Map<SearchStateCriteria>(model);
            var pageIndex = (key ?? 1);
            var searchResult = _searchService.SearchStates(data, pageIndex, PageSize);
            if (searchResult.Total > -1) {
                var profiles = searchResult.Profiles;
                var profilesModel = Mapper.Map<IList<ProfileModel>>(profiles);
                var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, PageSize, searchResult.Total);
                var searchResultModel = new SearchStateResultModel {
                    FacetValues = searchResult.FacetValues,
                    SearchCriteria = Mapper.Map<SearchStateCriteriaModel>(searchResult.SearchCriteria),
                    Total = searchResult.Total,
                    Profiles = profilesAsIPagedList
                };
                ViewBag.KatushaSearchResult = searchResultModel;
                return View("Search", searchResultModel);
            }
            return View("Search", new SearchStateResultModel { SearchCriteria = model });
        }

        public ActionResult GetCities(string query, string searching, string countryCode = "")
        {
            if (String.IsNullOrWhiteSpace(query)) return Json(new List<KeyValuePair<string, string>>(), JsonRequestBehavior.AllowGet);
            IDictionary<string, string> coll;
            searching = String.IsNullOrWhiteSpace(searching) ? "" : searching.ToLowerInvariant();
            switch (searching) {
                case "girls":
                    coll = ResourceService.GetSearchableCities(Sex.Female, countryCode);
                    break;
                case "men":
                    coll = ResourceService.GetSearchableCities(Sex.Male, countryCode);
                    break;
                default:
                    coll = ResourceService.GetCities(countryCode);
                    break;
            }
            var list = (from u in coll
                        where u.Value.StartsWith(query, StringComparison.CurrentCultureIgnoreCase) //IndexOf(query, System.StringComparison.InvariantCultureIgnoreCase) >= 0
                        select u).Take(20).ToArray();
            var dict = new List<KeyValuePair<string, string>>(list.Length);
            dict.AddRange(list.Select(item => new KeyValuePair<string, string>(item.Key, item.Value)));
            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCountries(string query, string searching)
        {
            if (String.IsNullOrWhiteSpace(query)) return Json(new List<KeyValuePair<string, string>>(), JsonRequestBehavior.AllowGet);
            IDictionary<string, string> coll;
            switch (searching.ToLowerInvariant()) {
                case "girls":
                    coll = ResourceService.GetSearchableCountries(Sex.Female);
                    break;
                case "men":
                    coll = ResourceService.GetSearchableCountries(Sex.Male);
                    break;
                default:
                    coll = ResourceService.GetCountries();
                    break;
            }
            var list = (from u in coll
                        where u.Value.StartsWith(query, StringComparison.CurrentCultureIgnoreCase) //IndexOf(query, System.StringComparison.InvariantCultureIgnoreCase) >= 0
                        select u).Take(20).ToArray();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
