using System.Collections.Generic;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Entities
{
    public class CheckoutPaymentResult
    {
        public IList<string> Errors { get; set; }
        public string TransactionId { get; set; }
        public string BillingAgreementId { get; set; }
        public string PaymentStatus { get; set; }
        public string PendingReason { get; set; }
        public string Referrer { get; set; }
        public ProductNames ProductName { get; set; }
    }
}
