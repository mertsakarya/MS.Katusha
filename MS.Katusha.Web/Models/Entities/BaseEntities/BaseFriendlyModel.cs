using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Infrastructure.Attributes;

namespace MS.Katusha.Web.Models.Entities.BaseEntities
{
    public abstract class BaseFriendlyModel : BaseGuidModel //, IValidatableObject
    {

        //[Display("FriendlyName")]
        [KatushaRegularExpression("FriendlyName")]
        [KatushaField("FriendlyName")]
        public String FriendlyName { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | FriendlyName: {0}", FriendlyName);
        }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{

        //    if(false)
        //        yield return new ValidationResult("POP");

        //}

    }
}