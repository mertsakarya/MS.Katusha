using System;
using System.Configuration;
using MS.Katusha.Enumerations;
using MS.Katusha.Services.Configuration;

namespace MS.Katusha.Domain.Entities
{
    public class PaypalSettings
    {
        public static PaypalSettings ParseConfiguration()
        {
            var sandbox = ConfigurationManager.AppSettings["PaypalSandbox"] == "true";
            var virtualPath = KatushaConfigurationManager.Instance.VirtualPath;
            var sandboxText = (sandbox) ? "Sandbox" : "";

            var settings = new PaypalSettings() {
                Username = ConfigurationManager.AppSettings[String.Format("Paypal{0}APIUsername", sandboxText)],
                Password = ConfigurationManager.AppSettings[String.Format("Paypal{0}APIPassword", sandboxText)],
                Signature = ConfigurationManager.AppSettings[String.Format("Paypal{0}Signature", sandboxText)],
                Environment = (sandbox) ? PaypalEnvironment.Sandbox : PaypalEnvironment.Live,
                CancelUrl = virtualPath + ConfigurationManager.AppSettings["PaypalCancelUrl"],
                ReturnUrl = virtualPath + ConfigurationManager.AppSettings["PaypalReturnUrl"],
                NotificationUrl = virtualPath + ConfigurationManager.AppSettings["PaypalNotificationUrl"],
            };
            return settings;
        }

        private PaypalSettings()
        {
            Version = "92.0";
            Language = "en-US";
            CurrencyCode = "USD";
        }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Signature { get; set; }
        public PaypalEnvironment Environment { get; set; }
        public string Language { get; set; }
        public string CancelUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string NotificationUrl { get; set; }
        public string CurrencyCode { get; set; }
        public string Version { get; set; }
    }
}