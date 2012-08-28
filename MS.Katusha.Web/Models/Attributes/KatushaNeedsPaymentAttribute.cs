using System;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Exceptions.Services;

namespace MS.Katusha.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true,
       AllowMultiple = false)]
    public class KatushaNeedsPaymentAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public KatushaNeedsPaymentAttribute() { 
            IsJson = false;
            ViewName = "NeedsPayment";
            HasLayout = false;

        }
          
        public string ViewName { get; set; }
        public ProductNames Product { get; set; }
        public bool IsJson { get; set; }
        public bool HasLayout { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var user = filterContext.Controller.ViewBag.KatushaUser as User;
            var arrpath = filterContext.Controller.ControllerContext.RequestContext.HttpContext.Request.FilePath.ToLowerInvariant().Split('/');
            Guid guid;
            var sameProfile = false;
            if(arrpath.Length >= 4 && arrpath[1] == "profiles" && arrpath[2] == "show" && arrpath[3].Length >= 36 && Guid.TryParse(arrpath[3].Substring(0,36), out guid)) {
                if (user == null) return;
                sameProfile = user.Guid == guid;
            }
            if (user.Gender == (byte)Sex.Male)
                if (user.Expires < DateTime.Now)
                    if (!sameProfile)
                        throw new KatushaNeedsPaymentException(user, Product); 
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");
            if (!(filterContext.Exception is KatushaNeedsPaymentException)) return;
            ActionResult result;
            if(IsJson)
                result = new JsonResult { Data = new { error = "NeedsPayment", product = Product.ToString() }, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
            else {
                var dict = new ViewDataDictionary<ProductNames>(Product);
                if (filterContext.Controller.ViewData.ContainsKey("KatushaUser")) dict.Add("KatushaUser", filterContext.Controller.ViewData["KatushaUser"]);
                if (filterContext.Controller.ViewData.ContainsKey("KatushaProfile")) dict.Add("KatushaProfile", filterContext.Controller.ViewData["KatushaProfile"]);
                dict.Add("HasLayout", HasLayout);
                result = new ViewResult { ViewName = ViewName, ViewData = dict, TempData = filterContext.Controller.TempData };
            }
            
            filterContext.Result = result;
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 200;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
