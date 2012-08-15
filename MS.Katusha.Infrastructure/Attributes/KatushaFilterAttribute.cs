using System;
using System.Web;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Infrastructure.Exceptions.Web;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class KatushaApiFilterAttribute : KatushaBaseFilterAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");
            var exception = filterContext.Exception;
            if (exception is KatushaNotAllowedException) {
                PromptBasicAuthentication(filterContext, exception.Message);
            } else {
                if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled) return;
                if (new HttpException(null, exception).GetHttpCode() != 500) return;

                var result = new JsonResult {Data = new {error = filterContext.Exception.Message}, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
                filterContext.Result = result;
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }

        private static void PromptBasicAuthentication(ExceptionContext filterContext, string message = "")
        {
            filterContext.Result = new HttpUnauthorizedResult();
            var response = filterContext.HttpContext.Response;
            response.Clear();
            response.StatusCode = 401;
            response.AddHeader("WWW-Authenticate", "Basic realm=\"MS.Katusha" + ((String.IsNullOrWhiteSpace(message))? "": " - " +message) + "\"");
            response.End();
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public abstract class KatushaBaseFilterAttribute : ActionFilterAttribute, IExceptionFilter
    {
        protected KatushaBaseFilterAttribute()
        {
            AllowedRole = UserRole.Normal;
        }

        public UserRole AllowedRole { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var user = filterContext.Controller.ViewBag.KatushaUser as User;
            var profile = filterContext.Controller.ViewBag.KatushaProfile as Profile;
            if (user == null || (long)AllowedRole <= 0) return;
            var userRole = (UserRole)user.UserRole;
            if ((AllowedRole & UserRole.Normal) > 0 && (userRole & UserRole.Normal) == 0)
                throw new KatushaNotAllowedException(profile, user, "You are not a normal user.");
            if ((AllowedRole & UserRole.Editor) > 0 && (userRole & UserRole.Editor) == 0)
                throw new KatushaNotAllowedException(profile, user, "You are not an editor.");
            if ((AllowedRole & UserRole.ApiUser) > 0 && (userRole & UserRole.ApiUser) == 0)
                throw new KatushaNotAllowedException(profile, user, "You are not an API user.");
            if ((AllowedRole & UserRole.Administrator) > 0 && (userRole & UserRole.Administrator) == 0)
                throw new KatushaNotAllowedException(profile, user, "You are not an administrator.");
        }

        public abstract void OnException(ExceptionContext filterContext);
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class KatushaFilterAttribute : KatushaBaseFilterAttribute
    {
        public KatushaFilterAttribute()
        {
            ExceptionView = "KatushaException";
            IsAuthenticated = false;
            MustHaveGender = false;
            MustHaveProfile = false;
            IsJson = false;
            HasLayout = true;
        }

        /// <summary>
        /// Default value is false
        /// </summary>
        public bool IsJson { get; set; }

        /// <summary>
        /// Default value is true
        /// </summary>
        public bool HasLayout { get; set; }

        /// <summary>
        /// Default value is false
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Default value is false
        /// </summary>
        public bool MustHaveGender { get; set; }

        /// <summary>
        /// Default value is false
        /// </summary>
        public bool MustHaveProfile { get; set; }

        /// <summary>
        /// Default view is KatushaException
        /// </summary>
        public string ExceptionView { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var user = filterContext.Controller.ViewBag.KatushaUser as User;
            var profile = filterContext.Controller.ViewBag.KatushaProfile as Profile;
            if (IsAuthenticated && !filterContext.HttpContext.User.Identity.IsAuthenticated)
                throw new KatushaNotAllowedException(profile, user, "You need to log in.");
            if (MustHaveProfile) {
                if (user == null || user.Gender == 0 || profile == null)
                    throw new KatushaNotAllowedException(profile, user, "You have to create a profle.");
            }
            if (MustHaveGender) {
                if (user == null || user.Gender == 0 || (MustHaveProfile && (profile == null || profile.Gender == 0)))
                    throw new KatushaNotAllowedException(profile, user, "You must have a gender. :)");
            }
        }

        public override void OnException(ExceptionContext filterContext)
        {

            if (filterContext == null) throw new ArgumentNullException("filterContext");
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled) return;
            var exception = filterContext.Exception;
            if (new HttpException(null, exception).GetHttpCode() != 500) return;
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            ActionResult result;
            if (IsJson)
                result = new JsonResult { Data = new { error = filterContext.Exception.Message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            else {
                var dict = new ViewDataDictionary<HandleErrorInfo>(model);
                if (filterContext.Controller.ViewData.ContainsKey("KatushaUser")) dict.Add("KatushaUser", filterContext.Controller.ViewData["KatushaUser"]);
                if (filterContext.Controller.ViewData.ContainsKey("KatushaProfile")) dict.Add("KatushaProfile", filterContext.Controller.ViewData["KatushaProfile"]);
                dict.Add("HasLayout", HasLayout);
                result = new ViewResult { ViewName = ExceptionView, ViewData = dict, TempData = filterContext.Controller.TempData };
            }
            filterContext.Result = result;
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
