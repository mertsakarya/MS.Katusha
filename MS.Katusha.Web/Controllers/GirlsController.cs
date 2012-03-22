using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models.Entities;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Web.Controllers.BaseControllers;

namespace MS.Katusha.Web.Controllers
{
    public class GirlsController : ProfileController<Girl, GirlModel>
    {
        public GirlsController(IGirlService girlService, IUserService userService, IResourceManager resourceManager) : base(girlService, userService, resourceManager) {}
    }
}
