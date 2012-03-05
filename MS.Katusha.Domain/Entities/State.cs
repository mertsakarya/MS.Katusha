using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Domain.Enums;

namespace MS.Katusha.Domain.Entities
{
    public class State : BaseModel
    {
        [Required]
        public long ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public byte MembershipType { get; set; }
        public byte Status { get; set; }
        public byte Existance { get; set; }
        public DateTime LastOnline { get; set; }
        public DateTime Expires { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format("[ProfileId: {0} | MembershipType: {1} | Status: {2} | Existance: {3} | LastOnline: {4} | Expires: {5}]", ProfileId, Enum.GetName(typeof(MembershipType) , MembershipType), Enum.GetName(typeof(Status) , Status), Enum.GetName(typeof(Existance) , Existance), LastOnline, Expires);
        }
    }
}