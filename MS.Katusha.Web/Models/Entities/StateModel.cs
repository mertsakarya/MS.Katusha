using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Models.Entities
{
    public class StateModel : BaseModel
    {
        [Required]
        public long ProfileId { get; set; }
        [JsonIgnore]
        public ProfileModel Profile { get; set; }

        public MembershipType MembershipType { get; set; }
        public Status Status { get; set; }
        public Existance Existance { get; set; }
        public DateTime LastOnline { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format("[ProfileId: {0} | MembershipType: {1} | Status: {2} | Existance: {3} | LastOnline: {4}]", ProfileId, Enum.GetName(typeof(MembershipType) , MembershipType), Enum.GetName(typeof(Status) , Status), Enum.GetName(typeof(Existance) , Existance), LastOnline);
        }
    }
}