using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Models.Entities
{
    public class SearchingForModel : BaseModel
    {
        [JsonIgnore]
        public long ProfileId { get; set; }
        [JsonIgnore]
        public ProfileModel Profile { get; set; }
        public byte Search { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Search: {1}", ProfileId, Enum.GetName(typeof(LookingFor) , Search));
        }
    }
}