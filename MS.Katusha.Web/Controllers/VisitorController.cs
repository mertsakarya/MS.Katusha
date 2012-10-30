using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class VisitorController : KatushaController
    {
        private readonly IVisitService _visitService;
        private const int PageSize = DependencyConfig.GlobalPageSize;

        public VisitorController(IResourceService resourceService, IUserService userService, IProfileService profileService, IVisitService visitService, IStateService stateService, IConversationService conversationService, ITokBoxService tokBoxService)
            : base(resourceService, userService, profileService, stateService, conversationService, tokBoxService)
        {
            _visitService = visitService;
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Index(int? key)
        {
            var pageIndex = (key ?? 1);
            int total;

            var visitors = _visitService.GetVisitors(KatushaProfile.Id, out total, pageIndex, PageSize);

            var visitorsModel = Mapper.Map<IList<NewVisitModel>>(visitors);

            var visitorAsIPagedList = new StaticPagedList<NewVisitModel>(visitorsModel, pageIndex, PageSize, total);
            var model = new PagedListModel<NewVisitModel> { List = visitorAsIPagedList, Total = total };

            return View(model);
        }


        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult MyVisits(int? key)
        {
            var pageIndex = (key ?? 1);
            int total;

            var visitors = _visitService.GetMyVisits(KatushaProfile.Id, out total, pageIndex, PageSize);

            var visitorsModel = Mapper.Map<IList<NewVisitModel>>(visitors);

            var visitorAsIPagedList = new StaticPagedList<NewVisitModel>(visitorsModel, pageIndex, PageSize, total);
            var model = new PagedListModel<NewVisitModel> { List = visitorAsIPagedList, Total = total };

            return View(model);
        }
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult NewVisits(string key)
        {
            NewVisits visits;
            try {
                var dateTime = new DateTime(
                    int.Parse(key.Substring(0,4)),
                    int.Parse(key.Substring(4,2)),
                    int.Parse(key.Substring(6,2)),
                    int.Parse(key.Substring(8,2)),
                    int.Parse(key.Substring(10,2)),
                    int.Parse(key.Substring(12,2))
                    ); //.ToUniversalTime();
                //var dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.Zero);
                visits = _visitService.GetVisitorsSinceLastVisit(KatushaProfile.Id, dateTime);
            } catch {
                visits = new NewVisits {LastVisitTime = DateTime.Now, Visits = new List<UniqueVisitorsResult>()};
            }
            var model = new NewVisitsModel {
                LastVisitTime = visits.LastVisitTime,
                Visits = Mapper.Map<IList<NewVisitModel>>(visits.Visits)
            };
            return View(model);
        }
    }
}
