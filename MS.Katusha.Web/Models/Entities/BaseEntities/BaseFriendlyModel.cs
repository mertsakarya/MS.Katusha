using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Attributes;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Web.Models.Entities.BaseEntities
{
    public abstract class BaseFriendlyModel : BaseGuidModel
    {

        //[RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Date is not valid must be like (dd/mm/jjjj)")]
        [DisplayName("FriendlyNameDisplayName")]
        [KatushaRegularExpression("FriendlyNameRegularExpression", Language = Language.English, ErrorMessageResourceName = "FriendlyNameRegularExpressionError")]
        public String FriendlyName { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | FriendlyName: {0}", FriendlyName);
        }
    }
}