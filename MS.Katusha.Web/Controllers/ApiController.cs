using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Interfaces.Services.Models;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Controllers
{
    [KatushaApiFilter(AllowedRole = UserRole.ApiUser)]
    public class ApiController : KatushaApiController
    {
        private readonly IUtilityService _utilityService;
        private readonly ISearchService _searchService;
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

        public void Search(int? key, SearchProfileCriteriaModel model)
        {
            var data = Mapper.Map<SearchProfileCriteria>(model);
            var pageIndex = (key ?? 1);
            var searchResult = _searchService.SearchProfiles(data, pageIndex, PageSize);
            var list = searchResult.Profiles.Select(profile => profile.Guid).ToList();
            var result = new ApiSearchResultModel {
                PageIndex = pageIndex,
                PageSize = PageSize,
                Profiles = list,
                Total = searchResult.Total
            };
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
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
            var extendedProfile = _utilityService.GetExtendedProfile(KatushaUser, id);
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(extendedProfile));
                
                //Json(new { extendedProfile }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void SetProfile()
        {
            string extendedProfileText;
            using (var str = new StreamReader(Request.InputStream))
                extendedProfileText = str.ReadToEnd();
            var extendedProfile = JsonConvert.DeserializeObject<AdminExtendedProfile>(extendedProfileText);

            var lines = _utilityService.SetExtendedProfile(extendedProfile);
            Response.ContentType = "text/plain";
            if (lines.Count == 0)
                Response.Write("OK");
            else {
                foreach (var line in lines)
                    Response.Write(line + "\r\n");
            }
        }

        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
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
