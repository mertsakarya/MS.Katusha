using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Web.Controllers
{
    public class BoysController :  ProfileController<Boy, BoyModel>
    {

        public BoysController(IBoyService boyService, IUserService userService)
            : base(boyService, userService)
        {

        }
    }
}
