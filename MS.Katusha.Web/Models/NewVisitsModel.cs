using System;
using System.Collections.Generic;

namespace MS.Katusha.Web.Models
{
    public class NewVisitsModel
    {
        public IList<NewVisitModel> Visits { get; set; }
        public DateTime LastVisitTime { get; set; }
    }
}