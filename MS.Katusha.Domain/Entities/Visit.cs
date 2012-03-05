using System;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class Visit : BaseGuidModel
    {
        public long ProfileId { get; set; }
        public long VisitorProfileId { get; set; }
        
        public Profile Profile { get; set; }
        public Profile VisitorProfile { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | VisitorProfileId: {1}", ProfileId, VisitorProfileId);
        }
    }
}