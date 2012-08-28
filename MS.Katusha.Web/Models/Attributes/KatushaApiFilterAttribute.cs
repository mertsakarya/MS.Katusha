using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using MS.Katusha.Infrastructure.Exceptions.Web;
using MS.Katusha.Configuration;

namespace MS.Katusha.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class KatushaApiFilterAttribute : KatushaBaseFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var localProtocol = ConfigurationManager.AppSettings["Protocol"];
            var requestProtocol = KatushaConfigurationManager.Instance.GetSettings().Protocol;
            if(localProtocol != requestProtocol)
                throw new KatushaNotAllowedException(null, null, "Protocol must be HTTPS");
            base.OnActionExecuting(filterContext);
        }
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
}