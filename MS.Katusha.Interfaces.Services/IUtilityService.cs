using System;
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
        IList<string> SetExtendedProfile(ExtendedProfile extendedProfile);
        void DeleteProfile(long profileId);
    }

    public class ExtendedProfile
    {
        public Profile Profile { get; set; }
        public IList<PhotoBackup> PhotoBackups { get; set; }
        public string[] CountriesToVisit { get; set; }
        public string[] LanguagesSpoken { get; set; }
        public string[] Searches { get; set; }
        public User User { get; set; }
        public IList<Domain.Raven.Entities.Conversation> Messages { get; set; }
        public Visit[] Visits { get; set; }
    }


}
