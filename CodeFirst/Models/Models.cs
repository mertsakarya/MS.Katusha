using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeFirst.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeFirst.Models
{
    
    public enum Status: byte { Online=1, Away, Offline }
    public enum MembershipType : byte { Normal=0, Gold=1, Platinium=2 }
    public enum Existance : byte { Active=0, Expired=1 }
    public enum BreastSize : byte { Small=1, Medium, Large, ExtraLarge }
    public enum DickSize : byte { Small=1, Medium, Large, ExtraLarge }
    public enum DickThickness : byte { Narrow=1, Wide, Thick, VeryThick }
    public enum Language : byte { Turkish=1, Russian, English }
    public enum Country : byte { Turkey=1, Ukraine, Russia, UnitedStates }
    


    //public abstract class Lookup : BaseModel
    //{
    //    public string Name { get; set; }
    //}

    //public class Status : Lookup { }
    //public class Country : Lookup { }
    //public class Language : Lookup { }
    //public class MembershipType : Lookup { }
    //public class Existance : Lookup {}
    //public class BreastSize : Lookup {}
    //public class DickSize : Lookup {}
    //public class DickThickness : Lookup {}



    public abstract class BaseModel
    {
        public int Id { get; set; }
        [Timestamp]
        public byte[] ts { get; set; }
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

    public abstract class Profile : BaseFriendlyModel
    {
        public Profile()
        {
            this.Photos = new HashSet<Photo>();
            this.CountriesToVisit = new HashSet<CountriesToVisit>();
            this.LanguagesSpoken = new HashSet<LanguagesSpoken>();
        }
        public int Height { get; set; }
        public string Description { get; set; }
        public ProfileStatus ProfileStatus { get; set; }
        public ICollection<Photo> Photos { get; private set; }
        public byte From { get; set; }
        public string City { get; set; }
        public ICollection<CountriesToVisit> CountriesToVisit { get; private set; }
        public ICollection<LanguagesSpoken> LanguagesSpoken { get; private set; }
    }

    public class CountriesToVisit : BaseModel
    {
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public byte Country { get; set; }
    }

    public class LanguagesSpoken : BaseModel
    {
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public byte Language { get; set; }
    }

    public class ProfileStatus : BaseGuidModel
    {
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public byte MembershipType { get; set; }
        public byte Status { get; set; }
        public byte Existance { get; set; }
        public DateTime LastOnline { get; set; }
        public DateTime Expires { get; set; }
    }

    public class Photo : BaseGuidModel
    {
        public int ProfileId { get; set; }
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
}