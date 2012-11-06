
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = true, AllowedRole = UserRole.Administrator)]
    public class Admin_ProfilesController : GridController<MS.Katusha.Domain.Entities.Profile>
    {

        public Admin_ProfilesController(IResourceService resourceService, IUserService userService, IGridService<MS.Katusha.Domain.Entities.Profile> gridService, IProfileService profileService, IStateService stateService, IConversationService conversationService)
            : base(resourceService, userService, gridService, profileService, stateService, conversationService)
        {
          
        }
    }
}
