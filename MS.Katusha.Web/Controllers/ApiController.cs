using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models.Entities;
using Newtonsoft.Json;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    public class ApiController : KatushaController
    {
        private readonly IUtilityService _utilityService;
        private ISearchService _searchService;
        private const int PageSize = 100;

        public ApiController(IResourceService resourceService, IUserService userService, IProfileService profileService, 
            IConversationService conversationService, IStateService stateService, IUtilityService utilityService,
            ISearchService searchService
            )
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
            _searchService = searchService;
            _utilityService = utilityService;
        }

        [HttpGet]
        [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = true, MustBeAdmin = true)]
        public void Search(int? key, SearchProfileCriteriaModel model)
        {
            var data = Mapper.Map<SearchProfileCriteria>(model);
            var pageIndex = (key ?? 1);
            var searchResult = _searchService.SearchProfiles(data, pageIndex, PageSize);
            var list = new List<Guid>();
            foreach(var profile in searchResult.Profiles)
                list.Add(profile.Guid);
            var result = new ApiSearchResultModel() {
                PageIndex = pageIndex,
                PageSize = PageSize,
                Profiles = list,
                Total = searchResult.Total
            };
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(result));
        }


        [HttpGet]
        [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = true, MustBeAdmin = true)]
        public void GetProfile(string key)
        {
            long id;
            if (!long.TryParse(key, out id)) {
                Guid guid;
                if (!Guid.TryParse(key, out guid)) {
                    var user = UserService.GetUser(key);
                    if (user == null) throw new NullReferenceException("Key invalid");
                    id = ProfileService.GetProfileId(user.Guid);
                } else {
                    id = ProfileService.GetProfileId(guid);
                }
            }
            if (id == 0) throw new NullReferenceException("Key invalid");
            var extendedProfile = _utilityService.GetExtendedProfile(id);
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(extendedProfile));
                
                //Json(new { extendedProfile }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = true, MustBeAdmin = true)]
        public void SetProfile()
        {
            string extendedProfileText;
            using (var str = new StreamReader(Request.InputStream))
                extendedProfileText = str.ReadToEnd();
            var extendedProfile = JsonConvert.DeserializeObject<ExtendedProfile>(extendedProfileText);
            if (String.IsNullOrWhiteSpace(Request.Headers["X-MSKATUSHA"])) return;
            if (Request.Headers["X-MSKATUSHA"] != "valid") return;

            
            var lines = _utilityService.SetExtendedProfile(extendedProfile);
            Response.ContentType = "text/plain";
            if (lines.Count == 0)
                Response.Write("OK");
            else {
                foreach (var line in lines)
                    Response.Write(line + "\r\n");
            }
        }

        [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = true, MustBeAdmin = true)]
        public void DeleteProfile(string key)
        {
            Guid guid;
            if (!Guid.TryParse(key, out guid)) Response.Write("Wrong Guid!");
            //GetExtendedProfile(key);
            _utilityService.DeleteProfile(guid);
            Response.Write("OK");
        }
    }
}
