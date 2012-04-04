using System;
using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Attributes;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Web.Models.Entities
{
    public class SearchCriteriaModel : BaseModel
    {
        public Sex Gender { get; set; }
        
        [KatushaRegularExpression("Profile.Name")]
        [KatushaStringLength("Profile.Name")]
        [KatushaField("Profile.Name")]
        public string Name { get; set; }

        [KatushaField("Profile.From")]
        public IList<Country> From { get; set; }
        
        [KatushaStringLength("Profile.City")]
        [KatushaField("Profile.City")]
        public IList<string> City { get; set; }

        [KatushaField("Profile.BodyBuild")]
        public IList<BodyBuild> BodyBuild { get; set; }

        [KatushaField("Profile.EyeColor")]
        public IList<EyeColor> EyeColor { get; set; }

        [KatushaField("Profile.HairColor")]
        public IList<HairColor> HairColor { get; set; }

        [KatushaField("Profile.Smokes")]
        public IList<Smokes> Smokes { get; set; }

        [KatushaField("Profile.Alcohol")]
        public IList<Alcohol> Alcohol { get; set; }

        [KatushaField("Profile.Religion")]
        public IList<Religion> Religion { get; set; }

        [KatushaField("Profile.DickSize")]
        public IList<DickSize> DickSize { get; set; }

        [KatushaField("Profile.DickThickness")]
        public IList<DickThickness> DickThickness { get; set; }

        [KatushaField("Profile.BreastSize")]
        public IList<BreastSize> BreastSize { get; set; }

        [KatushaRange("Profile.Height")]
        [KatushaField("Profile.Height")]
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }

        [KatushaRange("Profile.BirthYear")]
        [KatushaField("Profile.BirthYear")]
        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        [KatushaField("Profile.Searches")]
        public IList<LookingFor> Searches { get; set; }
        [KatushaField("Profile.CountriesToVisit")]
        public IList<Country> CountriesToVisit { get; set; }
        [KatushaField("Profile.LanguagesSpoken")]
        public IList<Language> LanguagesSpoken { get; set; }

        public HashSet<string> GetSelectedFieldsList()
        {
            var hs = new HashSet<string>();
            if (City.Count > 0 && City.Any(p => !String.IsNullOrWhiteSpace(p))) hs.Add("City");
            if (From.Count > 0 && From.Any(p => p > 0)) hs.Add("From");
            if (BodyBuild.Count > 0 && BodyBuild.Any(p => p > 0)) hs.Add("BodyBuild");
            if (EyeColor.Count > 0 && EyeColor.Any(p => p > 0)) hs.Add("EyeColor");
            if (HairColor.Count > 0 && HairColor.Any(p => p > 0)) hs.Add("HairColor");
            if (Smokes.Count > 0 && Smokes.Any(p => p > 0)) hs.Add("Smokes");
            if (Alcohol.Count > 0 && Alcohol.Any(p => p > 0)) hs.Add("Alcohol");
            if (Religion.Count > 0 && Religion.Any(p => p > 0)) hs.Add("Religion");
            if (DickSize.Count > 0 && DickSize.Any(p => p > 0)) hs.Add("DickSize");
            if (DickThickness.Count > 0 && DickThickness.Any(p => p > 0)) hs.Add("DickThickness");
            if (BreastSize.Count > 0 && BreastSize.Any(p => p > 0)) hs.Add("BreastSize");
            if (MinHeight > 0) hs.Add("MinHeight");
            if (MaxHeight > 0) hs.Add("MaxHeight");
            if (MinAge > 0) hs.Add("MinAge");
            if (MaxAge > 0) hs.Add("MaxAge");
            if (Searches.Count > 0 && Searches.Any(p => p > 0)) hs.Add("Searches");
            if (CountriesToVisit.Count > 0 && CountriesToVisit.Any(p => p > 0)) hs.Add("CountriesToVisit");
            if (LanguagesSpoken.Count > 0 && LanguagesSpoken.Any(p => p > 0)) hs.Add("LanguagesSpoken");
            return hs;
        }

        public bool CanSearch
        {
            get
            {
                if (!(Gender == Sex.Male || Gender == Sex.Female)) return false;
                var result = (String.IsNullOrWhiteSpace(Name) && From.Count == 0 && City.Count == 0 && BodyBuild.Count == 0 && EyeColor.Count == 0 && HairColor.Count == 0 && Smokes.Count == 0
                              && Alcohol.Count == 0 && Religion.Count == 0 && DickSize.Count == 0 && DickThickness.Count == 0 && BreastSize.Count == 0 && Searches.Count == 0 && CountriesToVisit.Count == 0 && LanguagesSpoken.Count == 0
                              && MinAge == 0 && MaxAge == 0 && MinHeight == 0 && MaxHeight == 0);
                return !result;
            }
        }
    }
}