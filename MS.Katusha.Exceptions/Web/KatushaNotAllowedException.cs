using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Exceptions.Web
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
        public string Key { get; set; }
    }

}
