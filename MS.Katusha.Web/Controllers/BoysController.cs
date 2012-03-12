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
        private IBoyService _boyService;

        public BoysController(IBoyService boyService)
        {
            _boyService = boyService;       
        }


        // Ajax Paging (cont'd)
        public ActionResult AjaxPage(int? page)
        {
            var listPaged = GetPagedNames(page);
            if (listPaged == null)
                return HttpNotFound();

            return Json(new
            {
                names = listPaged,
                pager = listPaged.GetMetaData()
            }, JsonRequestBehavior.AllowGet);
        }
        private IPagedList<string> GetPagedNames(int? page)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = GetStuffFromDatabase();

            // page the list
            const int pageSize = 20;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }

        private IEnumerable<string> GetStuffFromDatabase()
        {
            var sampleData = "1,2,3,4,5,6";
            return sampleData.Split(',');
        }

        public ActionResult Index(int? page)
        {

            var pageIndex = (page ?? 1); 
            var pageSize = 2;
            int totalUserCount = 1000; // will be set by call to GetAllUsers due to _out_ paramter :-|
            var boys = _boyService.GetNewProfiles<Boy>(0, 0); //pageIndex, pageSize);
            var boysModel = Mapper.Map<IList<BoyModel>>(boys);
            
            var boysAsIPagedList = new StaticPagedList<BoyModel>(boysModel, pageIndex, pageSize, totalUserCount);
            var model = boysAsIPagedList; // new BoysIndexModel { List = boysAsIPagedList };
            
            return View(model);

        }

        //
        // GET: /Boys/Details/5

        public ActionResult Details(string guid)
        {
            return View();
        }

        //
        // GET: /Boys/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Boy/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Boy/Edit/5

        public ActionResult Edit(string guid)
        {
            return View();
        }

        //
        // POST: /Boys/Edit/5

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

        //
        // GET: /Boys/Delete/5

        public ActionResult Delete(string guid)
        {
           
            var boy = _boyService.GetProfile<Boy>(Guid.Parse(guid));
            if(boy != null)
                _boyService.DeleteProfile(boy);
            return View();
        }

        //
        // POST: /Boys/Delete/5

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
