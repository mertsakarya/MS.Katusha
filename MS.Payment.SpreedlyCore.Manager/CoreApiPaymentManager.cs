using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using MS.Payment.SpreedlyCore.Domain;

namespace MS.Payment.SpreedlyCore.Manager
{
    public class CoreApiPaymentManager : IPaymentManager
    {
        public const string PostFormUrl = "https://spreedlycore.com/v1/payment_methods";
        public const string PostFormRedirectUrl = "http://localhost/Payments/Complete";

        private const string OrderId = "<order_id>{0}<order_id>";
        private const string Description = "<description>{0}<description>";
        private const string Ip = "<ip>{0}<ip>";
        private string SinceToken = "?since_token={0}";

        private IDictionary<string, CoreApiRequestInfo> CoreAPIRequestInfos = new Dictionary<string, CoreApiRequestInfo>() {
            {"gateway", new CoreApiRequestInfo() {Method = HttpMethod.POST, Url = "https://spreedlycore.com/v1/gateways.xml", XmlText = "<gateway><gateway_type>{0}</gateway_type></gateway>"}},
            {"purchase", new CoreApiRequestInfo() {Method = HttpMethod.POST, Url = "https://spreedlycore.com/v1/gateways/{0}/purchase.xml", XmlText = "<transaction><amount>{0}</amount><currency_code>{1}</currency_code><payment_method_token>{2}</payment_method_token>{3}{4}{5}</transaction>"}},
            {"authorize", new CoreApiRequestInfo() {Method = HttpMethod.POST, Url = "https://spreedlycore.com/v1/gateways/{0}/authorize.xml", XmlText = "<transaction><payment_method_token>{0}</payment_method_token><amount>{1}</amount><currency_code>{2}</currency_code>{3}{4}{5}</transaction>"}},
            {"capture", new CoreApiRequestInfo() {Method = HttpMethod.POST, Url = "https://spreedlycore.com/v1/transactions/{0}/capture.xml", XmlText = "<transaction><amount>{0}</amount>{1}{2}{3}</transaction>"}},
            {"credit", new CoreApiRequestInfo() {Method = HttpMethod.POST, Url = "https://spreedlycore.com/v1/transactions/{0}/credit.xml", XmlText = "<transaction>{0}{1}{2}</transaction>"}},
            {"void", new CoreApiRequestInfo() {Method = HttpMethod.POST, Url = "https://spreedlycore.com/v1/transactions/{0}/void.xml", XmlText = "<transaction>{0}{1}{2}</transaction>"}},
            {"referencePurchase", new CoreApiRequestInfo() {Method = HttpMethod.POST, Url = "https://spreedlycore.com/v1/transactions/{0}/purchase.xml", XmlText = "<transaction><amount>{0}</amount></transaction>"}},
            {"offsitePurchase", new CoreApiRequestInfo() {Method = HttpMethod.POST, Url = "", XmlText = ""}},

            {"retain", new CoreApiRequestInfo() {Method = HttpMethod.PUT, Url = "https://spreedlycore.com/v1/payment_methods/{0}/retain.xml", XmlText = ""}},
            {"redact", new CoreApiRequestInfo() {Method = HttpMethod.PUT, Url = "  https://spreedlycore.com/v1/payment_methods/{0}/redact.xml", XmlText = ""}},
            {"paymentMethods", new CoreApiRequestInfo() {Method = HttpMethod.GET, Url = "https://spreedlycore.com/v1/payment_methods.xml{0}", XmlText = ""}},
        };

        private CoreApiCredentials Credentials = new CoreApiCredentials() {ApiLogin = ConfigurationManager.AppSettings["SPREEDLYCORE_API_LOGIN"], ApiSecret = ConfigurationManager.AppSettings["SPREEDLYCORE_API_SECRET"]};

        private Uri Post(string uri, NameValueCollection pairs)
        {
            byte[] response = null;
            using (CoreApiWebClient client = new CoreApiWebClient()) {
                try {
                    response = client.UploadValues(uri, pairs);
                } catch {
                    return client.ResponseUri;
                }
                return client.ResponseUri;
            }
            
        }

        private Stream MakeRequest(string coreApiRequestInfoName, string token, params object[] parameters)
        {
            CoreApiCredentials credentials = Credentials;
            CoreApiRequestInfo info = CoreAPIRequestInfos[coreApiRequestInfoName];
            var request = WebRequest.Create(String.Format(info.Url, token));
            var authInfo = credentials.ApiLogin + ":" + credentials.ApiSecret;
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
            request.ContentType = "application/xml";
            request.Method = info.Method.ToString();
            Debug.WriteLine(request.RequestUri.ToString());
            Debug.WriteLine(string.Format(info.XmlText, parameters));
            if (parameters.Length > 0) {
                using (var stream = request.GetRequestStream()) {
                    using (var writer = new StreamWriter(stream, Encoding.UTF8)) {
                        writer.Write(info.XmlText, parameters);
                    }
                }
            }
            try {
                var response = request.GetResponse();
                return response.GetResponseStream();
            } catch(WebException ex) {
                return ex.Response.GetResponseStream();
            }
        }

        private T GetDocument<T>(string action, string token, string amount, string paymentMethodToken, string currency, string orderId, string description, string ip) where T : class
        {
            using (var stream = MakeRequest(action, token, amount, currency, paymentMethodToken,
                                            (String.IsNullOrWhiteSpace(orderId)) ? "" : String.Format(OrderId, orderId),
                                            (String.IsNullOrWhiteSpace(description)) ? "" : String.Format(Description, description),
                                            (String.IsNullOrWhiteSpace(ip)) ? "" : String.Format(Ip, ip)
                )) {
                var serializer = new XmlSerializer(typeof (T));
                return serializer.Deserialize(stream) as T;
            }

        }


        public CoreApiGateway Gateway(string name)
        {
            using (var stream = MakeRequest("gateway", "", name)) {
                var serializer = new XmlSerializer(typeof (CoreApiGateway));
                return serializer.Deserialize(stream) as CoreApiGateway;
            }
        }

        public CoreApiPaymentTransaction Purchase(CoreApiGateway gateway, string amount, string paymentMethodToken, string currency = "USD", string orderId = "", string description = "", string ip = "")
        {
            if (!gateway.Characteristics.SupportsPurchase) throw new Exception("Purchase not supported - " + gateway.Type);
            return GetDocument<CoreApiPaymentTransaction>("purchase", gateway.Token, amount, paymentMethodToken, currency, orderId, description, ip);
        }

        public string Authorize(CoreApiGateway gateway, string amount, string paymentMethodToken, string currency = "USD", string orderId = "", string description = "", string ip = "")
        {
            //"<transaction><payment_method_token>{0}</payment_method_token><amount>{1}</amount><currency_code>{2}</currency_code>{3}{4}{5}</transaction>"}},
            if (!gateway.Characteristics.SupportsAuthorize) throw new Exception("Authorize not supported - " + gateway.Type);
            //var doc = GetXmlDocument("action", gateway.Token, paymentMethodToken, amount, currency, orderId, description, ip);
            //if (doc != null) {
            //    var succeedNode = doc.SelectSingleNode("/transaction/succeded");
            //    var succeded = succeedNode != null && succeedNode.InnerText == "true";
            //    if (succeded) {
            //        var tokenNode = doc.SelectSingleNode("/transaction/token");
            //        if (tokenNode != null) {
            //            var resultToken = tokenNode.InnerText;
            //            return resultToken;
            //        }
            //    }
            //}
            return null;
        }

        public string Capture(CoreApiGateway gateway, string authorizeToken, string amount, string orderId = "", string description = "", string ip = "")
        {
            //"<transaction><amount>{0}</amount>{1}{2}{3}</transaction>"}},
            if (!gateway.Characteristics.SupportsCapture) throw new Exception("Capture not supported - " + gateway.Type);
            //var doc = GetXmlDocument("capture", authorizeToken, amount, "", "", orderId, description, ip);
            //if (doc != null) {
            //    var succeedNode = doc.SelectSingleNode("/transaction/succeded");
            //    var succeded = succeedNode != null && succeedNode.InnerText == "true";
            //    if (succeded) {
            //        var tokenNode = doc.SelectSingleNode("/transaction/token");
            //        if (tokenNode != null) {
            //            var resultToken = tokenNode.InnerText;
            //            return resultToken;
            //        }
            //    }
            //}
            return null;
        }

        public string Credit(CoreApiGateway gateway, string authorizeToken, string orderId = "", string description = "", string ip = "")
        {
            //"<transaction>{0}{1}{2}</transaction>"}},
            if (!gateway.Characteristics.SupportsCredit) throw new Exception("Credit not supported - " + gateway.Type);
            //var doc = GetXmlDocument("credit", authorizeToken, "", "", "", orderId, description, ip);
            //if (doc != null) {
            //    var succeedNode = doc.SelectSingleNode("/transaction/succeded");
            //    var succeded = succeedNode != null && succeedNode.InnerText == "true";
            //    if (succeded) {
            //        var tokenNode = doc.SelectSingleNode("/transaction/token");
            //        if (tokenNode != null) {
            //            var resultToken = tokenNode.InnerText;
            //            return resultToken;
            //        }
            //    }
            //}
            return null;
        }


        public string Void(CoreApiGateway gateway, string authorizeToken, string orderId = "", string description = "", string ip = "")
        {
            //"<transaction>{0}{1}{2}</transaction>"}},
            if (!gateway.Characteristics.SupportsVoid) throw new Exception("Void not supported - " + gateway.Type);
            //var doc = GetXmlDocument("void", authorizeToken, "", "", "", orderId, description, ip);
            //if (doc != null) {
            //    var succeedNode = doc.SelectSingleNode("/transaction/succeded");
            //    var succeded = succeedNode != null && succeedNode.InnerText == "true";
            //    if (succeded) {
            //        var tokenNode = doc.SelectSingleNode("/transaction/token");
            //        if (tokenNode != null) {
            //            var resultToken = tokenNode.InnerText;
            //            return resultToken;
            //        }
            //    }
            //}
            return null;
        }

        public string PostForm(string firstName, string lastName, string cardNumber, string cvv, string month, string year)
        {
            var result = Post(PostFormUrl, new NameValueCollection() {
                {"redirect_url", PostFormRedirectUrl},
                {"api_login", Credentials.ApiLogin},
                {"credit_card[first_name]", firstName},
                {"credit_card[last_name]", lastName},
                {"credit_card[number]", cardNumber},
                {"credit_card[verification_value]", cvv},
                {"credit_card[month]", month},
                {"credit_card[year]", year}
            }
                );
            var arr = result.Query.Split('=');
            if (arr.Length < 2) return ""; 
            return arr[0].IndexOf("token", System.StringComparison.Ordinal) >= 0 ? arr[1] : "";
        }
    }


    /*<form action="https://spreedlycore.com/v1/payment_methods" method="POST">
    <fieldset>
        <input name="redirect_url" type="hidden" value="http://example.com/transparent_redirect_complete" />
        <input name="api_login" type="hidden" value="Ll6fAtoVSTyVMlJEmtpoJV8Shw5" />
        <label for="credit_card_first_name">First name</label>
        <input id="credit_card_first_name" name="credit_card[first_name]" type="text" />

        <label for="credit_card_last_name">Last name</label>
        <input id="credit_card_last_name" name="credit_card[last_name]" type="text" />

        <label for="credit_card_number">Card Number</label>
        <input id="credit_card_number" name="credit_card[number]" type="text" />

        <label for="credit_card_verification_value">Security Code</label>
        <input id="credit_card_verification_value" name="credit_card[verification_value]" type="text" />

        <label for="credit_card_month">Expires on</label>
        <input id="credit_card_month" name="credit_card[month]" type="text" />
        <input id="credit_card_year" name="credit_card[year]" type="text" />

        <button type='submit'>Submit Payment</button>
    </fieldset>
</form>*/
}
