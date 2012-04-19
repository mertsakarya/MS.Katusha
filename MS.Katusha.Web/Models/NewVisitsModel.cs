using System;
using System.Collections.Generic;

namespace MS.Katusha.Web.Models
{
    public class NewVisitsModel
    {
        public IList<NewVisitModel> Visits { get; set; }
        public DateTimeOffset LastVisitTime { get; set; }
    }
}