using System;
using System.ComponentModel.DataAnnotations;

namespace MS.Katusha.Web.Models.Entities.BaseEntities
{
    public abstract class BaseModel
    {
        [Key]
        public long Id { get; set; }

 
        public DateTimeOffset ModifiedDate { get; set; }
        public DateTimeOffset CreationDate { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0} | ModifiedDate: {1} | CreationsDate: {2}", Id, ModifiedDate, CreationDate);
        }

    }
}