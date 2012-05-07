using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
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
            return ContextDependentView(null, "Register");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult JsonRegister(RegisterModel model)
        {
            if (ModelState.IsValid) {
                KatushaMembershipCreateStatus createStatus;
                UserService.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);
                if (createStatus == KatushaMembershipCreateStatus.Success) {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    return Json(new {success = true});
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }
            return Json(new {errors = GetErrorsFromModelState()});
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid) {
                KatushaMembershipCreateStatus createStatus;
                UserService.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);
                if (createStatus == KatushaMembershipCreateStatus.Success) {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }
            return View(model);
        }

        public ActionResult ChangePassword() { return View(); }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid) {
                bool changePasswordSucceeded;
                try {
                    changePasswordSucceeded = UserService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                } catch (Exception) {
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded) {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }
            return View(model);
        }

        public ActionResult ChangePasswordSuccess() { return View(); }

        private ActionResult ContextDependentView(object model = null, string viewName = "")
        {
            var actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null) {
                ViewBag.FormAction = "Json" + actionName;
                return PartialView(viewName, model);
            }
            ViewBag.FormAction = actionName;
            return View(viewName, model);
        }

        private IEnumerable<string> GetErrorsFromModelState() { return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage)); }

        #region Status Codes
        private static string ErrorCodeToString(KatushaMembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            var resourceManager = ResourceManager.GetInstance();
            switch (createStatus) {
                case KatushaMembershipCreateStatus.DuplicateUserName:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.DuplicateEmail:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidPassword:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidEmail:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidAnswer:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidQuestion:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.InvalidUserName:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.ProviderError:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                case KatushaMembershipCreateStatus.UserRejected:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus." + createStatus);

                default:
                    return resourceManager.ResourceValue("KatushaMembershipCreateStatus.Default");
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

            var location = "";
            if (me.location != null && !String.IsNullOrWhiteSpace(me.location.name)){
                location = me.location.name;
            } else if (me.hometown != null && !String.IsNullOrWhiteSpace(me.hometown.name)) {
                location = me.hometown.name;
            }
            string country = "";
            string city = "";
            if (!String.IsNullOrWhiteSpace(location)) {
                var arr = location.Split(',');
                if (arr.Length >= 2) {
                    country = arr[arr.Length - 1].Trim();
                    for (int i = 0; i < arr.Length - 1; i++)
                        city += arr[i] + ", ";
                    city = city.Substring(0, city.Length - 2);
                } else if (arr.Length == 1) {
                    country = arr[0].Trim();
                }
            }
            var model = new FacebookProfileModel() { Name = me.name, Description = me.quotes, Gender = (me.gender == "male") ? Sex.Male : Sex.Female, City = city, FacebookId = me.id };
            var rm = ResourceManager.GetInstance();
            if(rm.ContainsKey("Country", country)) {
                model.From = country;
            }
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
                    UserService.UpdateUser(user);
                    FormsAuthentication.SetAuthCookie(tmpGuid, createPersistentCookie: false);
                    return Json(new { success = true });
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }
            var errors = GetErrorsFromModelState();
            return Json(new { errors = errors });
        }

    }
}
