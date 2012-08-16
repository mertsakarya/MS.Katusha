using System;

namespace MS.Katusha.Interfaces.Services.Models
{
    public class ApiPhotoBackup
    {
        public Guid Guid { get; set; }
        public byte[] Data { get; set; }
    }
}
