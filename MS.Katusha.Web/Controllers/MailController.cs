using System;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;

namespace MS.Katusha.Web.Controllers
{
    public class MailController : KatushaController
    {
        private readonly INotificationService _notificationService;

        public MailController(INotificationService notificationService, IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, profileService, stateService, conversationService) { _notificationService = notificationService; }

        public ActionResult Confirm(string key)
        {
            User user = UserService.ConfirmEMailAddresByGuid(Guid.Parse(key));
            var model = new MailConfirmModel { UserName = (user == null) ? "" : user.UserName};
            return View(model);
        }

        public ActionResult SendConfirmation()
        {
            _notificationService.UserRegistered(KatushaUser);
            return View();
        }
    }
}
