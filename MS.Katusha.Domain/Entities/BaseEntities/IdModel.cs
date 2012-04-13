using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MS.Katusha.Domain.Entities.BaseEntities
{
    public class IdModel
    {
        public long Id { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0}", Id);
        }

    }
}
