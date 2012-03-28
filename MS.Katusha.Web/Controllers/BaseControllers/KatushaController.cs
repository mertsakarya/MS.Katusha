using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Domain.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers.BaseControllers
{
    public class KatushaController : Controller
    {
        public User KatushaUser { get; set; }
        public Profile KatushaProfile { get; set; }

        public Sex Gender { get; set; }

        public IUserService UserService { get; private set; }

        public KatushaController(IUserService userService)
        {
            UserService = userService;
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
            if(KatushaUser != null)
                KatushaProfile = (KatushaUser.Gender > 0) ? UserService.GetProfile(KatushaUser.Guid) : null;
            ViewBag.KatushaUser = KatushaUser;
            ViewBag.KatushaProfile = KatushaProfile;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Is it View ?
            var view = filterContext.Result as ViewResultBase;
            if (view == null) // if not exit
                return;

            string cultureName = Thread.CurrentThread.CurrentCulture.Name; // e.g. "en-US" // filterContext.HttpContext.Request.UserLanguages[0]; // needs validation return "en-us" as default            

            // Is it default culture? exit
            if (cultureName == CultureHelper.GetDefaultCulture())
                return;

            // Are views implemented separately for this culture?  if not exit
            bool viewImplemented = CultureHelper.IsViewSeparate(cultureName);
            if (viewImplemented == false)
                return;

            string viewName = view.ViewName;

            int i;

            if (string.IsNullOrEmpty(viewName))
                viewName = filterContext.RouteData.Values["action"] + "." + cultureName; // Index.en-US
            else if ((i = viewName.IndexOf('.')) > 0) {
                // contains . like "Index.cshtml"                
                viewName = viewName.Substring(0, i + 1) + cultureName + viewName.Substring(i);
            } else
                viewName += "." + cultureName; // e.g. "Index" ==> "Index.en-Us"

            view.ViewName = viewName;

            filterContext.Controller.ViewBag._culture = "." + cultureName;

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
