using System;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Models
{
    public class NewVisitModel
    {
        public ProfileModel Profile { get; set; }
        public ProfileModel VisitorProfile { get; set; }
        public int Count { get; set; }
        public DateTime LastVisitTime { get; set; }
    }
}