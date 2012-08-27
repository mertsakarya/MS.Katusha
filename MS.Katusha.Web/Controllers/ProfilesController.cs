using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Infrastructure.Exceptions;
using MS.Katusha.Infrastructure.Exceptions.Services;
using MS.Katusha.Infrastructure.Exceptions.Web;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class ProfilesController : KatushaController
    {
        private readonly IProfileService _profileService;
        private const int PageSize = DependencyConfig.GlobalPageSize;

        public ProfilesController(IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
            _profileService = profileService;
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Ping()
        {
            var pingResult = StateService.Ping(KatushaProfile);

            return (pingResult == null)
                ? Json(new { VisitTime = "", VisitCount = 0, ConversationCount = 0, ConversationUnreadCount = 0 }, JsonRequestBehavior.AllowGet) 
                : Json(new {
                    VisitTime = (pingResult.Visits == null) ? "" : ResourceService.UrlFriendlyDateTime(pingResult.Visits.LastVisitTime), 
                    VisitCount = (pingResult.Visits == null) ? 0 : pingResult.Visits.Visits.Count,
                    ConversationCount = (pingResult.Conversations == null) ? 0 : pingResult.Conversations.Count,
                    ConversationUnreadCount = (pingResult.Conversations == null) ? 0: pingResult.Conversations.UnreadCount
                }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Online(int? key) {
            var pageIndex = (key ?? 1);
            int total;
            var sex = (KatushaProfile.Gender == (byte) Sex.Female) ? (byte) Sex.Male : (byte) Sex.Female;
            IEnumerable<State> onlineStates = StateService.OnlineProfiles(sex, out total, pageIndex, PageSize).ToList();
            var onlineProfiles = new List<Profile>(PageSize);
            onlineProfiles.AddRange(onlineStates.Select(state => ProfileService.GetProfile(state.ProfileId)));

            var profilesModel = Mapper.Map<IEnumerable<ProfileModel>>(onlineProfiles);
            var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, PageSize, total);
            var model = new PagedListModel<ProfileModel> { List = profilesAsIPagedList, Total = total };
            return View(model);
        }
        
        public ActionResult New(int? key)
        {
            var pageIndex = (key ?? 1);
            int total;
            var newProfiles = KatushaProfile.Gender == (byte)Sex.Male
                    ? ProfileService.GetNewProfiles(p => p.Gender == (byte)Sex.Female, out total, pageIndex, PageSize)
                    : ProfileService.GetNewProfiles(p => p.Gender == (byte)Sex.Male, out total, pageIndex, PageSize);

            var profilesModel = Mapper.Map<IEnumerable<ProfileModel>>(newProfiles);
            var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, PageSize, total);
            var model = new PagedListModel<ProfileModel> { List = profilesAsIPagedList, Total = total };
            return View(model);
        }

        public ActionResult Men(int? key) { return Index(p => p.Gender == (byte)Sex.Male, key); }
        public ActionResult Girls(int? key) { return Index(p => p.Gender == (byte)Sex.Female, key); }

        private ActionResult Index(Expression<Func<Profile, bool>> controllerFilter, int? page = 1)
        {
            var pageIndex = (page ?? 1);
            int total;
            var profiles = _profileService.GetNewProfiles(controllerFilter, out total, pageIndex, PageSize);
            var profilesModel = Mapper.Map<IList<ProfileModel>>(profiles);
            var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, PageSize, total);
            var model = new PagedListModel<ProfileModel> { List = profilesAsIPagedList, Total = total };
            return View("Index", model);
        }

        [KatushaFilter(IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
        [KatushaNeedsPayment(Product = ProductNames.MonthlyKatusha, HasLayout = true)]
        public ActionResult Show(string key)
        {
            var profile = _profileService.GetProfile(key, KatushaProfile);
            if(profile == null) throw new KatushaProfileNotFoundException(key);
            ViewBag.SameProfile = IsKeyForProfile(key);
            ViewBag.GoogleAnalytics.AddPageLevelVariable(GoogleAnalyticsPageLevelVariableType.Product, key);
            var model = Mapper.Map<ProfileModel>(profile);
            return View(model);
        }

        [KatushaFilter(IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
        public ActionResult Me()
        {
            var profile = _profileService.GetProfile(KatushaProfile.Id, KatushaProfile);
            ViewBag.SameProfile = true;
            var model = Mapper.Map<ProfileModel>(profile);
            return View("Show", model);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = false)]
        public ActionResult Create(string key)
        {
            if (KatushaUser.Gender > 0) {
                if(KatushaUser.Guid != Guid.Empty) {
                    var id = ProfileService.GetProfileId(KatushaUser.Guid);
                    if(id > 0) { // profile exists
                        return RedirectToAction("Show", new {key = KatushaUser.Guid});
                    }
                } else {
                    throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, "CREATE GET");
                }
            }
            var model = new ProfileModel();
            if(key == null)
                throw new KatushaGenderNotExistsException(null);
            var genderValue = key.ToLowerInvariant();
            switch (genderValue) {
                case "girl":
                    model.Gender = Sex.Female;
                    break;
                case "man":
                    model.Gender =  Sex.Male;
                    break;
                default:
                    throw new KatushaGenderNotExistsException(null);
            }
            return View(model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = false)]
        public ActionResult Create(string key, ProfileModel model)
        {
            if (KatushaUser.Gender > 0) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, "CREATE GET");
            try {
                if (!ModelState.IsValid) return View(model);
                var genderValue = key.ToLowerInvariant();
                switch (genderValue) {
                    case "girl":
                        model.Gender = Sex.Female;
                        break;
                    case "man":
                        model.Gender = Sex.Male;
                        break;
                    default:
                        throw new KatushaGenderNotExistsException(null);
                }
                var profile = Mapper.Map<Profile>(model);
                profile.UserId = KatushaUser.Id;
                profile.Guid = KatushaUser.Guid;
                if (!ModelState.IsValid) return View(model);

                _profileService.CreateProfile(profile);
                //ValidateProfileCollections(model, profile);                
                KatushaProfile = profile; // _profileService.GetProfileDB(profile.Id, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches);
                //ValidateProfileCollections(model, KatushaProfile);
                if (!ModelState.IsValid) return View(key, model);
                return RedirectToAction("Show", new { key = (String.IsNullOrWhiteSpace(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName });
            } catch (KatushaFriendlyNameExistsException) {
                ModelState.AddModelError("FriendlyName", ResourceService.ResourceValue("FriendlyNameExists"));
                return View(model);
            } catch (KatushaException) {
                throw;
            } catch (Exception ex) {
                ModelState.AddModelError("Model", ex);
                return View(model);
            }
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Edit()
        {
            ViewBag.SameProfile = true;
            var model = Mapper.Map<ProfileModel>(KatushaProfile);
            return View(model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Edit(ProfileModel model)
        {
            try {
                ViewBag.SameProfile = true;
                var profileModel = KatushaProfile;
                if (!ModelState.IsValid) return View(model);
                var profile = Mapper.Map<Profile>(model);
                profile.Id = profileModel.Id;
                profile.Guid = profileModel.Guid;
                profile.Gender = profileModel.Gender;
                profile.ProfilePhotoGuid = profileModel.ProfilePhotoGuid;
                ValidateProfileCollections(model, profileModel);
                if (!ModelState.IsValid) return View(model);
                _profileService.UpdateProfile(profile);
                return RedirectToAction("Show", new { key = (String.IsNullOrWhiteSpace(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName });
            } catch (KatushaFriendlyNameExistsException) {
                ModelState.AddModelError("FriendlyName", ResourceService.ResourceValue("FriendlyNameExists"));
                return View(model);
            } catch (KatushaException) {
                throw;
            } catch {
                return View();
            }
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Delete(string key)
        {
            if (!IsKeyForProfile(key)) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, key);
            var profile = KatushaProfile;
            var model = Mapper.Map<ProfileModel>(profile);
            return View("Delete", model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Delete(string key, FormCollection collection)
        {
            if (!IsKeyForProfile(key)) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, key);
            _profileService.DeleteProfile(KatushaProfile.Id);
            return RedirectToAction("Girls");
        }

        private void ValidateProfileCollections(ProfileModel profileModel, Profile model)
        {

            var llp = new LookupListProcessor<ProfileModel, Profile, CountriesToVisitModel, CountriesToVisit, string>(
                p => p.CountriesToVisit,
                p => p.CountriesToVisit,
                p => (string)p.Country,
                p => p.Country,
                (modelData, country) => _profileService.DeleteCountriesToVisit(modelData.Id, country),
                (modelData, country) => _profileService.AddCountriesToVisit(modelData.Id, country)
                );

            var llp2 = new LookupListProcessor<ProfileModel, Profile, LanguagesSpokenModel, LanguagesSpoken, string>(
                p => p.LanguagesSpoken,
                p => p.LanguagesSpoken,
                p => (string)p.Language,
                p => p.Language,
                (modelData, language) => _profileService.DeleteLanguagesSpoken(modelData.Id, language),
                (modelData, language) => _profileService.AddLanguagesSpoken(modelData.Id, language)
                );

            var llp3 = new LookupListProcessor<ProfileModel, Profile, SearchingForModel, SearchingFor, LookingFor>(
                p => p.Searches,
                p => p.Searches,
                p => (LookingFor)p.Search,
                p => p.Search,
                (modelData, search) => _profileService.DeleteSearches(modelData.Id, search),
                (modelData, search) => _profileService.AddSearches(modelData.Id, search)
                );

            llp.Process(Request, ModelState, profileModel, model);
            llp2.Process(Request, ModelState, profileModel, model);
            llp3.Process(Request, ModelState, profileModel, model);
        }
    }
}
