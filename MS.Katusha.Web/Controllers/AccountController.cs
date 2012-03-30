using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Facebook;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Models;

namespace MS.Katusha.Web.Controllers
{

    [Authorize]
    public class AccountController : KatushaController
    {
        private readonly IUserService _service;

        public AccountController(IUserService service, ISearchService searchService) : base(service, searchService)
        {
            _service = service;
        }
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            return ContextDependentView();
        }

        //
        // POST: /Account/JsonLogin

        [AllowAnonymous]
        [HttpPost]
        public JsonResult JsonLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_service.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return Json(new { success = true, redirect = returnUrl });
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_service.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session["AccessToken"] = null;
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return ContextDependentView();
        }

        //
        // POST: /Account/JsonRegister

        [AllowAnonymous]
        [HttpPost]
        public ActionResult JsonRegister(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                KatushaMembershipCreateStatus createStatus;
                _service.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

                if (createStatus == KatushaMembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                KatushaMembershipCreateStatus createStatus;
                _service.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

                if (createStatus == KatushaMembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    //var currentUser = _service.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = _service.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private ActionResult ContextDependentView()
        {
            string actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null)
            {
                ViewBag.FormAction = "Json" + actionName;
                return PartialView();
            }
            else
            {
                ViewBag.FormAction = actionName;
                return View();
            }
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        #region Status Codes
        private static string ErrorCodeToString(KatushaMembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            IResourceManager resourceManager = ResourceManager.GetInstance();
            switch (createStatus)
            {
                case KatushaMembershipCreateStatus.DuplicateUserName:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());                    

                case KatushaMembershipCreateStatus.DuplicateEmail:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());

                case KatushaMembershipCreateStatus.InvalidPassword:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());

                case KatushaMembershipCreateStatus.InvalidEmail:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());

                case KatushaMembershipCreateStatus.InvalidAnswer:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());

                case KatushaMembershipCreateStatus.InvalidQuestion:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());

                case KatushaMembershipCreateStatus.InvalidUserName:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());

                case KatushaMembershipCreateStatus.ProviderError:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());

                case KatushaMembershipCreateStatus.UserRejected:
                    return resourceManager._R("KatushaMembershipCreateStatus." + createStatus.ToString());

                default:
                    return resourceManager._R("KatushaMembershipCreateStatus.Default");
            }
        }
        #endregion

        [HttpPost]
        public void FacebookLogin(string accessToken, string uid)
        {
            var accessToken1 = Request["accessToken"];
            var uid1 = Request["uid"];
            User user = _service.GetUserByFacebookUId(uid1);
            if (user == null) {
                //Create associated user
            } else
                FormsAuthentication.SetAuthCookie(user.UserName, false);
            Session["AccessToken"] = accessToken1;
        }
    }
}
