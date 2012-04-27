using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class SearchCriteria : BaseModel
    {
        public Sex Gender { get; set; }
        public string Name { get; set; }
        public IList<string> From { get; set; }
        public IList<string> City { get; set; }
        public IList<BodyBuild?> BodyBuild { get; set; }
        public IList<EyeColor?> EyeColor { get; set; }
        public IList<HairColor?> HairColor { get; set; }
        public IList<Smokes?> Smokes { get; set; }
        public IList<Alcohol?> Alcohol { get; set; }
        public IList<Religion?> Religion { get; set; }
        public IList<DickSize?> DickSize { get; set; }
        public IList<DickThickness?> DickThickness { get; set; }
        public IList<BreastSize?> BreastSize { get; set; }

        public IList<Age?> Age { get; set; }
        public IList<Height?> Height { get; set; }

        public IList<LookingFor> LookingFor { get; set; }
        public IList<string> Country { get; set; }
        public IList<string> Language { get; set; }

        public bool CanSearch
        {
            get
            {
                if (!(Gender == Sex.Male || Gender == Sex.Female)) return false;
                var result = (String.IsNullOrWhiteSpace(Name) && From.Count == 0 && City.Count == 0 && BodyBuild.Count == 0 && EyeColor.Count == 0 && HairColor.Count == 0 && Smokes.Count == 0
                              && Alcohol.Count == 0 && Religion.Count == 0 && DickSize.Count == 0 && DickThickness.Count == 0 && BreastSize.Count == 0 && LookingFor.Count == 0 && Country.Count == 0 && Language.Count == 0
                              && Age.Count == 0 && Height.Count == 0);
                return !result;
            }
        }
    }
}