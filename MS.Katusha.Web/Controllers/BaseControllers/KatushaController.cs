using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Web.Controllers.BaseControllers
{
    public class KatushaController : Controller
    {
        public User KatushaUser { get; set; }
        public Sex Gender { get; set; }

        public IUserService UserService { get; private set; }

        public KatushaController(IUserService userService)
        {
            UserService = userService;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            KatushaUser = (User.Identity.IsAuthenticated) ? UserService.GetUser(User.Identity.Name) : null;
            ViewBag.KatushaUser = KatushaUser;
        }

    }
}