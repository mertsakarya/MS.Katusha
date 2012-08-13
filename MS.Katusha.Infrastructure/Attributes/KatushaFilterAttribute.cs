using System;
using System.Web;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Infrastructure.Exceptions.Web;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,
       AllowMultiple = false)]
    public class KatushaFilterAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public KatushaFilterAttribute() { 
            ExceptionView = "KatushaException";
            IsAuthenticated = false;
            MustHaveGender = false;
            MustHaveProfile = false;
            AllowedRole = UserRole.Normal;
        }

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
        /// Default value is UserRole.Normal
        /// </summary>
        public UserRole AllowedRole { get; set; }

        /// <summary>
        /// Default view is KatushaError
        /// </summary>
        public string ExceptionView { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var user = filterContext.Controller.ViewBag.KatushaUser as User;
            var profile = filterContext.Controller.ViewBag.KatushaProfile as Profile;
            var actionName = (string)filterContext.RouteData.Values["action"];
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
            if (user != null && (long)AllowedRole > 0) {
                var userRole = (UserRole) user.UserRole;
                if ((AllowedRole & UserRole.Normal) > 0 && (userRole & UserRole.Normal) == 0)
                    throw new KatushaNotAllowedException(profile, user, "You are not a normal user.");
                if ((AllowedRole & UserRole.Editor) > 0 && (userRole & UserRole.Editor) == 0)
                    throw new KatushaNotAllowedException(profile, user, "You are not an editor.");
                if ((AllowedRole & UserRole.ApiUser) > 0 && (userRole & UserRole.ApiUser) == 0)
                    throw new KatushaNotAllowedException(profile, user, "You are not an API user.");
                if ((AllowedRole & UserRole.Administrator) > 0 && (userRole & UserRole.Administrator) == 0)
                    throw new KatushaNotAllowedException(profile, user, "You are not an administrator.");
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            
            if (filterContext == null) {
                throw new ArgumentNullException("filterContext");
            }

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled) {
                return;
            }

            Exception exception = filterContext.Exception;

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
            // ignore it.
            if (new HttpException(null, exception).GetHttpCode() != 500) {
                return;
            }

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            filterContext.Result = new ViewResult {
                ViewName = ExceptionView,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
