using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class DialogResult
    {
        public long FromId { get; set; }
        public System.Guid FromGuid { get; set; }
        public long ToId { get; set; }
        public System.Guid ToGuid { get; set; }
        public System.DateTime LastConversationDate { get; set; }
        public int Count { get; set; }
        public int UnreadCount { get; set; }
    }

}
