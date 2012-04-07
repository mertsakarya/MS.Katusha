using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Attributes;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class VisitorController : KatushaController
    {
        private readonly IVisitService _visitService;
        private const int PageSize = DependencyHelper.GlobalPageSize;

        public VisitorController(IUserService userService, IVisitService visitService)
            : base(userService)
        {
            _visitService = visitService;
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Index(int? page)
        {
            var pageIndex = (page ?? 1);
            int total;

            var visitors = _visitService.GetVisitors(KatushaProfile.Id, out total, pageIndex, PageSize);
            var visitorsModel = Mapper.Map<IList<VisitModel>>(visitors);

            var visitorAsIPagedList = new StaticPagedList<VisitModel>(visitorsModel, pageIndex, PageSize, total);
            var model = new PagedListModel<VisitModel> { List = visitorAsIPagedList };

            return View(model);
        }
    }
}
