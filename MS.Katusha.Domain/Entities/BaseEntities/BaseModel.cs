using System;
using System.ComponentModel.DataAnnotations;

namespace MS.Katusha.Domain.Entities.BaseEntities
{
    public abstract class BaseModel
    {
        [Key]
        public long Id { get; set; }

        [Timestamp]
        public byte[] ts { get; set; }

        public DateTime ModifiedDate { get; set; }
        public DateTime CreationDate { get; set; }


        public override string ToString()
        {
            return String.Format("Id: {0} | ts: {1} | ModifiedDate: {2} | CreationsDate: {3}", Id, ts, ModifiedDate, CreationDate);
        }

    }
}