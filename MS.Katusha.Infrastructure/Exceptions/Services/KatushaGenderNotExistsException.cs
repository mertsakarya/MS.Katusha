using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Infrastructure.Exceptions.Services
{
    public class KatushaGenderNotExistsException : KatushaException
    {
        public KatushaGenderNotExistsException(BaseFriendlyModel profile)
            : base("GenderNotExists", "Must be male or female sorry!!!", null)
        {
            Profile = profile;
        }

        public BaseFriendlyModel Profile { get; private set; }
    }

}
