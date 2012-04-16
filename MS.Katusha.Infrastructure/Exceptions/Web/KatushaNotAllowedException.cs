using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Infrastructure.Exceptions.Web
{
    public class KatushaNotAllowedException : KatushaException
    {

        public KatushaNotAllowedException(Profile profile, User user, string message)
            : base("NotAllowed", message, null)
        {
            Profile = profile;
            User = user;
        }

        public BaseFriendlyModel Profile { get; private set; }
        public User User { get; set; }
    }

}
