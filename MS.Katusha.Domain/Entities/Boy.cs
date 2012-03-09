using System;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Entities
{
    public class Boy : Profile
    {
        public byte DickSize { get; set; }
        public byte DickThickness { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | DickSize: {0} | DickThickness: {1}", Enum.GetName(typeof(DickSize), DickSize), Enum.GetName(typeof(DickThickness), DickThickness));
        }

    }
}