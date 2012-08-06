using System;
using System.Xml.Serialization;

namespace MS.Payment.SpreedlyCore.Domain
{
    [Serializable]
    [XmlRoot("gateway")]
    public class CoreApiGateway
    {
        [XmlElement("token", typeof (string))]
        public string Token { get; set; }

        [XmlElement("gateway_type", typeof (string))]
        public string Type { get; set; }

        [XmlElement("characteristics", typeof (CoreApiGatewayCharacteristics))]
        public CoreApiGatewayCharacteristics Characteristics { get; set; }

        [XmlElement("redacted", typeof (bool))]
        public bool Redacted { get; set; }

        [XmlElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [XmlElement("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}