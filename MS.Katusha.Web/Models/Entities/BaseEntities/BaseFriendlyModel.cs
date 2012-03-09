using System;

namespace MS.Katusha.Web.Models.Entities.BaseEntities
{
    public abstract class BaseFriendlyModel : BaseGuidModel
    {
        public String FriendlyName { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | FriendlyName: {0}", FriendlyName);
        }
    }
}