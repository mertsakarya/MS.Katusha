using System.Collections.Generic;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IUtilityService
    {
        void ClearDatabase(string photosFolder);
        void RegisterRaven();
        IEnumerable<string> ResetDatabaseResources();

        ExtendedProfile GetExtendedProfile(long profileId);
    }
}
