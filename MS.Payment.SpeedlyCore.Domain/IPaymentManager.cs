namespace MS.Payment.SpreedlyCore.Domain
{
    public interface IPaymentManager
    {
        CoreApiGateway Gateway(string name);
        CoreApiPaymentTransaction Purchase(CoreApiGateway gateway, string amount, string paymentMethodToken, string currency = "USD", string orderId = "", string description = "", string ip = "");

        string Authorize(CoreApiGateway gateway, string amount, string paymentMethodToken, string currency = "USD", string orderId = "", string description = "", string ip = "");
        string Capture(CoreApiGateway gateway, string authorizeToken, string amount, string orderId = "", string description = "", string ip = "");
        string Credit(CoreApiGateway gateway, string authorizeToken, string orderId = "", string description = "", string ip = "");
        string Void(CoreApiGateway gateway, string authorizeToken, string orderId = "", string description = "", string ip = "");
        string PostForm(string firstName, string lastName, string cardNumber, string cvv, string month, string year);
    }
}