using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Web.Controllers
{
    public class BoysController : Controller
    {
        private readonly IBoyService _boyService;
        private readonly IUserService _userService;

        public BoysController(IBoyService boyService, IUserService userService)
        {
            _boyService = boyService;
            _userService = userService;
        }

        #region AjaxPaging
        //// Ajax Paging (cont'd)
        //public ActionResult AjaxPage(int? page)
        //{
        //    var listPaged = GetPagedNames(page);
        //    if (listPaged == null)
        //        return HttpNotFound();

        //    return Json(new
        //    {
        //        names = listPaged,
        //        pager = listPaged.GetMetaData()
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //private IPagedList<string> GetPagedNames(int? page)
        //{
        //    // return a 404 if user browses to before the first page
        //    if (page.HasValue && page < 1)
        //        return null;

        //    // retrieve list from database/whereverand
        //    var listUnpaged = GetStuffFromDatabase();

        //    // page the list
        //    const int pageSize = 20;
        //    var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

        //    // return a 404 if user browses to pages beyond last page. special case first page if no items exist
        //    if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
        //        return null;

        //    return listPaged;
        //}

        //private IEnumerable<string> GetStuffFromDatabase()
        //{
        //    var sampleData = "1,2,3,4,5,6";
        //    return sampleData.Split(',');
        //}
        #endregion

        public ActionResult Index(int? page)
        {

            var pageIndex = (page ?? 1); 
            var pageSize = 2;
            int totalUserCount = 1000; // will be set by call to GetAllUsers due to _out_ paramter :-|
            var boys = _boyService.GetNewProfiles(pageIndex, pageSize) as IEnumerable<Boy>;
            var boysModel = Mapper.Map<IList<BoyModel>>(boys);
            
            var boysAsIPagedList = new StaticPagedList<BoyModel>(boysModel, pageIndex, pageSize, totalUserCount);
            var model = new BoysIndexModel { List = boysAsIPagedList };
            
            return View(model);

        }

        public ActionResult Details(string id)
        {
            var boy = _boyService.GetProfile(Guid.Parse(id), p => p.CountriesToVisit, p => User, p => p.State,
                                  p => p.LanguagesSpoken, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos);
            var model = Mapper.Map<BoyModel>(boy);
            return View(model);
        }

        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public ActionResult Create(BoyModel model)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Index");
            try
            {
                if (!ModelState.IsValid) return View(model);
                var boy = Mapper.Map<Boy>(model);

                var user = _userService.GetUser(User.Identity.Name);
                _boyService.CreateProfile(boy);
                user.Profile = boy;
                //...
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(string guid, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(string guid)
        {           
            var boy = _boyService.GetProfile(Guid.Parse(guid));
            if(boy != null)
                _boyService.DeleteProfile(boy);
            return View();
        }

        [HttpPost]
        public ActionResult Delete(string guid, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
