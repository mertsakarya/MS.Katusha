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
    public abstract class KatushaBaseController : Controller, IKatushaBase
    {
        public User KatushaUser { get; set; }
        public Profile KatushaProfile { get; set; }

        public IUserService UserService { get; private set; }
        public IProfileService ProfileService { get; private set; }
        public IStateService StateService { get; private set; }
        public IResourceService ResourceService { get; set; }
        public IConversationService ConversationService { get; set; }

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