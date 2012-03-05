using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Enums;

namespace MS.Katusha.Web.Models
{
    public abstract class BaseModel
    {
        public long Id { get; set; }
        public DateTime LastModified { get; set; }
    }

    public abstract class BaseGuidModel : BaseModel
    {
        public Guid Guid { get; set; }
    }

    public abstract class BaseFriendlyModel : BaseGuidModel
    {
        public String UrlFriendlyId { get; set; }
    }

    public class Profile : BaseFriendlyModel
    {
        public string Name { get; set; }
        public MembershipType MembershipType { get; set; }
        public Status Status { get; set; }
        public Existance Existance { get; set; }
        public Country From { get; set; }

        [MaxLength(64)]
        [MinLength(3)]
        public string City { get; set; }
        public BodyBuild BodyBuild { get; set; }
        public EyeColor EyeColor { get; set; }
        public HairColor HairColor { get; set; }
        public Smokes Smokes { get; set; }
        public Alcohol Alcohol { get; set; }
        public Religion Religion { get; set; }

        public DateTime LastOnline { get; set; }
        public DateTime Expires { get; set; }
        public DateTime CreationTime { get; set; }

        public int Height { get; set; }
        public int Weight { get; set; }
        public string Description { get; set; }
    }

    public class CountriesToVisit : BaseModel
    {
        public long ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public byte Country { get; set; }
    }

    public class LanguagesSpoken : BaseModel
    {
        public long ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public byte Language { get; set; }

    }

    public class Photo : BaseGuidModel
    {
        public long ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public string Description { get; set; }
    }

    public class Girl : Profile
    {
        public byte BreastSize { get; set; }
    }

    public class Boy : Profile
    {
        public byte DickSize { get; set; }
        public byte DickThickness { get; set; }
    }

    public class User : BaseGuidModel
    {

        public byte Gender { get; set; }

        [MinLength(3), MaxLength(64)]
        public string UserName { get; set; }

        [MinLength(7), MaxLength(64)]
        public string Email { get; set; }

        public string Phone { get; set; }

        [MinLength(6), MaxLength(14)]
        public string Password { get; set; }
    }
}