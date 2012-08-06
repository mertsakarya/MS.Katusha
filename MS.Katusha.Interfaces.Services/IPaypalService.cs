using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IPaypalService
    {
        string SetExpressCheckout(User user, Product product, string referrer = "",  int quantity = 1, string billingAgreementText = "");
        CheckoutDetailResult GetExpressCheckoutDetails(User user, string token);
        CheckoutPaymentResult DoExpressCheckoutPayment(User user, string token, string payerId);
    }
}