using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Models
{
    public class PageModel
    {
        public UserModel ActiveUser { get; set; }
        public ProfileModel ActiveProfile { get; set; }
        public string FacebookAccessToken { get; set; }
        public bool SameProfile { get; set; }
        public string Title { get; set; }
    }
}