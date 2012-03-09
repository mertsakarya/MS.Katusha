using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Models.Entities
{
    public class LanguagesSpokenModel : BaseModel
    {
        [JsonIgnore]
        public long ProfileId { get; set; }
        [JsonIgnore]
        public Web.Models.Entities.ProfileModel Profile { get; set; }
        public byte Language { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Language: {1}", ProfileId, Enum.GetName(typeof(Language), Language));
        }
    }
}