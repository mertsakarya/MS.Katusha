//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using MS.Katusha.Domain.Enums;

//namespace MS.Katusha.Domain
//{

//    #region Base Classes
//    public abstract class BaseModel : ICloneable
//    {
//        [Key]
//        public long Id { get; set; }
//        [Timestamp]
//        public byte[] ts { get; set; }
//        public DateTime LastModified { get; set; }

//        public abstract object Clone();
//    }

//    public abstract class BaseGuidModel : BaseModel
//    {
//        [Required]
//        public Guid Guid { get; set; }
//    }

//    public abstract class BaseFriendlyModel : BaseGuidModel
//    {
//        public String UrlFriendlyId { get; set; }
//    }
//    #endregion

//    public class State : BaseModel
//    {
//        [Required]
//        public long ProfileId { get; set; }
//        public virtual Profile Profile { get; set; }

//        public byte MembershipType { get; set; }
//        public byte Status { get; set; }
//        public byte Existance { get; set; }
//        public DateTime LastOnline { get; set; }
//        public DateTime Expires { get; set; }
//        public DateTime CreationTime { get; set; }

//        public override object Clone()
//        {
//            var obj = new State
//                          {
//                              MembershipType = MembershipType,
//                              Status = Status,
//                              Existance = Existance,
//                              LastOnline = LastOnline,
//                              Expires = Expires,
//                              CreationTime = CreationTime,
//                              Id = Id,
//                              LastModified = DateTime.Now.ToUniversalTime()
//                          };
//            return obj;
//        }
//    }

//    public class Profile : BaseFriendlyModel
//    {
//        //[Required]
//        //public long UserId { get; set; }
//        public virtual User User { get; set; }

//        public long StateId { get; set; }
//        public State State { get; set; }


//        public string Name { get; set; }

//        public byte From { get; set; }
//        public string City { get; set; }
//        public byte BodyBuild { get; set; }
//        public byte EyeColor { get; set; }
//        public byte HairColor { get; set; }
//        public byte Smokes { get; set; }
//        public byte Alcohol { get; set; }
//        public byte Religion { get; set; }

//        public int Height { get; set; }
//        public int BirthYear { get; set; }

//        public string Description { get; set; }

//        public HashSet<SearchingFor> Searches { get; set; }
//        public HashSet<Photo> Photos { get; set; }
//        public HashSet<CountriesToVisit> CountriesToVisit { get; set; }
//        public HashSet<LanguagesSpoken> LanguagesSpoken { get; set; }

//        public override object Clone()
//        {
//            var obj = new Profile();
//            SetProfile(this, obj);
//            return obj;
//        }

//        protected static void SetProfile(Profile from, Profile to)
//        {
//            to.BirthYear = from.BirthYear;
//            to.Name = from.Name;
//            to.LastModified = DateTime.Now.ToUniversalTime();
//            to.Height = from.Height;
//            to.Description = from.Description;
//            to.From = from.From;
//            to.City = from.City;
//            to.Id = from.Id;
//            to.Guid = Guid.NewGuid();
//            to.UrlFriendlyId = from.UrlFriendlyId;
//            to.State = (State) from.State.Clone();
//            SetCollections(from, to);
//        }

//        protected static void SetCollections(Profile from, Profile to)
//        {
//            if (from.Searches != null)
//            {
//                to.Searches = new HashSet<SearchingFor>();
//                foreach (var o in from.Searches) to.Searches.Add((SearchingFor)o.Clone());
//            }

//            if (from.Photos != null)
//            {
//                to.Photos = new HashSet<Photo>();
//                foreach (var o in from.Photos) to.Photos.Add((Photo)o.Clone());
//            }
//            if (from.LanguagesSpoken != null)
//            {
//                to.LanguagesSpoken = new HashSet<LanguagesSpoken>();
//                foreach (var o in from.LanguagesSpoken) to.LanguagesSpoken.Add((LanguagesSpoken)o.Clone());
//            }
//            if (from.CountriesToVisit != null)
//            {
//                to.CountriesToVisit = new HashSet<CountriesToVisit>();
//                foreach (var o in from.CountriesToVisit) to.CountriesToVisit.Add((CountriesToVisit)o.Clone());
//            }
//        }
//    }

//    public class Visitor : BaseGuidModel
//    {
//        public long ProfileId { get; set; }
//        public long VisitorProfileId { get; set; }
//        public virtual Profile Profile { get; set; }
//        public virtual Profile VisitorProfile { get; set; }
//        public DateTime VisitDate { get; set; }

//        public override object Clone()
//        {
//            var obj = new Visitor
//            {
//                VisitorProfileId = VisitorProfileId,
//                Id = Id,
//                Guid = Guid.NewGuid(),
//                ProfileId = ProfileId,
//                VisitDate = VisitDate,
//                LastModified = DateTime.Now.ToUniversalTime()
//            };
//            return obj;
//        }
//    }

//    public class Conversation : BaseGuidModel
//    {
//        [Required]
//        public long FromId { get; set; }

//        [Required]
//        public long ToId { get; set; }

//        [Required]
//        [MinLength(7), MaxLength(8000)]
//        public string Message { get; set; }

//        [Required]
//        public DateTime Date { get; set; }

//        public DateTime ReadDate { get; set; }

//        public override object Clone()
//        {
//            var obj = new Conversation
//            {
//                Guid = Guid.NewGuid(),
//                Id = Id,
//                FromId = FromId,
//                ToId = ToId,
//                Message = Message,
//                ReadDate = ReadDate,
//                Date = Date,
//                LastModified = DateTime.Now.ToUniversalTime()
//            };
//            return obj;
//        }
//    }
   
//    public class CountriesToVisit : BaseModel
//    {
//        public long ProfileId { get; set; }
//        public virtual Profile Profile { get; set; }
//        public byte Country { get; set; }
//        public override object Clone()
//        {
//            var obj = new CountriesToVisit
//            {
//                Country = Country,
//                Id = Id,
//                ProfileId = ProfileId,
//                LastModified = DateTime.Now.ToUniversalTime()
//            };
//            return obj;
//        }
//    }

//    public class SearchingFor : BaseModel
//    {
//        public long ProfileId { get; set; }
//        public virtual Profile Profile { get; set; }
//        public byte Search { get; set; }

//        public override object Clone()
//        {
//            var obj = new SearchingFor
//            {
//                Search = Search,
//                Id = Id,
//                ProfileId = ProfileId,
//                LastModified = DateTime.Now.ToUniversalTime()
//            };
//            return obj;
//        }
//    }

//    public class LanguagesSpoken : BaseModel
//    {
//        public long ProfileId { get; set; }
//        public virtual Profile Profile { get; set; }
//        public byte Language { get; set; }

//        public override object Clone()
//        {
//            var obj = new LanguagesSpoken
//            {
//                Language = Language,
//                Id = Id,
//                ProfileId = ProfileId,
//                LastModified = DateTime.Now.ToUniversalTime()
//            };
//            return obj;
//        }
//    }

//    public class Photo : BaseGuidModel
//    {
//        public long ProfileId { get; set; }
//        public virtual Profile Profile { get; set; }
//        public string Description { get; set; }

//        public override object Clone()
//        {
//            var obj = new Photo
//            {
//                Description = Description,
//                Id = Id,
//                Guid = Guid.NewGuid(),
//                ProfileId = ProfileId,
//                LastModified = DateTime.Now.ToUniversalTime()
//            };
//            return obj;
//        }
//    }

//    public class Girl : Profile
//    {
//        public byte BreastSize { get; set; }

//        public override object Clone()
//        {
//            var obj = new Girl();
//            SetProfile(this, obj);
 
//            obj.BreastSize = BreastSize;
            
//            return obj;
//        }

//    }

//    public class Boy : Profile
//    {
//        public byte DickSize { get; set; }
//        public byte DickThickness { get; set; }

//        public override object Clone()
//        {
//            var obj = new Boy();
//            SetProfile(this, obj);

//            obj.DickSize = DickSize;
//            obj.DickThickness = DickThickness;
//            return obj;
//        }
//    }

//    public class User : BaseGuidModel
//    {
//        private Profile _profile;

//        public long ProfileId { get; set; }
//        public Profile Profile
//        {
//            get { return _profile; }
//            set { 
//                _profile = value;
//                ProfileId = _profile.Id;
//                if (_profile is Girl)
//                    Gender = (byte) Sex.Female;
//                else if(_profile is Boy)
//                    Gender = (byte)Sex.Male;
//            }
//        }

//        [Required]
//        public byte Gender { get; private set; }

//        [Required]
//        [MinLength(3), MaxLength(64)]
//        public string UserName { get; set; }

//        [Required]
//        [MinLength(7), MaxLength(64)]
//        public string Email { get; set; }

//        public string Phone { get; set; }

//        [Required]
//        [MinLength(6), MaxLength(14)]
//        public string Password { get; set; }

//        public override object Clone()
//        {
//            var obj = new User
//            {
//                Profile = (Profile)Profile.Clone(),
//                Phone = Phone,
//                Gender = Gender,
//                Email = Email,
//                Guid = Guid,
//                Id = Id,
//                Password = Password,
//                UserName = UserName,
//                LastModified = DateTime.Now.ToUniversalTime(),
//            };
//            return obj;
//        }
//    }
//}