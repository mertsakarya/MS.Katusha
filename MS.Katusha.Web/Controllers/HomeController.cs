using System;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;

namespace MS.Katusha.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IKatushaMembershipService _service;

        public HomeController(IKatushaMembershipService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            Session["MERTIKO"] = "MERT";
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


        public ActionResult MailConfirm(string id)
        {
            User user = _service.ConfirmEMailAddresByGuid(Guid.Parse(id));
            var model = new MailConfirmModel { UserName = user.UserName };
            return View(model);
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