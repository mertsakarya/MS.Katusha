using System;
using System.Collections.Generic;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MS.Katusha.Interfaces.Services.Models
{
    public class ApiProfile
    {
        public ApiProfile()
        {
            Photos = new List<ApiPhoto>();
        }
        public Guid Guid { get; set; }
        public string FriendlyName { get; set; }
        public string Name { get; set; }
        public ApiLocation Location { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BodyBuild BodyBuild { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EyeColor EyeColor { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public HairColor HairColor { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Smokes Smokes { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Alcohol Alcohol { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Religion Religion { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Sex Gender { get; set; }
        public int Height { get; set; }
        public int BirthYear { get; set; }
        public string Description { get; set; }
        public Guid ProfilePhotoGuid { get; set; }
        public IList<ApiPhoto> Photos { get; set; }
    }
}