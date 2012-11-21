using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class Photo : BaseGuidModel
    {
        public long ProfileId { get; set; }

        public string Description { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public byte Status { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Description: [\r\n{1}\r\n]", ProfileId, Description);
        }
    }
}