using System;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class Photo : BaseGuidModel
    {
        public long ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Description: [\r\n{1}\r\n]", ProfileId, Description);
        }

    }
}