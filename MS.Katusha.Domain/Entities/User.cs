using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Domain.Enums;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class User : BaseGuidModel
    {
        private Profile _profile;

        //public long ProfileId { get; set; }
        [JsonIgnore]
        public Profile Profile
        {
            get { return _profile; }
            set { 
                _profile = value;
                if (_profile is Girl)
                    Gender = (byte) Sex.Female;
                else if(_profile is Boy)
                    Gender = (byte)Sex.Male;
            }
        }

        [Required]
        public byte Gender { get; private set; }

        [Required]
        [MinLength(3), MaxLength(64)]
        public string UserName { get; set; }

        [Required]
        [MinLength(7), MaxLength(64)]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        [MinLength(6), MaxLength(14)]
        public string Password { get; set; }

        public DateTime Expires { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | Profile: {0} | Gender: {1} | UserName: {2} | Email: {3} | Phone: {4} | Password: {5} | Expires: {6}", Profile, Enum.GetName(typeof(Sex) , Gender), UserName, Email, Phone, Password, Expires);
        }

    }
}