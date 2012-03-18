using System;
using MS.Katusha.Attributes;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Web.Models.Entities
{
    public class BoyModel : ProfileModel
    {
        [KatushaField("Boy.DickSize")]
        public DickSize? DickSize { get; set; }

        [KatushaField("Boy.DickThickness")]
        public DickThickness? DickThickness { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | DickSize: {0} | DickThickness: {1}", Enum.GetName(typeof(DickSize), DickSize), Enum.GetName(typeof(DickThickness), DickThickness));
        }

    }
}