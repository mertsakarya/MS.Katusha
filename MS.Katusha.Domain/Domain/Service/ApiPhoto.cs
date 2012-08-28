using System;

namespace MS.Katusha.Domain.Service
{
    public class ApiPhoto
    {
        public Guid Guid { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte Status { get; set; }
    }
}