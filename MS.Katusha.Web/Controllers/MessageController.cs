using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Controllers
{
    public class MessageController : KatushaController
    {
        public MessageController(IUserService userService) : base(userService) {}

        public ActionResult Send(string key)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Send(string key, ConversationModel model)
        {
            return View();
        }

    }
}
