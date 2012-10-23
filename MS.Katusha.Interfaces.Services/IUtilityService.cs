using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Service;

namespace MS.Katusha.Interfaces.Services
{
    public interface IUtilityService
    {
        void ClearDatabase();
        void RegisterRaven();
        IEnumerable<string> SetDatabaseResources();
        void DeleteDatabaseResources();
        void UpdateRavenProfiles();

        IExtendedProfile GetExtendedProfile(User katushaUser, long profileId);
        IList<string> SetExtendedProfile(AdminExtendedProfile extendedProfile);
        void DeleteProfile(Guid guid);
        IList<string> BackupAndDeleteProfile(User user, Guid guid);
        IList<string> BackupProfile(User katushaUser, Guid guid, string folder = null);
        IList<string> RestoreProfile(User katushaUser, Guid guid, bool isDeleted);
    }
}
