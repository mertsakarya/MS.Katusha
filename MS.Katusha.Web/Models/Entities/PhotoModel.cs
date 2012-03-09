using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Models.Entities
{
    public class PhotoModel : BaseGuidModel
    {
        [JsonIgnore]
        public long ProfileId { get; set; }
        [JsonIgnore]
        public Web.Models.Entities.ProfileModel Profile { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Description: [\r\n{1}\r\n]", ProfileId, Description);
        }

    }
}