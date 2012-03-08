using System;
using System.ComponentModel.DataAnnotations;

namespace MS.Katusha.Web.Models.BaseModels
{
    public abstract class BaseGuidModel : BaseModel
    {
        public Guid Guid { get; set; }
        
        public override string ToString()
        {
            return base.ToString() + String.Format(" | Guid: {0}", Guid);
        }
    }
}