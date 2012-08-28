using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class Visit : BaseModel
    {
        public long ProfileId { get; set; }
        public long VisitorProfileId { get; set; }
        public int VisitCount { get; set; }

        [JsonIgnore]
        public Profile Profile { get; set; }
        [JsonIgnore]
        public Profile VisitorProfile { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | VisitorProfileId: {1} | Count: {2}", ProfileId, VisitorProfileId, VisitCount);
        }
    }
}