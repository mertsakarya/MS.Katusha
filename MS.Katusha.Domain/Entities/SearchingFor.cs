using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Domain.Enums;

namespace MS.Katusha.Domain.Entities
{
    public class SearchingFor : BaseModel
    {
        public long ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public byte Search { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Search: {1}", ProfileId, Enum.GetName(typeof(LookingFor) , Search));
        }
    }
}