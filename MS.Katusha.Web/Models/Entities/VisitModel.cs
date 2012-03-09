using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Models.Entities
{
    public class VisitModel : BaseGuidModel
    {
        public long ProfileId { get; set; }
        public long VisitorProfileId { get; set; }

        [JsonIgnore]
        public ProfileModel Profile { get; set; }
        [JsonIgnore]
        public ProfileModel VisitorProfile { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | VisitorProfileId: {1}", ProfileId, VisitorProfileId);
        }
    }
}