using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Infrastructure.Exceptions.Services
{
    public class KatushaFriendlyNameExistsException : KatushaException
    {
        public KatushaFriendlyNameExistsException(BaseFriendlyModel profile)
            : base("FriendlyNameExists", null)
        {
            Profile = profile;
        }

        public BaseFriendlyModel Profile { get; private set; }
    }

}
