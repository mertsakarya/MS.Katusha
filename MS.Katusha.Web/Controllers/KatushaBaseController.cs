using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Helpers.Converters;

namespace MS.Katusha.Web.Controllers
{
    public abstract class KatushaBaseController : Controller
    {
        protected User KatushaUser { get; set; }
        protected Profile KatushaProfile { get; set; }

        protected IUserService UserService { get; private set; }
        protected IProfileService ProfileService { get; private set; }
        protected IStateService StateService { get; private set; }
        protected IResourceService ResourceService { get; set; }
        protected IConversationService ConversationService { get; set; }

        protected KatushaBaseController(IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
        {
            ResourceService = resourceService;
            ConversationService = conversationService;
            ProfileService = profileService;
            UserService = userService;
            StateService = stateService;
            UniqueVisitorsResultConverter.GetInstance().ProfileService = profileService;
            ConversationResultTypeConverter.GetInstance().ProfileService = profileService;
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