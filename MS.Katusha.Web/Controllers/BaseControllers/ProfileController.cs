using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Exceptions;
using MS.Katusha.Exceptions.Services;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers.BaseControllers
{
    public class ProfileController<T, TModel> : KatushaController where T : Profile where TModel : ProfileModel
    {
        private readonly IProfileService<T> _profileService;
        private readonly IResourceManager _resourceManager;

        public ProfileController(IProfileService<T> profileService, IUserService userService, IResourceManager resourceManager)
            : base(userService)
        {
            _profileService = profileService;
            _resourceManager = resourceManager;
        }

        public ActionResult Index(int? page)
        {

            var pageIndex = (page ?? 1);
            const int pageSize = 2;
            const int totalUserCount = 1000; // will be set by call to GetAllUsers due to _out_ paramter :-|
            var profiles = _profileService.GetNewProfiles(pageIndex, pageSize);
            var profilesModel = Mapper.Map<IList<TModel>>(profiles);

            var profilesAsIPagedList = new StaticPagedList<TModel>(profilesModel, pageIndex, pageSize, totalUserCount);
            var model = new PagedListModel<ProfileModel> {List = profilesAsIPagedList};
            return View(model);

        }

        public ActionResult Details(string key)
        {
            try {
                var profile = _profileService.GetProfile(key);
                var model = MapToModel(profile);
                return View(model);
            } catch (KatushaFriendlyNameNotFoundException ex) {
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
            try {
                if (!ModelState.IsValid) return View(model);

                var profile = MapToEntity(model);
                profile.UserId = KatushaUser.Id;
                profile.Guid = KatushaUser.Guid;
                _profileService.CreateProfile(profile);

                return RedirectToAction("Index");
            } catch {
                return View();
            }
        }

        public ActionResult Edit(string key)
        {
            try {
                var profile = _profileService.GetProfile(key, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches);
                var model = MapToModel(profile);
                return View(model);
            } catch (KatushaFriendlyNameNotFoundException ex) {
                return View("KatushaError", ex);
            }
        }

        [HttpPost]
        public ActionResult Edit(string key, TModel model)
        {

            try {
                if (!ModelState.IsValid) return View(model);
                var profileModel = _profileService.GetProfile(key, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches);
                var llp = new LookupListProcessor<TModel, T, CountriesToVisitModel, CountriesToVisit, Country>(
                    p => p.CountriesToVisit,
                    p => (Country)p.Country,
                    (modelData, country) => _profileService.DeleteCountriesToVisit(modelData.Id, country),
                    (modelData, country) => _profileService.AddCountriesToVisit(modelData.Id, country),
                    modelData => model.CountriesToVisit.Add(modelData),
                    () => model.CountriesToVisit.Clear()
                    );
                var llp2 = new LookupListProcessor<TModel, T, LanguagesSpokenModel, LanguagesSpoken, Language>(
                    p => p.LanguagesSpoken,
                    p => (Language)p.Language,
                    (modelData, language) => _profileService.DeleteLanguagesSpoken(modelData.Id, language),
                    (modelData, language) => _profileService.AddLanguagesSpoken(modelData.Id, language),
                    modelData => model.LanguagesSpoken.Add(modelData),
                    () => model.LanguagesSpoken.Clear()
                );
                var llp3 = new LookupListProcessor<TModel, T, SearchingForModel, SearchingFor, LookingFor>(
                    p => p.Searches,
                    p => (LookingFor)p.Search,
                    (modelData, search) => _profileService.DeleteSearches(modelData.Id, search),
                    (modelData, search) => _profileService.AddSearches(modelData.Id, search),
                    modelData => model.Searches.Add(modelData),
                    () => model.Searches.Clear()
                );
                if (!llp.Process(Request, ModelState, model, profileModel)) {
                    return View(model);
                }

                if (!llp2.Process(Request, ModelState, model, profileModel)) {
                    return View(model);
                }

                if (!llp3.Process(Request, ModelState, model, profileModel)) {
                    return View(model);
                }

                T profile = MapToEntity(model);
                profile.Id = profileModel.Id;
                profile.Guid = profileModel.Guid;
                _profileService.UpdateProfile(profile);
                return RedirectToAction("Index");
            } catch (KatushaException ex) {
                return View("KatushaError", ex);
            } catch {
                return View();
            }
        }

        public ActionResult Delete(string key)
        {
            try {
                var profile = _profileService.GetProfile(key);
                var model = MapToModel(profile);
                return View("DeleteProfile", model);
            } catch (KatushaFriendlyNameNotFoundException ex) {
                return View("KatushaError", ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(string key, FormCollection collection)
        {
            try {
                var profile = _profileService.GetProfile(key);
                _profileService.DeleteProfile(profile.Guid);
                return RedirectToAction("Index");
            } catch (KatushaFriendlyNameNotFoundException ex) {
                return View("KatushaError", ex);
            } catch {
                return View("DeleteProfile");
            }
        }

        public JsonResult GetCountriesToVisit()
        {
            var result = new JsonResult();

            var tags = _profileService.GetCountriesToVisit();
            if (tags != null) {
                var data = from r in tags
                           select new {
                                          key = r.Value,
                                          value = r.Key
                                      };
                result = Json(data);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }

            return result;
        }

        public JsonResult GetSelectedCountriesToVisit(string key)
        {

            var list = new List<dynamic>();
            foreach (var item in _profileService.GetSelectedCountriesToVisit(key)) {
                list.Add(new {key = _resourceManager._LText("Country", item), value = item});
            }
            var result = Json(list);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
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

/*
                try {
                    if (Request.Form["CountriesToVisitSelection[]"] != null) {
                        var list = Request.Form["CountriesToVisitSelection[]"].Split(',');
                        HashSet<Country> setForm = new HashSet<Country>();
                        foreach (var line in list) {
                            Country c;
                            if (Enum.TryParse(line, out c))
                                setForm.Add(c);
                            else
                                validationResults.Add(line + " Can't Parse");
                        }

                        var setData = new HashSet<Country>();
                        foreach (var line in countriesToVisit) {
                            var country = (Country) line.Country;
                            setData.Add(country);
                            if (!setForm.Contains(country)) {
                                try {
                                    _profileService.DeleteCountriesToVisit(profileModel.Id, country);
                                } catch(Exception ex) {
                                    validationResults.Add(country.ToString() + " Can't Delete" );
                                }
                            }
                        }
                        foreach (var country in setForm) {
                            if (!setData.Contains(country)) {
                                try {
                                    _profileService.AddCountriesToVisit(profileModel.Id, country);
                                } catch (Exception ex) {
                                    validationResults.Add(country.ToString() + " Can't Add" );
                                }
                            }
                        }
                    }

                } catch(Exception ex) {
                    validationResults.Add(ex.Message);
                }
                if(validationResults.Count > 0) {
                    foreach(var item in validationResults)
                        ModelState.AddModelError("CountriesToVisit", item);
                    model.CountriesToVisit.Clear();
                    foreach (var ctv in countriesToVisit) {
                        var ctvModel = Mapper.Map<CountriesToVisitModel>(ctv);
                        model.CountriesToVisit.Add(ctvModel);
                    }
                    return View(model);
                }
 * 
 * * * */
