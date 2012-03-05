using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Enums;

namespace MS.Katusha.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IKatushaService katushaService;

        public HomeController(IKatushaService katushaService)
        {
            this.katushaService = katushaService;
        }

        public ActionResult Index()
        {
            
            var id = katushaService.GetUserIdByUserName("mertsakarya4");
            var user = katushaService.GetUserById(id);
            var profile = katushaService.GetProfileById(id);
            var boy = katushaService.GetBoyById(id);
            var girl = katushaService.GetGirlById(id);
            if(boy != null)
                ViewBag.Message = String.Format("Id: {0}, Email: {1}, Menşei: {2}, Aletin Büyüklüğü: {3}   ----> ", id, user.Email, (Country) profile.From, (DickSize)boy.DickSize) +"Modify this template to jump-start your ASP.NET MVC application.";
            else if(girl != null)
                ViewBag.Message = String.Format("Id: {0}, Email: {1}, Menşei: {2}, Göğüs Büyüklüğü: {3}   ----> ", id, user.Email, (Country)profile.From, (BreastSize)girl.BreastSize) + "Modify this template to jump-start your ASP.NET MVC application.";
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