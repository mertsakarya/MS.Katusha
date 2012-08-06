using System;
using System.Xml.Serialization;

namespace MS.Payment.SpreedlyCore.Domain
{
    [Serializable]
    [XmlRoot("transaction")]
    public class CoreApiPaymentTransaction
    {
        [XmlElement("amount")]
        public int Amount { get; set; }

        [XmlElement("on_test_gateway")]
        public bool IsTest { get; set; }

        [XmlElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [XmlElement("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [XmlElement("currency_code")]
        public String Currency { get; set; }

        [XmlElement("succeeded")]
        public bool Succeeded { get; set; }

        [XmlElement("token")]
        public String Token { get; set; }

        [XmlElement("message")]
        public String Message { get; set; }

        [XmlElement("transaction_type")]
        public String TransactionType { get; set; }

        [XmlElement("order_id")]
        public String OrderId { get; set; }

        [XmlElement("ip")]
        public String Ip { get; set; }

        [XmlElement("description")]
        public String Description { get; set; }

        [XmlElement("gateway_token")]
        public String GatewayToken { get; set; }

        [XmlElement("response")]
        public CoreApiPaymentTransactionResponse Response { get; set; }

        [XmlElement("payment_method")]
        public CoreApiPaymentTransactionPaymentMethod PaymentMethod { get; set; }

    }
}