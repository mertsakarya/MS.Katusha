using System;

namespace MS.Katusha.Domain.Entities.BaseEntities
{
    public abstract class BaseModel
    {
        public long Id { get; set; }

        public DateTime ModifiedDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DeletionDate { get; set; }
        public bool Deleted { get; set; }


        public override string ToString()
        {
            return String.Format("Id: {0} | ModifiedDate: {1} | CreationsDate: {2}", Id, ModifiedDate, CreationDate);
        }

    }
}