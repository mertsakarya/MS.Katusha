using System;
using System.Collections.Generic;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Entities
{
    public class MailModel
    {
        public MailModel()
        {
            To = new List<string>();
            Descriptions = new List<string>();
            Data = new List<object>(); 
        }

        public string Subject { get; set; }
        public IList<string> To { get; set; }
        public IList<object> Data { get; set; }
        public IList<string> Descriptions { get; set; }
        public MailType MailType { get; set; }
        public Guid Guid { get; set; }
    }
}
