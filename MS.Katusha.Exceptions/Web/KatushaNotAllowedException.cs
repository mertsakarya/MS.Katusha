using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Exceptions.Services
{
    public class KatushaNotAllowedException : KatushaException
    {

        public KatushaNotAllowedException(Profile profile, User user, string key)
            : base("NotAllowed", "You cannot do this!", null)
        {
            Profile = profile;
            User = user;
            Key = key;
        }

        public BaseFriendlyModel Profile { get; private set; }
        public User User { get; set; }
        public string Key { get; set; }
    }

}
