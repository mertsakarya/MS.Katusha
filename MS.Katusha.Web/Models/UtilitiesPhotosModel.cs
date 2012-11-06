using System;
using System.Collections.Generic;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Models
{
    public class UtilitiesPhotosModel
    {
        public PagedListModel<Guid> PhotoGuids { get; set; }
        public IDictionary<Guid, PhotoModel> Photos { get; set; }
        public IDictionary<Guid, ProfileModel> Profiles { get; set; }
    }
}