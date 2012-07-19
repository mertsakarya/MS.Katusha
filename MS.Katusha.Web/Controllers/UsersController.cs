using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = true, MustBeAdmin = true)]
    public class UsersController : GridController<User>
    {

        public UsersController(IResourceService resourceService, IUserService userService, IGridService<User> gridService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, gridService, profileService, stateService, conversationService)
        {
          
        }
    }
}
