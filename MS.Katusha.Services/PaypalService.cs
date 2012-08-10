using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Exceptions.Services;
using MS.Katusha.Interfaces.Services;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MS.Katusha.Services
{
    public class PaypalService : IPaypalService
    {
        private readonly IUserService _userService;
        private readonly PayPalAPIInterfaceServiceService _payPalApiService;
        private PaypalSettings _settings;
        private const string MSKatushaorderDescription = "Your order for MS.Katusha";
        private const string MSKatushaBrandName = "MS.Katusha";
        private const string MSKatushaImageUrl = "http://mskatusha.apphb.com/Images/logo.jpg";
        const CurrencyCodeType MSKatushaCurrencyCode = CurrencyCodeType.USD;

        private string GetApiUserName()
        {
            var paypalPaymentTest = ConfigurationManager.AppSettings["PaypalSandbox"] == "true";
            return paypalPaymentTest ? "mertm_1344098150_biz_api1.hotmail.com" : "mertsakarya_api1.hotmail.com";
        }

        public PaypalService(IUserService userService) {
            _userService = userService;
            _payPalApiService = new PayPalAPIInterfaceServiceService();
            _settings = PaypalSettings.ParseConfiguration();
        }

        public string SetExpressCheckout(User user, Product product, string referrer = "", int quantity = 1, string billingAgreementText = "")
        {
            var request = new SetExpressCheckoutRequestType();
            PopulateSetExpressCheckoutRequestObject(request, user, product, referrer, quantity, billingAgreementText);
            var wrapper = new SetExpressCheckoutReq {SetExpressCheckoutRequest = request};
            var setEcResponse = _payPalApiService.SetExpressCheckout(wrapper, GetApiUserName());
            return setEcResponse.Token;
        }

        private void PopulateSetExpressCheckoutRequestObject(SetExpressCheckoutRequestType request, User user, Product product, string referrer,  int quantity, string billingAgreementText = "")
        {
            const string zero = "0.00";
            var orderTotal = 0.0;
            var itemTotal = 0.0;

            // Each payment can include requestDetails about multiple items
            // This example shows just one payment item
            if (quantity < 1) throw new Exception("Insufficient quantity");
            var itemDetails = new PaymentDetailsItemType {
                Name = product.Name,
                Amount = new BasicAmountType(MSKatushaCurrencyCode, product.Amount), 
                Quantity = quantity, 
                //ItemCategory = ItemCategoryType.PHYSICAL,
                Tax = new BasicAmountType(MSKatushaCurrencyCode, product.Tax), 
                Description = product.Description,
            };
            itemTotal += (Double.Parse(itemDetails.Amount.value) * quantity);

            orderTotal += Double.Parse(itemDetails.Tax.value);
            orderTotal += itemTotal;

            var paymentDetails = new PaymentDetailsType {
                ShippingTotal = new BasicAmountType(MSKatushaCurrencyCode, zero), 
                OrderDescription = MSKatushaorderDescription, 
                PaymentAction = PaymentActionCodeType.SALE,
                ItemTotal = new BasicAmountType(MSKatushaCurrencyCode, itemTotal.ToString(CultureInfo.InvariantCulture)),
                Custom = product.FriendlyName + "|" +(referrer ?? ""),
            };
            orderTotal += Double.Parse(paymentDetails.ShippingTotal.value);
            paymentDetails.OrderTotal = new BasicAmountType(MSKatushaCurrencyCode, orderTotal.ToString(CultureInfo.InvariantCulture));
            paymentDetails.PaymentDetailsItem.Add(itemDetails);


            var ecDetails = new SetExpressCheckoutRequestDetailsType {
                ReturnURL = _settings.ReturnUrl, 
                CancelURL = _settings.CancelUrl, 
                BuyerEmail = user.Email, 
                AddressOverride = "0", 
                NoShipping = "1", 
                SolutionType = SolutionTypeType.SOLE, 
                BuyerDetails = new BuyerDetailsType() {BuyerId = user.Guid.ToString(), BuyerRegistrationDate = user.CreationDate.ToString("s"), BuyerUserName = user.UserName},
                cppHeaderImage = MSKatushaImageUrl,
                BrandName = MSKatushaBrandName
                //PageStyle = pageStyle.Value,
                //cppHeaderBorderColor = cppheaderbordercolor.Value,
                //cppHeaderBackColor = cppheaderbackcolor.Value,
                //cppPayflowColor = cpppayflowcolor.Value,
            };
            ecDetails.PaymentDetails.Add(paymentDetails);

            if (!String.IsNullOrWhiteSpace(billingAgreementText)) {
                var baType = new BillingAgreementDetailsType(BillingCodeType.MERCHANTINITIATEDBILLINGSINGLEAGREEMENT) { BillingAgreementDescription = billingAgreementText };
                ecDetails.BillingAgreementDetails.Add(baType);
            }
            request.SetExpressCheckoutRequestDetails = ecDetails;

            /*
                        //if (insuranceTotal.Value != "" && !double.Parse(insuranceTotal.Value).Equals(0.0)) {
                        //    paymentDetails.InsuranceTotal = new BasicAmountType(MSKatushaCurrencyCode, zero);
                        //    paymentDetails.InsuranceOptionOffered = "true";
                        //    orderTotal += Double.Parse(insuranceTotal.Value);
                        //}
                        //if (handlingTotal.Value != "") {
                        //    paymentDetails.HandlingTotal = new BasicAmountType(MSKatushaCurrencyCode, handlingTotal.Value);
                        //    orderTotal += Double.Parse(handlingTotal.Value);
                        //}
                        //if (taxTotal.Value != "") {
                        //    paymentDetails.TaxTotal = new BasicAmountType(MSKatushaCurrencyCode, taxTotal.Value);
                        //    orderTotal += Double.Parse(taxTotal.Value);
                        //}
                        //if (shippingName.Value != "" && shippingStreet1.Value != ""
                        //    && shippingCity.Value != "" && shippingState.Value != ""
                        //    && shippingCountry.Value != "" && shippingPostalCode.Value != "") {
                        //    AddressType shipAddress = new AddressType();
                        //    shipAddress.Name = shippingName.Value;
                        //    shipAddress.Street1 = shippingStreet1.Value;
                        //    shipAddress.Street2 = shippingStreet2.Value;
                        //    shipAddress.CityName = shippingCity.Value;
                        //    shipAddress.StateOrProvince = shippingState.Value;
                        //    shipAddress.Country = (CountryCodeType)
                        //        Enum.Parse(typeof(CountryCodeType), shippingCountry.Value);
                        //    shipAddress.PostalCode = shippingPostalCode.Value;
                        //    ecDetails.PaymentDetails[0].ShipToAddress = shipAddress;
                        //}
             * */
        }

        public CheckoutDetailResult GetExpressCheckoutDetails(User user, string token)
        {
            try {
                var response = _GetExpressCheckoutDetails(token);
                CheckoutStatus status;
                if (!Enum.TryParse(response.GetExpressCheckoutDetailsResponseDetails.CheckoutStatus, true, out status))
                    return new CheckoutDetailResult() {Errors = new List<string>() {"NOCHECKOUTSTATUS"}};
                var result = new CheckoutDetailResult { CheckoutStatus = status, Errors = new List<string>(), };
                if (response.Ack.Equals(AckCodeType.FAILURE) || (response.Errors != null && response.Errors.Count > 0)) {
                    foreach (var error in response.Errors)
                        result.Errors.Add(error.LongMessage);
                } else {
                    var custom = response.GetExpressCheckoutDetailsResponseDetails.PaymentDetails[0].Custom.Split('|');
                    result.Referrer = custom[1];
                    result.ProductName = (ProductNames) Enum.Parse(typeof (ProductNames), custom[0], true);
                }
                return result;
            } catch (KatushaProductNameNotFoundException ex) {
                return new CheckoutDetailResult() { Errors = new List<string> { "PRODUCTNOTFOUND" } };
            }
        }

        private GetExpressCheckoutDetailsResponseType _GetExpressCheckoutDetails(string token)
        {
            var wrapper = new GetExpressCheckoutDetailsReq { GetExpressCheckoutDetailsRequest = new GetExpressCheckoutDetailsRequestType { Token = token } };
            var response = _payPalApiService.GetExpressCheckoutDetails(wrapper, GetApiUserName());
            var custom0 = response.GetExpressCheckoutDetailsResponseDetails.PaymentDetails[0].Custom.Split('|')[0];
            ProductNames productName;
            if (!Enum.TryParse(custom0, true, out productName))
                throw new KatushaProductNameNotFoundException(custom0);
            return response;
        }

        public CheckoutPaymentResult DoExpressCheckoutPayment(User user, string token, string payerId)
        {
            var getEcResponse = _GetExpressCheckoutDetails(token);
            CheckoutStatus status;
            if (!Enum.TryParse(getEcResponse.GetExpressCheckoutDetailsResponseDetails.CheckoutStatus, true, out status))
                return new CheckoutPaymentResult() {Errors = new List<string>() {"NOCHECKOUTSTATUS"}};
            if (status == CheckoutStatus.PaymentActionNotInitiated) {
                user.PaypalPayerId = payerId;
                var request = new DoExpressCheckoutPaymentRequestType();
                var requestDetails = new DoExpressCheckoutPaymentRequestDetailsType {Token = token, PayerID = payerId, PaymentAction = PaymentActionCodeType.SALE};
                request.DoExpressCheckoutPaymentRequestDetails = requestDetails;
                requestDetails.PaymentDetails = getEcResponse.GetExpressCheckoutDetailsResponseDetails.PaymentDetails;
                var wrapper = new DoExpressCheckoutPaymentReq {DoExpressCheckoutPaymentRequest = request};
                var doEcResponse = _payPalApiService.DoExpressCheckoutPayment(wrapper, GetApiUserName());
                var custom = getEcResponse.GetExpressCheckoutDetailsResponseDetails.PaymentDetails[0].Custom.Split('|');
                var result = new CheckoutPaymentResult() {
                    BillingAgreementId = doEcResponse.DoExpressCheckoutPaymentResponseDetails.BillingAgreementID,
                    PaymentStatus = doEcResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                    PendingReason = doEcResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PendingReason.ToString(),
                    TransactionId = doEcResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID,
                    ProductName = (ProductNames) Enum.Parse(typeof (ProductNames), custom[0], true),
                    Referrer = custom[1],
                    Errors = new List<string>(),
                };
                if (doEcResponse.Ack.Equals(AckCodeType.FAILURE) || (doEcResponse.Errors != null && doEcResponse.Errors.Count > 0)) {
                    foreach (var error in doEcResponse.Errors)
                        result.Errors.Add(error.LongMessage);
                } else {
                    _userService.Purchase(user, result.ProductName, payerId);
                }
                return result;
            } return new CheckoutPaymentResult() { Errors = new List<string>() { status.ToString() } };
        }
    }
}
