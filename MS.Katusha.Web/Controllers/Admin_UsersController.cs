using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = true, AllowedRole = UserRole.Administrator)]
    public class Admin_UsersController : GridController<User>
    {

        public Admin_UsersController(IResourceService resourceService, IUserService userService, IGridService<User> gridService, IProfileService profileService, IStateService stateService, IConversationService conversationService, ITokBoxService tokBoxService)
            : base(resourceService, userService, gridService, profileService, stateService, conversationService, tokBoxService)
        {
          
        }
    }
}
