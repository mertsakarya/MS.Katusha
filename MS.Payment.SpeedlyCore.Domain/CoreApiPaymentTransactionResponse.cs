using System;
using System.Xml.Serialization;

namespace MS.Payment.SpreedlyCore.Domain
{
    public class CoreApiPaymentTransactionResponse
    {
        [XmlElement("succeess")]
        public bool Succeeded { get; set; }

        [XmlElement("avs_code")]
        public String AvsCode { get; set; }

        [XmlElement("avs_message")]
        public String AvsMessage { get; set; }

        [XmlElement("cvv_code")]
        public String CvvCode { get; set; }

        [XmlElement("cvv_message")]
        public String CvvMessage { get; set; }

        [XmlElement("error_code")]
        public String ErrorCode { get; set; }

        [XmlElement("error_detail")]
        public String ErrorDetail { get; set; }

        [XmlElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [XmlElement("updated_at")]
        public DateTime UpdatedAt { get; set; }

    }
}