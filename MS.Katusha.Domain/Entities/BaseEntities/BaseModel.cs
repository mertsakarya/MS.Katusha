using System;

namespace MS.Katusha.Domain.Entities.BaseEntities
{
    public abstract class BaseModel : IdModel
    {
        public DateTime ModifiedDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DeletionDate { get; set; }
        public bool Deleted { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ModifiedDate: {0} | CreationsDate: {1}", ModifiedDate, CreationDate);
        }
    }
}