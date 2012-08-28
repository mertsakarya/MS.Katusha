using System;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MS.Katusha.Domain.Service
{
    public class ApiAdminUser : ApiUser
    {
        [JsonConverter(typeof(EncryptedStringConverter))]
        public string Password { get; set; }
        public string FacebookUid { get; set; }
        public string PaypalPayerId { get; set; }
        public DateTime Expires { get; set; }
        [JsonConverter(typeof (StringEnumConverter))]
        public MembershipType MembershipType { get; set; }
        [JsonConverter(typeof (StringEnumConverter))]
        public UserRole UserRole { get; set; }
        public bool EmailValidated { get; set; }
    }

}