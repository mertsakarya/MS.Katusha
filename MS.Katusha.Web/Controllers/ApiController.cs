using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Domain.Service;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
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
            var list = new List<ApiProfileInfo>(searchResult.Profiles.Count());
            list.AddRange(searchResult.Profiles.Select(profile => new ApiProfileInfo {Guid = profile.Guid, Name = profile.Name, Email = profile.User.Email, UserName = profile.User.UserName, ProfilePhotoGuid = profile.ProfilePhotoGuid, Country = profile.Location.CountryName, Age = DateTime.Now.Year - profile.BirthYear}));
            var result = new ApiSearchResult {
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
        }

        [HttpGet]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void GetProfileGuid(string key)
        {
            Response.ContentType = "application/json";
            long id;
            if (!long.TryParse(key, out id)) {
                Guid guid;
                if (!Guid.TryParse(key, out guid)) {
                    var user = UserService.GetUser(key);
                    Response.Write(user == null ? "{userGuid:''}" : "{userGuid:'" + user.Guid + "'}");
                } else {
                    var user = UserService.GetUser(guid);
                    Response.Write(user == null ? "{userGuid:''}" : "{userGuid:'" + user.Guid + "'}");
                }
            } else {
                var user = UserService.GetUser(id);
                Response.Write(user == null ? "{userGuid:''}" : "{userGuid:'" + user.Guid + "'}");
            }
        }

        [HttpPost]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void SetProfile()
        {
            string extendedProfileText;
            using (var str = new StreamReader(Request.InputStream))
                extendedProfileText = str.ReadToEnd();
            var extendedProfile = JsonConvert.DeserializeObject<AdminExtendedProfile>(extendedProfileText);
            if(extendedProfile == null) 
                throw new ArgumentNullException("ProfileData");
            var lines = _utilityService.SetExtendedProfile(extendedProfile);
            Response.ContentType = "text/plain";
            if (lines.Count == 0)
                Response.Write("");
            else {
                foreach (var line in lines)
                    Response.Write(line + "\r\n");
            }
        }

        [HttpGet]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void UpdateRavenProfile(string key)
        {
            User user;
            long id;
            if (!long.TryParse(key, out id)) {
                Guid guid;
                if (!Guid.TryParse(key, out guid)) {
                    user = UserService.GetUser(key);
                } else {
                    user = UserService.GetUser(guid);
                }
            } else {
                user = UserService.GetUser(id);
            }
            var profileId = ProfileService.GetProfileId(user.Guid);
            ProfileService.UpdateRavenProfile(profileId);
            Response.ContentType = "application/json";
            Response.Write("{status:'ok'}");
        }

        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void DeleteProfile(string key)
        {
            Guid guid;
            if (!Guid.TryParse(key, out guid)) Response.Write("Wrong Guid!");
            Response.ContentType = "text/plain";
            try {
                _utilityService.DeleteProfile(guid);
                Response.Write("");
            } catch(Exception ex) {
                Response.Write(ex.Message);
            }
        }
    }
}
