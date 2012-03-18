using System;
using MS.Katusha.Attributes;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Web.Models.Entities
{
    public class GirlModel : ProfileModel
    {
        [KatushaField("Girl.BreastSize")]
        public BreastSize? BreastSize { get; set; }


        public override string ToString()
        {
            return base.ToString() + String.Format(" | BreastSize: {0}", Enum.GetName(typeof(BreastSize), BreastSize));
        }
    }
}