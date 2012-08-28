using System;
using System.Web.Mvc;

namespace MS.Katusha.Infrastructure.Attributes
{
    public class KatushaRequireBasicAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            if (!String.IsNullOrEmpty(req.Headers["Authorization"])) return;
            var res = filterContext.HttpContext.Response;
            res.StatusCode = 401;
            res.AddHeader("WWW-Authenticate", "Basic realm=\"Twitter\"");
            res.End();
        }
    }
}
