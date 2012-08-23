using System;

namespace MS.Katusha.Web.Models
{
    public class ApiProfileInfo
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Guid ProfilePhotoGuid { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}