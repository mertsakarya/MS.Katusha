using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Web.Models.Entities.BaseEntities;


namespace MS.Katusha.Web.Models.Entities
{
    public class FacebookProfileModel : BaseGuidModel
    {
        [KatushaRegularExpression("Profile.Name")]
        [KatushaStringLength("Profile.Name")]
        [KatushaField("Profile.Name")]
        [KatushaRequired("Profile.Name")]
        public string Name { get; set; }

        public LocationModel Location { get; set; }

        //[KatushaStringLength("Profile.City")]
        //[KatushaField("Profile.City")]
        //public City City { get; set; }


        [KatushaField("Profile.Gender")]
        [KatushaRequired("Profile.Gender")]
        public Sex Gender { get; set; }

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

        [KatushaRequired("Profile.Description")]
        [KatushaStringLength("Profile.Description")]
        [KatushaField("Profile.Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [KatushaRequired("User.Email")]
        [KatushaStringLength("User.Email")]
        [KatushaField("User.Email")]
        public string Email { get; set; }

        public string Album { get; set; }
        public StringDictionary Albums { get; set; }

        public string FacebookId { get; set; }
    }
}