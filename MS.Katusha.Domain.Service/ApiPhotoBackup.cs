using System;

namespace MS.Katusha.Domain.Service
{
    public class ApiPhotoBackup
    {
        public Guid Guid { get; set; }
        public byte[] Data { get; set; }
    }
}
