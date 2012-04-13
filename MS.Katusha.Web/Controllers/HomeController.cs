using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;

namespace MS.Katusha.Web.Controllers
{
    public class HomeController : KatushaController
    {
        public HomeController(IUserService service, IStateService stateService)
            : base(service, stateService)
        {
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
    }
}
