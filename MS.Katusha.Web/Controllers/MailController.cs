using System;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;

namespace MS.Katusha.Web.Controllers
{
    public class MailController : KatushaController
    {
        public MailController(IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, profileService, stateService, conversationService) { }

        public ActionResult Confirm(string key)
        {
            User user = UserService.ConfirmEMailAddresByGuid(Guid.Parse(key));
            var model = new MailConfirmModel { UserName = user.UserName };
            return View(model);
        }

        public ActionResult SendConfirmation()
        {
            UserService.SendConfirmationMail(KatushaUser);
            return View();
        }
    }
}
