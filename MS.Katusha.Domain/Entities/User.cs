using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Entities
{
    public class User : BaseGuidModel
    {
        [Required]
        [MinLength(6), MaxLength(14)]
        public string Password { get; set; }
        public string FacebookUid { get; set; }
        public string PaypalPayerId { get; set; }

        public byte Gender { get; set; }

        [Required]
        [MinLength(3), MaxLength(64)]
        public string UserName { get; set; }

        [Required]
        [MinLength(7), MaxLength(64)]
        public string Email { get; set; }

        public bool EmailValidated { get; set; }

        public string Phone { get; set; }

        public long UserRole { get; set; }

        public DateTime Expires { get; set; }
        public byte MembershipType { get; set; }


        public override string ToString()
        {
            return base.ToString() + String.Format(" | UserName: {0} | UserRole: {1} | Gender: {2} | Email: {3} | Expires: {4} | Membership: {5}", UserName, Enum.GetName(typeof(UserRole), UserRole), Enum.GetName(typeof(Sex), Gender), Email, Expires, Enum.GetName(typeof(MembershipType), MembershipType));
        }
        
    }
}