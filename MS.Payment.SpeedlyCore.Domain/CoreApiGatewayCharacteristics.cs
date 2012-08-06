using System;
using System.Xml.Serialization;

namespace MS.Payment.SpreedlyCore.Domain
{
    [Serializable]
    public class CoreApiGatewayCharacteristics
    {
        [XmlElement("supports_purchase")]
        public bool SupportsPurchase { get; set; }

        [XmlElement("supports_authorize")]
        public bool SupportsAuthorize { get; set; }

        [XmlElement("supports_capture")]
        public bool SupportsCapture { get; set; }

        [XmlElement("supports_credit")]
        public bool SupportsCredit { get; set; }

        [XmlElement("supports_void")]
        public bool SupportsVoid { get; set; }

        [XmlElement("supports_reference_purchase")]
        public bool SupportsReferencePurchase { get; set; }

        [XmlElement("supports_offsite_purchase")]
        public bool SupportsOffsitePurchase { get; set; }
    }
}