using System;

namespace MS.Katusha.Domain.Service
{
    public class ApiProfileInfo
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Guid ProfilePhotoGuid { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
    }
}