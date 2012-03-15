using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Exceptions;
using MS.Katusha.Exceptions.Services;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers.BaseControllers
{
    public class ProfileController<T, TModel> : KatushaController  where T : Profile where TModel : ProfileModel
    {
        private readonly IProfileService<T> _profileService;

        public ProfileController(IProfileService<T> profileService, IUserService userService)
            : base(userService)
        {
            _profileService = profileService;
        }

        public ActionResult Index(int? page)
        {

            var pageIndex = (page ?? 1);
            const int pageSize = 2;
            const int totalUserCount = 1000; // will be set by call to GetAllUsers due to _out_ paramter :-|
            var profiles = _profileService.GetNewProfiles(pageIndex, pageSize);
            var profilesModel = Mapper.Map<IList<TModel>>(profiles);

            var profilesAsIPagedList = new StaticPagedList<TModel>(profilesModel, pageIndex, pageSize, totalUserCount);
            var model = new PagedListModel<ProfileModel> { List = profilesAsIPagedList };
            return View(model);

        }

        public ActionResult Details(string key)
        {
            try
            {
                var profile = _profileService.GetProfile(key);
                var model = MapToModel(profile);
                return View(model);
            }

            catch (KatushaFriendlyNameNotFoundException ex)
            {
                return View("KatushaError", ex);
            }
        }

        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender > 0) return RedirectToAction("Index");
            ProfileModel model = null;
            var controllerName = RouteData.Values["Controller"].ToString().ToLower();
            if (controllerName == "boys")
                model = new BoyModel();
            else if (controllerName == "girls")
                model = new GirlModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(TModel model)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender > 0) return RedirectToAction("Index");
            try
            {
                if (!ModelState.IsValid) return View(model);
                
                var profile = MapToEntity(model);
                profile.UserId = KatushaUser.Id;
                profile.Guid = KatushaUser.Guid;
                _profileService.CreateProfile(profile);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(string key)
        {
            try
            {
                var profile = _profileService.GetProfile(key);
                var model = MapToModel(profile);
                return View(model);
            }
            catch (KatushaFriendlyNameNotFoundException ex)
            {
                return View("KatushaError", ex);
            }
        }

        [HttpPost]
        public ActionResult Edit(string key, TModel model)
        {
            try
            {
                var tmp = _profileService.GetProfile(key);
                if (!ModelState.IsValid) return View(model);
                T profile = MapToEntity(model);
                profile.Id = tmp.Id;
                profile.Guid = tmp.Guid;
                _profileService.UpdateProfile(profile);
                return RedirectToAction("Index");
            }
            catch (KatushaException ex)
            {
                return View("KatushaError", ex);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(string key)
        {
            try
            {
                var profile = _profileService.GetProfile(key);
                var model = MapToModel(profile);
                return View("DeleteProfile", model);
            }
            catch (KatushaFriendlyNameNotFoundException ex)
            {
                return View("KatushaError", ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(string key, FormCollection collection)
        {
            try
            {
                var profile = _profileService.GetProfile(key);
                _profileService.DeleteProfile(profile.Guid);
                return RedirectToAction("Index");
            }
            catch (KatushaFriendlyNameNotFoundException ex)
            {
                return View("KatushaError", ex);
            }
            catch
            {
                return View("DeleteProfile");
            }
        }

        private T MapToEntity(TModel model)
        {
            if (model is BoyModel)
                return Mapper.Map<Boy>(model) as T;
            if (model is GirlModel)
                return Mapper.Map<Girl>(model) as T;
            return null;
        }

        private TModel MapToModel(T model)
        {
            if (model is Boy)
                return Mapper.Map<BoyModel>(model) as TModel;
            if (model is Girl)
                return Mapper.Map<GirlModel>(model) as TModel;
            return null;
        }
    }
}
