using System;
using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;

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

        [KatushaField("Profile.Height")]
        public IList<Height> Height  { get; set; }

        [KatushaField("Profile.Age")]
        public IList<Age> Age { get; set; }

        [KatushaField("Profile.Searches")]
        public IList<LookingFor> LookingFor { get; set; }
        [KatushaField("Profile.CountriesToVisit")]
        public IList<Country> Country { get; set; }
        [KatushaField("Profile.LanguagesSpoken")]
        public IList<Language> Language { get; set; }

        public static SearchCriteriaModel Empty()
        {
            var scm = new SearchCriteriaModel {
                Alcohol = new List<Alcohol>(), 
                BodyBuild = new List<BodyBuild>(),
                BreastSize = new List<BreastSize>(),
                City = new List<string>(),
                Country = new List<Country>(),
                DickSize = new List<DickSize>(),
                DickThickness = new List<DickThickness>(),
                EyeColor = new List<EyeColor>(),
                HairColor = new List<HairColor>(),
                Height = new List<Height>(),
                Age = new List<Age>(),
                Language = new List<Language>(),
                Religion = new List<Religion>(),
                LookingFor = new List<LookingFor>(),
                Smokes = new List<Smokes>(),
                From = new List<Country>(),
                Name = ""
            };
            return scm;
        }

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
            if (Height.Count > 0 && Height.Any(p => p > 0)) hs.Add("Height");
            if (Age.Count > 0 && Age.Any(p => p > 0)) { hs.Add("Age"); hs.Add("BirthYear"); }
            if (LookingFor.Count > 0 && LookingFor.Any(p => p > 0)) hs.Add("LookingFor");
            if (Country.Count > 0 && Country.Any(p => p > 0)) hs.Add("Country");
            if (Language.Count > 0 && Language.Any(p => p > 0)) hs.Add("Language");
            return hs;
        }
    }
}