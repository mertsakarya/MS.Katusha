using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class SearchingFor : BaseModel
    {
        [JsonIgnore]
        public long ProfileId { get; set; }
        [JsonIgnore]
        public Profile Profile { get; set; }
        public byte Search { get; set; }

        public override string ToString()
        {
            return Enum.GetName(typeof(LookingFor) , Search);
        }
    }
}