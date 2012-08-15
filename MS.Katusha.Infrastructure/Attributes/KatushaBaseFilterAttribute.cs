using System;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Exceptions.Web;

namespace MS.Katusha.Infrastructure.Attributes
{
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
}