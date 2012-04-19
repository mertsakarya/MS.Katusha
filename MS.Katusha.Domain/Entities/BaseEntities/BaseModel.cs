using System;

namespace MS.Katusha.Domain.Entities.BaseEntities
{
    public abstract class BaseModel : IdModel
    {
        public DateTimeOffset ModifiedDate { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset DeletionDate { get; set; }
        public bool Deleted { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ModifiedDate: {0} | CreationsDate: {1}", ModifiedDate, CreationDate);
        }
    }
}