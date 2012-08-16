using System.Collections.Generic;

namespace MS.Katusha.Interfaces.Services.Models
{
    public class AdminExtendedProfile : ApiExtendedProfile
    {
        public AdminExtendedProfile(IExtendedProfile apiExtendedProfile)
        {
            var profile = apiExtendedProfile as ApiExtendedProfile;
            if (profile != null) {
                CountriesToVisit = profile.CountriesToVisit;
                this.LanguagesSpoken = profile.LanguagesSpoken;
                this.Profile = profile.Profile;
                this.Searches = profile.Searches;
            }
        }

        public IList<ApiPhotoBackup> PhotoBackups { get; set; }
        public IList<ApiConversation> Messages { get; set; }
        public new ApiAdminUser User { get; set; }

    }
}
