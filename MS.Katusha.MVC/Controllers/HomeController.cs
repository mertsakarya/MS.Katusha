using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MS.Katusha.Domain;

namespace MS.Katusha.MVC.Controllers
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
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
