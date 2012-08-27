
namespace MS.Katusha.Domain.Service
{
    public class ApiExtendedProfile : IExtendedProfile
    {
        public ApiProfile Profile { get; set; }
        public string[] CountriesToVisit { get; set; }
        public string[] LanguagesSpoken { get; set; }
        public string[] Searches { get; set; }
        public ApiUser User { get; set; }
    }
}