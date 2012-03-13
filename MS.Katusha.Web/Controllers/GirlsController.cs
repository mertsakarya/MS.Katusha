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
    public class GirlsController : Controller
    {
        private readonly IGirlService _girlService;

        public GirlsController(IGirlService girlService)
        {
            _girlService = girlService;       
        }

        public ActionResult Index(int? page)
        {

            var pageIndex = (page ?? 1); 
            var pageSize = 2;
            int totalUserCount = 1000; // will be set by call to GetAllUsers due to _out_ paramter :-|
            var girls = _girlService.GetNewProfiles(pageIndex, pageSize) as IEnumerable<Girl>;
            var girlsModel = Mapper.Map<IList<GirlModel>>(girls);
            
            var girlsAsIPagedList = new StaticPagedList<GirlModel>(girlsModel, pageIndex, pageSize, totalUserCount);
            var model = new GirlsIndexModel {List = girlsAsIPagedList};
            
            return View(model);

        }

        //
        // GET: /Girls/Details/5

        public ActionResult Details(string guid)
        {
            return View();
        }

        //
        // GET: /Girls/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Girl/Create

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
        // GET: /Girl/Edit/5

        public ActionResult Edit(string guid)
        {
            return View();
        }

        //
        // POST: /Girls/Edit/5

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
        // GET: /Girls/Delete/5

        public ActionResult Delete(string guid)
        {
           
            Girl girl = _girlService.GetProfile(Guid.Parse(guid)) as Girl;
            if(girl != null)
                _girlService.DeleteProfile(girl);
            return View();
        }

        //
        // POST: /Girls/Delete/5

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
