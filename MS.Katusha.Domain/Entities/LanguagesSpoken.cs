using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Domain.Enums;

namespace MS.Katusha.Domain.Entities
{
    public class LanguagesSpoken : BaseModel
    {
        public long ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public byte Language { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Language: {1}", ProfileId, Enum.GetName(typeof(Language), Language));
        }
    }
}