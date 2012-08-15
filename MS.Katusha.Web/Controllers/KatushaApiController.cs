using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Web.Controllers
{
    public class KatushaApiController : KatushaBaseController
    {
        protected KatushaApiController(IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService) 
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");
            var auth = filterContext.HttpContext.Request.Headers["authorization"];
            if (!String.IsNullOrEmpty(auth)) {
                var encodedDataAsBytes = Convert.FromBase64String(auth.Replace("Basic ", ""));
                var val = Encoding.UTF8.GetString(encodedDataAsBytes);
                var userpass = val;
                var username = userpass.Substring(0, userpass.IndexOf(':'));
                var pass = userpass.Substring(userpass.IndexOf(':') + 1);
                var user = UserService.GetUser(username);
                if (user == null || user.Password != pass) {
                    PromptBasicAuthentication(filterContext);
                } else {
                    ViewBag.KatushaUser = user;
                    KatushaUser = user;
                    if (KatushaUser != null) {
                        KatushaProfile = (KatushaUser.Gender > 0) ? UserService.GetProfile(KatushaUser.Guid) : null;
                        ViewBag.KatushaProfile = KatushaProfile;
                    }
                }
            } else {
                PromptBasicAuthentication(filterContext);
            }
        }

        private static void PromptBasicAuthentication(AuthorizationContext filterContext, string message ="")
        {
            filterContext.Result = new HttpUnauthorizedResult();
            var response = filterContext.HttpContext.Response;
            response.Clear();
            response.StatusCode = 401;
            response.AddHeader("WWW-Authenticate", "Basic realm=\"MS.Katusha" + ((String.IsNullOrWhiteSpace(message)) ? "" : " - " + message) + "\"");
            response.End();
        }

    }
}