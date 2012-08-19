using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services.Models;

namespace MS.Katusha.Interfaces.Services
{
    public interface IUtilityService
    {
        void ClearDatabase();
        void RegisterRaven();
        IEnumerable<string> ResetDatabaseResources();

        IExtendedProfile GetExtendedProfile(User katushaUser, long profileId);
        IList<string> SetExtendedProfile(AdminExtendedProfile extendedProfile);
        void DeleteProfile(Guid guid);
    }
}
