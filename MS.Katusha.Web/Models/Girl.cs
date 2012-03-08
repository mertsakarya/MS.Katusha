using System;
using MS.Katusha.Domain.Enums;

namespace MS.Katusha.Web.Models
{
    public class Girl : Profile
    {
        public byte BreastSize { get; set; }


        public override string ToString()
        {
            return base.ToString() + String.Format(" | BreastSize: {0}", Enum.GetName(typeof(BreastSize), BreastSize));
        }
    }
}