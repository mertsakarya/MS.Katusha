using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using PagedList;
using Telerik.Web.Mvc;

namespace MS.Katusha.Web.Controllers
{
    public abstract class GridDetailController<T, TModel> : GridController<T>
        where T : MS.Katusha.Domain.Entities.BaseEntities.BaseModel, new()
    {
        private readonly IDetailGridService<T> _service;

        protected GridDetailController(IResourceService resourceService, IUserService userService, IDetailGridService<T> service, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, service, profileService, stateService, conversationService)
        {
            _service = service;
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult IndexByKeyAjax(long id)
        {
            int gridPage;
            int.TryParse(Request["Page"], out gridPage);
            if (gridPage == 0) {
                int.TryParse(Request["Grid-Page"], out gridPage);
                if (gridPage == 0) {
                    gridPage = 1;
                }
            }

            int size;
            int.TryParse(Request["Size"], out size);
            if (size == 0)
                size = PageSize;

            string orderBy = "Name";
            if (!string.IsNullOrEmpty(Request["orderby"]))
                orderBy = Request["orderby"];
            else if (!string.IsNullOrEmpty(Request["Grid-orderby"]))
                orderBy = Request["Grid-orderby"];

            //ViewData["FilterValue"] = filterValue;

            int total;
            var items = _service.GetAllByKey(id, out total, gridPage, size, p => p.Id, true);
            var itemsModel = Mapper.Map<IList<TModel>>(items);
            return View(new GridModel(itemsModel) { Total = total });
        }
    }   
    
    public abstract class GridController<T> : KatushaController
        where T : MS.Katusha.Domain.Entities.BaseEntities.BaseModel, new()
    {
        private readonly IGridService<T> _service;
        protected const int PageSize = DependencyConfig.GlobalPageSize;

        public GridController(IResourceService resourceService, IUserService userService, IGridService<T> service, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
            _service = service;
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult Index()
        {
            int total;
            var items = _service.GetAll(out total, 1, PageSize);
            var pagedList = new StaticPagedList<T>(items, 1, DependencyConfig.GlobalPageSize, total);
            var itemsIndexModel = new PagedListModel<T> { List = pagedList, Total = total };
            return View(itemsIndexModel);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult IndexAjax()
        {
            int gridPage;
            int.TryParse(Request["Page"], out gridPage);
            if (gridPage == 0) {
                int.TryParse(Request["Grid-Page"], out gridPage);
                if (gridPage == 0) {
                    gridPage = 1;
                }
            }

            int size;
            int.TryParse(Request["Size"], out size);
            if (size == 0)
                size = PageSize;

            string orderBy = "Name";
            if (!string.IsNullOrEmpty(Request["orderby"]))
                orderBy = Request["orderby"];
            else if (!string.IsNullOrEmpty(Request["Grid-orderby"]))
                orderBy = Request["Grid-orderby"];

                //ViewData["FilterValue"] = filterValue;

            int total;
            var items = _service.GetAll(out total, gridPage, size);
            return View(new GridModel(items) {Total = total});
        }

        [HttpPost]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult Insert()
        {
            var model = new T();

            if (TryUpdateModel(model)) {
                var data = Mapper.Map<T>(model);
                _service.Add(data);
            }
            return IndexAjax();
        }

        [HttpPost]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult Update(long id)
        {
            var data = _service.GetById(id);
            if (data != null) {
                if (TryUpdateModel(data)) {
                    _service.Update(data);
                }
            }
            return IndexAjax();
        }

        [HttpPost]
        [GridAction]
        public ActionResult Delete(long id)
        {
            var data = _service.GetById(id);
            if (data != null) {
                _service.Delete(data);
            }
            return IndexAjax();
        }
    }
}
