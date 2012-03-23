using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class Conversation : BaseGuidModel
    {
        public long FromId { get; set; }
        public long ToId { get; set; }

        public Profile From { get; set; }
        public Profile To { get; set; }

        [Required]
        [MinLength(2), MaxLength(8000)]
        public string Message { get; set; }

        [MaxLength(255)]
        public string Subject { get; set; }

        public DateTime ReadDate { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | From: {0} | To: {1} | ReadDate: {2} | Subject: {3}\r\nMESSAGE: [\r\n\r\n{4}\r\n\r\n]", FromId, ToId, ReadDate, Subject, Message);
        }

    }
}