using System;
using System.Web.Mvc;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Web.Controllers
{
    public class GirlsController : Controller
    {
        private IGirlService _girlService;

        public GirlsController(IGirlService girlService)
        {
            _girlService = girlService;       
        }
        //
        // GET: /Girls/

        public ActionResult Index()
        {

            return View();
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
           
            var girl = _girlService.GetProfile<Girl>(Guid.Parse(guid));
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
