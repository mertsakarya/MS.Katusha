using System;
using System.ComponentModel.DataAnnotations;

namespace MS.Katusha.Domain.Entities.BaseEntities
{
    public abstract class BaseGuidModel : BaseModel
    {
        [Required]
        public Guid Guid { get; set; }
        
        public override string ToString()
        {
            return base.ToString() + String.Format(" | Guid: {0}", Guid);
        }
    }
}