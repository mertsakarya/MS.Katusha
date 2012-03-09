using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Models.Entities
{
    public class ProfileModel : BaseFriendlyModel
    {
        public ProfileModel()
        {
            Searches = new List<SearchingForModel>();
            Photos = new List<PhotoModel>();
            CountriesToVisit = new List<CountriesToVisitModel>();
            LanguagesSpoken = new List<LanguagesSpokenModel>();
            SentMessages = new List<ConversationModel>();
            RecievedMessages = new List<ConversationModel>();
            WhoVisited = new List<VisitModel>();
            Visited = new List<VisitModel>();
        }

        //[Required]
        //public long UserId { get; set; }
        
        public UserModel User { get; set; }

        // public long StateId { get; set; }
        public StateModel State { get; set; }

        public string Name { get; set; }

        public Country From { get; set; }

        public string City { get; set; }
        public BodyBuild BodyBuild { get; set; }
        public EyeColor EyeColor { get; set; }
        public HairColor HairColor { get; set; }
        public Smokes Smokes { get; set; }
        public Alcohol Alcohol { get; set; }
        public Religion Religion { get; set; }

        public int Height { get; set; }
        public int BirthYear { get; set; }

        public string Description { get; set; }

        public IList<SearchingForModel> Searches { get; set; }
        public IList<PhotoModel> Photos { get; set; }
        public IList<CountriesToVisitModel> CountriesToVisit { get; set; }
        public IList<LanguagesSpokenModel> LanguagesSpoken { get; set; }

        public IList<ConversationModel> SentMessages { get; set; }
        public IList<ConversationModel> RecievedMessages { get; set; }

        public IList<VisitModel> WhoVisited { get; set; }
        public IList<VisitModel> Visited { get; set; }

        public override string ToString()
        {
            var str = base.ToString() +
                      String.Format(
                          " | UserId: {0} | State: {1} | Name: {2} | From: {3} | City: {4} | BodyBuild: {5} | EyeColor: {6} | HairColor: {7} | Smokes: {8} | Alcohol: {9} | Religion: {10} | Height: {11} | BirthYear: {12}",
                          (User == null) ? 0 : User.Id, State, Name,
                          Enum.GetName(typeof (Country), From),
                          City,
                          Enum.GetName(typeof (BodyBuild), BodyBuild),
                          Enum.GetName(typeof (EyeColor), EyeColor),
                          Enum.GetName(typeof (HairColor), HairColor),
                          Enum.GetName(typeof (Smokes), Smokes),
                          Enum.GetName(typeof (Alcohol), Alcohol),
                          Enum.GetName(typeof (Religion), Religion),
                          Height,
                          BirthYear
                          );

            if (Searches != null && Searches.Count > 0)
            {
                str += "\r\nSearches: [\r\n";
                foreach (var search in Searches)
                    str += "\t" + search.ToString() + "\r\n";
                str += "]";
            }

            if (Photos != null && Photos.Count > 0)
            {
                str += "\r\nPhotos: [\r\n";
                foreach (var photo in Photos)
                    str += "\t" + photo + "\r\n";
                str += "]";
            }

            if (CountriesToVisit != null && CountriesToVisit.Count > 0)
            {
                str += "\r\nCountriesToVisit: [\r\n";
                foreach (var countriesToVisit in CountriesToVisit)
                    str += "\t" + countriesToVisit + "\r\n";
                str += "]";
            }

            if (LanguagesSpoken != null && LanguagesSpoken.Count > 0)
            {
                str += "\r\nLanguagesSpoken: [\r\n";
                foreach (var languagesSpoken in LanguagesSpoken)
                    str += "\t" + languagesSpoken + "\r\n";
                str += "]";
            }
            return str;
        }
    }
}