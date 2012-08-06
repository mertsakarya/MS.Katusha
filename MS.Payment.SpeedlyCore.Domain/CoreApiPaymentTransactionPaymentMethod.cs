using System;
using System.Xml.Serialization;

namespace MS.Payment.SpreedlyCore.Domain
{
    public class CoreApiPaymentTransactionPaymentMethod
    {
        public CoreApiPaymentTransactionPaymentMethod() { }

        [XmlElement("token")]
        public string Token { get; set; }

        [XmlElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [XmlElement("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [XmlElement("last_four_digits")]
        public String LastFourDigits { get; set; }

        [XmlElement("card_type")]
        public String CardType { get; set; }

        [XmlElement("first_name")]
        public String FirstName { get; set; }

        [XmlElement("last_name")]
        public String LastName { get; set; }

        [XmlElement("month")]
        public int Month { get; set; }

        [XmlElement("year")]
        public int Year { get; set; }

        [XmlElement("email")]
        public String Email { get; set; }

        [XmlElement("address1")]
        public String Address1 { get; set; }

        [XmlElement("address2")]
        public String Address2 { get; set; }

        [XmlElement("city")]
        public String City { get; set; }

        [XmlElement("state")]
        public String State { get; set; }

        [XmlElement("zip")]
        public String Zip { get; set; }

        [XmlElement("country")]
        public String Country { get; set; }

        [XmlElement("phone_number")]
        public String PhoneNumber { get; set; }

        [XmlElement("data")]
        public String Data { get; set; }

        [XmlElement("payment_method_type")]
        public String PaymentMethodType { get; set; }

        [XmlElement("verification_value")]
        public String VerificationValue { get; set; }

        [XmlElement("number")]
        public String Number { get; set; }

        [XmlArrayItem("errors")]
        public CoreApiPaymentErrors[] Errors { get; set; }
    }
}