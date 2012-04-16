using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    public class HomeController : KatushaController
    {
        public HomeController(IUserService userService, IProfileService profileService, IStateService stateService)
            : base(userService, profileService, stateService)
        {
        }

        public ActionResult Index(int? key)
        {
            var pageIndex = (key ?? 1);
            int total;
            var newProfiles = ProfileService.GetNewProfiles(null, out total, pageIndex, DependencyHelper.GlobalPageSize);

            var profilesModel = Mapper.Map<IEnumerable<ProfileModel>>(newProfiles);
            var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, DependencyHelper.GlobalPageSize, total);
            var model = new PagedListModel<ProfileModel> { List = profilesAsIPagedList };
            return View(model);
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
