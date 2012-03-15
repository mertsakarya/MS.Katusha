using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Exceptions.Services
{
    public class KatushaFriendlyNameExistsException : KatushaException
    {
        public KatushaFriendlyNameExistsException(BaseFriendlyModel profile)
            : base("FriendlyNameExists", "Cannot create or update this friendlyName", null)
        {
            Profile = profile;
        }

        public BaseFriendlyModel Profile { get; private set; }
    }

}
