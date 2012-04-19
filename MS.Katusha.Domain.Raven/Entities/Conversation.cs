using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class Conversation : BaseGuidModel
    {
        public long FromId { get; set; }
        public long ToId { get; set; }

        public string FromName { get; set; }
        public string ToName { get; set; }

        public Guid FromPhotoGuid { get; set; }
        public Guid ToPhotoGuid { get; set; }

        public Guid FromGuid { get; set; }
        public Guid ToGuid { get; set; }

        [Required]
        [MinLength(2), MaxLength(4000)]
        public string Message { get; set; }

        [MaxLength(255)]
        public string Subject { get; set; }

        public DateTimeOffset ReadDate { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | From: {0} | To: {1} | ReadDate: {2} | Subject: {3}\r\nMESSAGE: [\r\n\r\n{4}\r\n\r\n]", FromId, ToId, ReadDate, Subject, Message);
        }
    }
}