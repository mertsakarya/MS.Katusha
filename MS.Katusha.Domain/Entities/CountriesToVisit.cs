using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Domain.Enums;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class CountriesToVisit : BaseModel
    {
        public long ProfileId { get; set; }
        [JsonIgnore]
        public Profile Profile { get; set; }
        public byte Country { get; set; }


        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Country: {1}", ProfileId, Enum.GetName(typeof(Country), Country));
        }
    }
}