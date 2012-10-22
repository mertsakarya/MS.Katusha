using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Web.Controllers
{
    public interface IKatushaBase {
        User KatushaUser { get; set; }
        Profile KatushaProfile { get; set; }
        TokBoxSession TokBoxSession { get; }

        ITokBoxService TokBoxService { get; }
        IUserService UserService { get; }
        IProfileService ProfileService { get; }
        IStateService StateService { get; }
        IResourceService ResourceService { get; set; }
        IConversationService ConversationService { get; set; }
    }
}