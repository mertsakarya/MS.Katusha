using System.Collections.Generic;
using System.IO;
using System.Text;
using MS.Katusha.Domain.Entities;
using Newtonsoft.Json;

namespace MS.Katusha.Interfaces.Services
{
    public interface IUtilityService
    {
        void ClearDatabase(string photosFolder);
        void RegisterRaven();
        IEnumerable<string> ResetDatabaseResources();

        ExtendedProfile GetExtendedProfile(long profileId);
        IList<string> SetExtendedProfile(ExtendedProfile extendedProfile);
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
    }


}
