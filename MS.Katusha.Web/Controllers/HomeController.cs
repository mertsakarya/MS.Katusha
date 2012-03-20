using System;
using System.Linq;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Models;
using MS.Katusha.Exceptions;

namespace MS.Katusha.Web.Controllers
{
    public class HomeController : KatushaController
    {
        private readonly IConfigurationService _configurationService;

        public HomeController(IUserService service, IConfigurationService configurationService) : base(service)
        {
            _configurationService = configurationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }


        public ActionResult MailConfirm(string key)
        {
            User user = UserService.ConfirmEMailAddresByGuid(Guid.Parse(key));
            var model = new MailConfirmModel { UserName = user.UserName };
            return View(model);
        }

        public ActionResult SendConfirmationMail()
        {
            UserService.SendConfirmationMail(KatushaUser);
            return View();
        }


        public ActionResult Init()
        {
            string result = _configurationService.ResetDatabaseResources().Aggregate("", (current, line) => current + (line + "\r\n"));
            return View("KatushaError", new KatushaException("", result));
        }
    }
}

/*
 MENU
 * 
 * Find a girl
 * Find a man
 
 LEFT MENU
 * Conversations
 * Profile Visitors
 * View my profile
 * Edit my profile
 * My Pictures
 * Membership
 * New Girls 
 * New Men
 
 TOP MENU
 * Browse Members
 * Travel tps
 * Support
 
 RIGHT MENU
 * Login
 * Logout
 * Register
 * Upgrade

 DISPLAY PAGE
 * Send Message
 * Browse Photos
 * Send Gift
 */