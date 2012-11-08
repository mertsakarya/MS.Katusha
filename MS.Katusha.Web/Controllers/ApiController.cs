using System;
using System.Collections.Generic;
using System.Globalization;
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
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers
{
    [KatushaApiFilter(AllowedRole = UserRole.ApiUser)]
    public class ApiController : KatushaApiController
    {
        private readonly IUtilityService _utilityService;
        private readonly ISearchService _searchService;
        private const int PageSize = 1000;

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
            list.AddRange(searchResult.Profiles.Select(profile => new ApiProfileInfo {Id = profile.Id, Guid = profile.Guid, Name = profile.Name, Email = profile.User.Email, UserName = profile.User.UserName, ProfilePhotoGuid = profile.ProfilePhotoGuid, Country = profile.Location.CountryName, Age = DateTime.Now.Year - profile.BirthYear}));
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
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void GetProfilesByTime(string key)
        {
            DateTime dateTime;
            Response.ContentType = "application/json";
            if (DateTime.TryParse(key, out dateTime)) {
                var result = ProfileService.GetProfilesByTime(dateTime);
                Response.Write(JsonConvert.SerializeObject(result));
            } else Response.Write("{error:'wrong date'}");
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

        //curl -u mertiko:690514 http://www.mskatusha.com/Api/GetDialogs/{GUID}
        //curl -u mertiko:690514 http://www.mskatusha.com/Api/GetDialog/{FROMGUID}/{TOGUID}
        //curl -u mertiko:690514 http://www.mskatusha.com/Api/DeleteDialogs/{GUID}
        //curl -u mertiko:690514 http://www.mskatusha.com/Api/DeleteDialog/{FROMGUID}/{TOGUID}
        //curl -u mertiko:690514 http://www.mskatusha.com/Api/DeleteMessage/{GUID}
        //curl -u mertiko:690514 http://www.mskatusha.com/Api/DeleteAllMessages/{GUID}

        [HttpGet]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void GetDialogs(string key)
        {
            var dialogs = GetConversations(key);
            Response.ContentType = "application/json";
            if (dialogs == null) Response.Write("{error:'Profile not found.', message:'" + key + "'}");
            else Response.Write(JsonConvert.SerializeObject(dialogs));
        }

        [HttpGet]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void GetDialog(string from, string to)
        {
            int total;
            var messages = GetConversation(from, to, out total);
            Response.ContentType = "application/json";
            if (messages == null)
                Response.Write("{error:'Profile not found.', message:'" + from + "' or '" + to + "'}");
            else
                Response.Write(JsonConvert.SerializeObject(new {
                    Statistics = new { Count = total, From = from, To = to },
                    Messages = messages
                }));
        }

        [HttpGet]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void DeleteDialogs(string key)
        {
            var errors = new List<string[]>();
            var dialogs = GetConversations(key);
            Response.ContentType = "application/json";
            if (dialogs == null) errors.Add( new [] {"Profile not found.", key} );
            else {
                foreach (var dialog in dialogs) {
                    var e = _deleteDialog(key, dialog.ProfileId.ToString(CultureInfo.InvariantCulture));
                    if (e.Count > 0) errors.AddRange(e);
                }
            }
            Response.Write(errors.Count > 0 ? JsonConvert.SerializeObject(new { errors = errors }) : "{status:'ok'}");

        }

        [HttpGet]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void DeleteDialog(string from, string to)
        {
            Response.ContentType = "application/json";
            var e = _deleteDialog(from, to);
            Response.Write(e.Count > 0 ? JsonConvert.SerializeObject(new { errors = e }) : "{status:'ok'}");
        }

        [HttpGet]
        [KatushaApiFilter(AllowedRole = UserRole.Administrator)]
        public void DeleteMessage(string key)
        {
            Response.ContentType = "application/json";
            var e = _deleteMessage(key);
            Response.Write(e.Count > 0 ? JsonConvert.SerializeObject(new {errors = e}) : "{status:'ok'}");
        }

        private List<string[]> _deleteDialog(string key, string to)
        {
            var errors = new List<string[]>();
            int total;
            var conversation = GetConversation(key, to, out total);
            if (conversation == null)
                errors.Add(new[] { "Dialog not found.", String.Format("{0} - {1}", key, to) });
            else
                foreach (var message in conversation) {
                    var e = _deleteMessage(message.Guid.ToString());
                    if (e.Count > 0) errors.AddRange(e);
                }
            return errors;
        }

        private List<string[]> _deleteMessage(string key)
        {
            var errors = new List<string[]>();
            Guid guid;
            if (Guid.TryParse(key, out guid)) {
                try {
                    ConversationService.DeleteMessage(guid);
                } catch (Exception ex) {
                    errors.Add(new[] { "Delete error.", ex.Message.Replace("'", "\\'") });
                }
            } else {
                errors.Add(new[] { "Guid error.", key });
            }
            return errors;
        }

        private IList<Dialog> GetConversations(string key)
        {
            long isGuid;
            var profile = ParseKey(key, out isGuid);
            if (profile != null) {
                int total;
                const int pageSize = 1000;
                var dialogs = ConversationService.GetDialogs(profile.Id, out total, 1, pageSize);
                return dialogs;
            } else return null;
        }

        private IList<Conversation> GetConversation(string from, string to, out int total)
        {
            const int pageIndex = (1);
            const int pageSize = 1000;

            long fromIsId, toIsId;
            var fromProfile = ParseKey(from, out fromIsId);
            var toProfile = ParseKey(to, out toIsId);
            var fromId = (fromIsId > 0 && fromProfile == null) ? fromIsId : fromProfile.Id;
            var toId = (toIsId > 0 && toProfile == null) ? toIsId : toProfile.Id;
            if (toId > 0 && fromId > 0) {
                return ConversationService.GetConversation(fromId, toId, out total, pageIndex, pageSize);
            }
            total = 0;
            return null;
        }

        private Profile ParseKey(string text, out long isId)
        {
            long id;
            Guid guid;

            isId = 0;

            if (long.TryParse(text, out id)) {
                isId = id;
                return ProfileService.GetProfile(id);
            }

            if (Guid.TryParse(text, out guid)) {
                id = ProfileService.GetProfileId(guid);
                return ProfileService.GetProfile(id);
            }

            var user = UserService.GetUser(text);
            if (user == null) return null;
            id = ProfileService.GetProfileId(user.Guid);
            return ProfileService.GetProfile(id);
        }

    }
}
