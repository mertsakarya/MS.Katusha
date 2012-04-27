using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class LanguagesSpoken : BaseModel
    {
        [JsonIgnore]
        public long ProfileId { get; set; }
        [JsonIgnore]
        public Profile Profile { get; set; }
        [StringLength(2)]
        public string Language { get; set; }

        public override string ToString()
        {
            return Language;
        }
    }
}