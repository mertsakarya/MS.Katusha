using System;

namespace MS.Katusha.Web.Models.Entities.BaseEntities
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