using System;
using System.ComponentModel.DataAnnotations;

namespace MS.Katusha.Domain.Entities.BaseEntities
{
    public abstract class BaseFriendlyModel : BaseGuidModel
    {
        [StringLength(64)]
        public String FriendlyName { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | FriendlyName: {0}", FriendlyName);
        }
    }
}