using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    public class HomeController : KatushaController
    {
        public HomeController(IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
        }

        public ActionResult Index(int? key)
        {
            var pageIndex = (key ?? 1);
            int total;
            IEnumerable<Domain.Entities.Profile> profiles;
            if (KatushaProfile == null) {
                profiles = ProfileService.GetNewProfiles(p => p.ProfilePhotoGuid != Guid.Empty, out total, pageIndex, DependencyConfig.GlobalPageSize);
            } else {
                if(KatushaProfile.Gender == (byte) Sex.Female) {
                    profiles = ProfileService.GetNewProfiles(p => p.Gender == (byte)Sex.Male, out total, pageIndex, DependencyConfig.GlobalPageSize);
                } else {
                    profiles = ProfileService.GetNewProfiles(p => p.Gender == (byte)Sex.Female, out total, pageIndex, DependencyConfig.GlobalPageSize);
                }
            }
            var profilesModel = Mapper.Map<IEnumerable<ProfileModel>>(profiles);
            var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, DependencyConfig.GlobalPageSize, total);
            var model = new PagedListModel<ProfileModel> { List = profilesAsIPagedList, Total = total};
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

        //[OutputCache(Duration = 3600 * 6, VaryByParam = "none")]
        public ActionResult SiteMapXml()
        {
            Response.ContentType = "text/xml";
            var model = ProfileService.GetAllProfileGuids();
            return View(model);
        }

        //[OutputCache(Duration = 3600 * 6, VaryByParam = "none")]
        public ActionResult RobotsTxt()
        {
            Response.ContentType = "text/plain";
            return View();
        }
    }
}
