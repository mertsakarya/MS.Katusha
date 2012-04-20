using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Helpers.Converters;
using MS.Katusha.Web.Models.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers
{
    public class KatushaController : Controller
    {
        public User KatushaUser { get; set; }
        public Profile KatushaProfile { get; set; }

        protected IUserService UserService { get; set; }
        protected IProfileService ProfileService { get; set; }
        protected IStateService StateService { get; set; }
        protected IConversationService ConversationService { get; set; }
        private const int ProfileCount = 8;

        public KatushaController(IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
        {
            ConversationService = conversationService;
            ProfileService = profileService;
            UserService = userService;
            StateService = stateService;
            UniqueVisitorsResultConverter.GetInstance().ProfileService = profileService;
            ConversationResultTypeConverter.GetInstance().ProfileService = profileService;
        }

        protected bool IsKeyForProfile(string key)
        {
            if (KatushaUser == null) return false;
            Guid guid;
            if (Guid.TryParse(key, out guid))
                return KatushaUser.Guid == guid;
            if (KatushaProfile == null) return false;
            return (KatushaProfile.FriendlyName == key);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            KatushaUser = (User.Identity.IsAuthenticated) ? UserService.GetUser(User.Identity.Name) : null;
            if (KatushaUser != null) {
                KatushaProfile = (KatushaUser.Gender > 0) ? UserService.GetProfile(KatushaUser.Guid) : null;
                int total;
                IEnumerable<Profile> newProfiles = null;
                if (KatushaProfile != null)
                    newProfiles = KatushaProfile.Gender == (byte)Sex.Male
                        ? ProfileService.GetNewProfiles(p => p.Gender == (byte)Sex.Female, out total, 1, ProfileCount)
                        : ProfileService.GetNewProfiles(p => p.Gender == (byte)Sex.Male, out total, 1, ProfileCount);
                if(newProfiles != null)
                    ViewBag.KatushaNewProfiles = Mapper.Map<IEnumerable<ProfileModel>>(newProfiles);

                IEnumerable<State> onlineStates = null;
                switch (KatushaUser.Gender) {
                    case (byte)Sex.Male:
                        onlineStates = StateService.OnlineGirls(out total, 1, ProfileCount).ToList();
                        break;
                    case (byte)Sex.Female:
                        onlineStates = StateService.OnlineMen(out total, 1, ProfileCount).ToList();
                        break;
                    default:
                        onlineStates = StateService.OnlineProfiles(out total, 1, ProfileCount).ToList();
                        break;
                }
                var onlineProfiles = new List<Profile>();
                onlineProfiles.AddRange(onlineStates.Select(state => ProfileService.GetProfile(state.ProfileId)));
                if(onlineProfiles.Count > 0)
                    ViewBag.KatushaOnlineProfiles = Mapper.Map<IEnumerable<ProfileModel>>(onlineProfiles);
            }
            ViewBag.KatushaUser = KatushaUser;
            ViewBag.KatushaProfile = KatushaProfile;
            if (KatushaProfile != null) {
                ViewBag.ConversationCount = ConversationService.GetConversationStatistics(KatushaProfile.Id);
                ViewBag.NewVisits = StateService.Ping(KatushaProfile.Id, (Sex)KatushaProfile.Gender);
            } else {
                ViewBag.ConversationCount = new ConversationCountResult();
                ViewBag.NewVisits = new NewVisits {LastVisitTime = DateTime.Now, Visits = new List<UniqueVisitorsResult>()};
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            #region internationalization
            //// Is it View ?
            //var view = filterContext.Result as ViewResultBase;
            //if (view == null) // if not exit
            //    return;

            //string cultureName = Thread.CurrentThread.CurrentCulture.Name; // e.g. "en-US" // filterContext.HttpContext.Request.UserLanguages[0]; // needs validation return "en-us" as default            

            //// Is it default culture? exit
            //if (cultureName == CultureHelper.GetDefaultCulture())
            //    return;

            //// Are views implemented separately for this culture?  if not exit
            //bool viewImplemented = CultureHelper.IsViewSeparate(cultureName);
            //if (viewImplemented == false)
            //    return;

            //string viewName = view.ViewName;

            //int i;

            //if (string.IsNullOrEmpty(viewName))
            //    viewName = filterContext.RouteData.Values["action"] + "." + cultureName; // Index.en-US
            //else if ((i = viewName.IndexOf('.')) > 0) {
            //    // contains . like "Index.cshtml"                
            //    viewName = viewName.Substring(0, i + 1) + cultureName + viewName.Substring(i);
            //} else
            //    viewName += "." + cultureName; // e.g. "Index" ==> "Index.en-Us"

            //view.ViewName = viewName;

            //filterContext.Controller.ViewBag._culture = "." + cultureName;
            #endregion
            
            base.OnActionExecuted(filterContext);
        }

        protected override void ExecuteCore()
        {
            var cultureCookie = Request.Cookies["_culture"];
            var cultureName = cultureCookie == null ? (Request.UserLanguages != null ? Request.UserLanguages[0] : null) : cultureCookie.Value;
            cultureName = CultureHelper.GetValidCulture(cultureName);

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);

            base.ExecuteCore();
        }
    }
}
