using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers
{
    public class KatushaController : KatushaBaseController
    {
        private const int ProfileCount = 8;

        protected KatushaController(IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
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
            var googleAnalytics = new GoogleAnalytics();
            KatushaUser = (User.Identity.IsAuthenticated) ? UserService.GetUser(User.Identity.Name) : null;
            if (KatushaUser != null) {
                KatushaProfile = (KatushaUser.Gender > 0) ? UserService.GetProfile(KatushaUser.Guid) : null;
            }
            var isPing = (filterContext.ActionDescriptor.ActionName == "Ping");
            if (!isPing) {
                if (KatushaUser != null) {
                    googleAnalytics.AddVisitorLevelVariable(GoogleAnalyticsVisitorLevelVariableType.Gender, Enum.GetName(typeof(Sex), KatushaUser.Gender));
                    googleAnalytics.AddVisitorLevelVariable(GoogleAnalyticsVisitorLevelVariableType.CategoryType, KatushaUser.Guid.ToString());
                }
                googleAnalytics.AddSessionLevelVariable(GoogleAnalyticsSessionLevelVariableType.Login, (KatushaUser != null) ? "true" : "false");
                if (KatushaProfile != null) {
                    int total;
                    var oppositeGender = (byte) ((KatushaProfile.Gender == (byte) Sex.Female) ? Sex.Male : Sex.Female);
                    var newProfiles = ProfileService.GetNewProfiles(p => p.Gender == oppositeGender, out total, 1, ProfileCount);
                    ViewBag.KatushaNewProfiles = Mapper.Map<IEnumerable<ProfileModel>>(newProfiles);
                    var onlineProfiles = new List<Profile>();
                    var onlineStates = StateService.OnlineProfiles(oppositeGender, out total, 1, ProfileCount).ToList();
                    onlineProfiles.AddRange(onlineStates.Select(state => ProfileService.GetProfile(state.ProfileId)));
                    if (onlineProfiles.Count > 0)
                        ViewBag.KatushaOnlineProfiles = Mapper.Map<IEnumerable<ProfileModel>>(onlineProfiles);
                    KatushaState = StateService.GetState(KatushaProfile);
                }
            }
            ViewBag.KatushaState = KatushaState;
            ViewBag.KatushaUser = KatushaUser;
            ViewBag.KatushaProfile = KatushaProfile;
            ViewBag.GoogleAnalytics = googleAnalytics;
        }

        protected ActionResult ContextDependentView(object model = null, string viewName = "")
        {
            var actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null) {
                ViewBag.FormAction = "Json" + actionName;
                return PartialView(viewName, model);
            }
            ViewBag.FormAction = actionName;
            return View(viewName, model);
        }

        protected IEnumerable<string> GetErrorsFromModelState() { return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage)); }


        #region OLD CODE OnActionExecuted
        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    #region internationalization
        //    //// Is it View ?
        //    //var view = filterContext.Result as ViewResultBase;
        //    //if (view == null) // if not exit
        //    //    return;

        //    //string cultureName = Thread.CurrentThread.CurrentCulture.Name; // e.g. "en-US" // filterContext.HttpContext.Request.UserLanguages[0]; // needs validation return "en-us" as default            

        //    //// Is it default culture? exit
        //    //if (cultureName == CultureHelper.GetDefaultCulture())
        //    //    return;

        //    //// Are views implemented separately for this culture?  if not exit
        //    //bool viewImplemented = CultureHelper.IsViewSeparate(cultureName);
        //    //if (viewImplemented == false)
        //    //    return;

        //    //string viewName = view.ViewName;

        //    //int i;

        //    //if (string.IsNullOrEmpty(viewName))
        //    //    viewName = filterContext.RouteData.Values["action"] + "." + cultureName; // Index.en-US
        //    //else if ((i = viewName.IndexOf('.')) > 0) {
        //    //    // contains . like "Index.cshtml"                
        //    //    viewName = viewName.Substring(0, i + 1) + cultureName + viewName.Substring(i);
        //    //} else
        //    //    viewName += "." + cultureName; // e.g. "Index" ==> "Index.en-Us"

        //    //view.ViewName = viewName;

        //    //filterContext.Controller.ViewBag._culture = "." + cultureName;
        //    #endregion
            
        //    base.OnActionExecuted(filterContext);
        //}
        #endregion
    }
}
