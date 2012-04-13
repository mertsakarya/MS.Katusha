using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Web.Models.Entities.BaseEntities;


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

        public StateModel State { get; set; }

        [KatushaRegularExpression("Profile.Name")]
        [KatushaStringLength("Profile.Name")]
        [KatushaField("Profile.Name")]
        [KatushaRequired("Profile.Name")]
        public string Name { get; set; }

        [KatushaField("Profile.From")]
        public Country? From { get; set; }

        [KatushaStringLength("Profile.City")]
        [KatushaField("Profile.City")]
        public string City { get; set; }

        [KatushaField("Profile.BodyBuild")]
        public BodyBuild? BodyBuild { get; set; }

        [KatushaField("Profile.EyeColor")]
        public EyeColor? EyeColor { get; set; }

        [KatushaField("Profile.HairColor")]
        public HairColor? HairColor { get; set; }

        [KatushaField("Profile.Smokes")]
        public Smokes? Smokes { get; set; }

        [KatushaField("Profile.Alcohol")]
        public Alcohol? Alcohol { get; set; }

        [KatushaField("Profile.Religion")]
        public Religion? Religion { get; set; }

        [KatushaField("Profile.DickSize")]
        public DickSize? DickSize { get; set; }

        [KatushaField("Profile.DickThickness")]
        public DickThickness? DickThickness { get; set; }

        [KatushaField("Profile.BreastSize")]
        public BreastSize? BreastSize { get; set; }
        
        [KatushaRange("Profile.Height")]
        [KatushaField("Profile.Height")]
        public int? Height { get; set; }

        [KatushaRange("Profile.BirthYear")]
        [KatushaField("Profile.BirthYear")]
        public int? BirthYear { get; set; }

        [KatushaField("Profile.Age")]
        [ReadOnly(true)]
        public int? Age { get { return DateTime.Now.Year - BirthYear; } }

        [KatushaRequired("Profile.Description")]
        [KatushaStringLength("Profile.Description")]
        [KatushaField("Profile.Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ReadOnly(true)]
        public Sex Gender { get; set; }

        public Guid ProfilePhotoGuid { get; set; }

        [KatushaField("Profile.Searches")]
        public IList<SearchingForModel> Searches { get; set; }
        [KatushaField("Profile.CountriesToVisit")]
        public IList<CountriesToVisitModel> CountriesToVisit { get; set; }
        [KatushaField("Profile.LanguagesSpoken")]
        public IList<LanguagesSpokenModel> LanguagesSpoken { get; set; }

        public IList<PhotoModel> Photos { get; set; }

        public IList<ConversationModel> SentMessages { get; set; }
        public IList<ConversationModel> RecievedMessages { get; set; }

        public IList<VisitModel> WhoVisited { get; set; }
        public IList<VisitModel> Visited { get; set; }

        public override string ToString()
        {
            var str = base.ToString() +
                      String.Format(
                          " | UserId: {0} | State: {1} | Name: {2} | From: {3} | City: {4} | BodyBuild: {5} | EyeColor: {6} | HairColor: {7} | Smokes: {8} | Alcohol: {9} | Religion: {10} | Height: {11} | BirthYear: {12}",
                          0/*(User == null) ? 0 : User.Id*/, State, Name,
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