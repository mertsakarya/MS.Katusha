using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Web.Models.Entities
{
    public class UserModel : BaseGuidModel
    {
        public Sex Gender { get; set; }

        [Required]
        [MinLength(3), MaxLength(64)]
        public string UserName { get; set; }

        [Required]
        [MinLength(7), MaxLength(64)]
        public string Email { get; set; }

        public bool EmailValidated { get; set; }

        public string Phone { get; set; }

        [Required]
        [MinLength(6), MaxLength(14)]
        public string Password { get; set; }

        public DateTime Expires { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | Gender: {0} | UserName: {1} | Email: {2} | EmailValidated: {6} | Phone: {3} | Password: {4} | Expires: {5}", Enum.GetName(typeof(Sex), Gender), UserName, Email, Phone, Password, Expires, EmailValidated);
        }

    }
}