using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Exceptions;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers
{

    [Authorize]
    public class AccountController : KatushaController
    {
        public AccountController(IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, profileService, stateService, conversationService) {}

        [AllowAnonymous]
        public ActionResult Login() { return ContextDependentView(null, "Login"); }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult JsonLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid) {
                if (UserService.ValidateUser(model.UserName, model.Password)) {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return Json(new {success = true, redirect = returnUrl});
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }
            return Json(new {errors = GetErrorsFromModelState()});
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid) {
                if (UserService.ValidateUser(model.UserName, model.Password)) {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl)) {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LoginWithId(string key)
        {
            long uid;
            Guid guid;
            User model;
            if (long.TryParse(key, out uid)) {
                model = UserService.GetUser(uid);
            } else if (Guid.TryParse(key, out guid)) {
                model = UserService.GetUser(guid);
            } else {
                model = UserService.GetUser(key);
            }
            if (model != null) {
                if (UserService.ValidateUser(model.UserName, model.Password)) {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View("Login", new LoginModel());
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session["AccessToken"] = null;
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return ContextDependentView(new RegisterModel() {Location = new LocationModel()}, "Register");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult JsonRegister(RegisterModel model) {
            return _Register(model, Json(new { success = true }), Json(new { errors = GetErrorsFromModelState() }));
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            return _Register(model, RedirectToAction("Index", "Home"), View(model));
        }

        private ActionResult _Register(RegisterModel model, ActionResult successResult, ActionResult failResult)
        {
            if (ModelState.IsValid) {
                KatushaMembershipCreateStatus createStatus;
                KatushaUser = UserService.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);
                if (createStatus == KatushaMembershipCreateStatus.Success) {
                    var profile = Mapper.Map<Profile>(model);
                    profile.UserId = KatushaUser.Id;
                    profile.Guid = KatushaUser.Guid;
                    ProfileService.CreateProfile(profile);
                    KatushaProfile = profile;
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    return successResult;
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }
            return failResult;
        }

        public ActionResult ChangePassword() { return ContextDependentView(null, "ChangePassword"); }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            return _ChangePassword(model, RedirectToAction("ChangePasswordSuccess"), View(model));
        }

        [HttpPost]
        public JsonResult JsonChangePassword(ChangePasswordModel model, string returnUrl)
        {
            return (JsonResult) _ChangePassword(model, Json(new { success = true, redirect = Url.Action("ChangePasswordSuccess", "Account") }), Json(new { errors = GetErrorsFromModelState() }));
        }

        private ActionResult _ChangePassword(ChangePasswordModel model, ActionResult successResult, ActionResult failResult)
        {
            if (ModelState.IsValid) {
                bool changePasswordSucceeded;
                try {
                    changePasswordSucceeded = UserService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                } catch (Exception) {
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded) return successResult;
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }
            return failResult;
        }

        public ActionResult ChangePasswordSuccess() { return View(); }

        #region Status Codes
        private string ErrorCodeToString(KatushaMembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus) {
                case KatushaMembershipCreateStatus.DuplicateUserName:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.DuplicateEmail:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidPassword:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidEmail:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidAnswer:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidQuestion:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidUserName:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.ProviderError:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.UserRejected:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                default:
                    return ResourceService.ResourceValue("KatushaMembershipCreateStatus.Default");
            }
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        public JsonResult FacebookLogin(string accessToken, string uid)
        {
            Session["AccessToken"] = accessToken;
            var user = UserService.GetUserByFacebookUId(uid);
            if (user == null)
                return Json(new {status = "new", url = Url.Action("Facebook", "Account")});
            FormsAuthentication.SetAuthCookie(user.UserName, false);
            return Json(new {status = "ok", url = Url.Action("Index", "Home")});
        
        }
        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var accessToken = (String) Session["AccessToken"];
            if (String.IsNullOrWhiteSpace(accessToken))
                throw new KatushaException("FacebookAccessToken", "No Facebook Access Token");
            var client = new Facebook.FacebookClient(accessToken);
            dynamic me = client.Get("/me?access_token=" + accessToken); //, new {fields = "name,id,email,birthday,email,gender,hometown,location,quotes,username,id"});

            var facebookLocation = "";
            if (me.location != null && !String.IsNullOrWhiteSpace(me.location.name)){
                facebookLocation = me.location.name;
            } else if (me.hometown != null && !String.IsNullOrWhiteSpace(me.hometown.name)) {
                facebookLocation = me.hometown.name;
            }
            var location = new LocationModel() {CityCode = 0, CountryCode = "", CityName = ""};
            if (!String.IsNullOrWhiteSpace(facebookLocation)) {
                var arr = facebookLocation.Split(',');
                if (arr.Length >= 2) {
                    location.CountryName = arr[arr.Length - 1].Trim();
                    for (int i = 0; i < arr.Length - 1; i++)
                        location.CityName += arr[i] + ", ";
                    location.CityName = location.CityName.Substring(0, location.CityName.Length - 2);
                } else if (arr.Length == 1) {
                    location.CountryName = arr[0].Trim();
                }
            }
            var model = new FacebookProfileModel { Name = me.name, Description = me.quotes, Gender = (me.gender == "male") ? Sex.Male : Sex.Female, FacebookId = me.id };
            var countries = ResourceService.GetCountries();
            location.CountryName = location.CountryName.ToLowerInvariant();
            foreach (var item in countries.Where(item => item.Value.ToLowerInvariant() == location.CountryName.ToLowerInvariant())) {
                location.CountryCode = item.Key;
                location.CountryName = item.Value;
                break;
            }
            if(location.CountryCode == "") location.CountryName = "";
            if(location.CountryCode != "" && location.CityName != "") {
                location.CityName = location.CityName.ToLowerInvariant();
                var cities = ResourceService.GetCities(location.CountryCode);
                var found = false;
                foreach (var item in cities.Where(item => item.Value.ToLowerInvariant() == location.CityName)) {
                    found = true;
                    location.CityName = item.Value;
                    location.CityCode = int.Parse(item.Key);
                    break;
                }
                if(!found) location.CityName = "";
            }
            model.Location = location;
            return ContextDependentView(model, "Facebook");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Facebook(FacebookProfileModel model)
        {
            var accessToken = (String)Session["AccessToken"];
            if (String.IsNullOrWhiteSpace(accessToken))
                throw new KatushaException("FacebookAccessToken", "No Facebook Access Token");
            var client = new Facebook.FacebookClient(accessToken);
            dynamic me = client.Get("/me?access_token=" + accessToken); //new { fields = "name,id,email,birthday" }
            //if (createStatus == KatushaMembershipCreateStatus.Success) {
            //    FormsAuthentication.SetAuthCookie(user.UserName, createPersistentCookie: false);
            //    return Json(new { status = "new", redir = Url.Action("Index", "Home"), tmp = me.id });
            //}
            return View(me);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult JsonFacebook(FacebookProfileModel model)
        {
            if (ModelState.IsValid) {
                KatushaMembershipCreateStatus createStatus;
                var tmpGuid = Guid.NewGuid().ToString("N");
                var user = UserService.CreateUser(tmpGuid, "tEmpPassword", model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);
                if (createStatus == KatushaMembershipCreateStatus.Success) {
                    var profile = Mapper.Map<Profile>(model);
                    profile.UserId = user.Id;
                    profile.Guid = user.Guid;

                    ProfileService.CreateProfile(profile);
                    user.FacebookUid = model.FacebookId;
                    user.UserRole = (long)UserRole.Normal;
                    UserService.UpdateUser(user);
                    FormsAuthentication.SetAuthCookie(tmpGuid, createPersistentCookie: false);
                    return Json(new { success = true });
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }
            var errors = GetErrorsFromModelState();
            return Json(new { errors });
        }

    }
}
