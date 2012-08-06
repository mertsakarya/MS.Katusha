using System.Xml.Serialization;

namespace MS.Payment.SpreedlyCore.Domain
{
    public class CoreApiPaymentErrors
    {
        [XmlAttribute("attribute")]
        public string Field { get; set; }

        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlElement("error")]
        public string Error { get; set; }
    }
}