using System;
using System.Configuration;
using System.Web;

namespace MS.Katusha.Services.Configuration.Data
{
    public class SettingsData : ConfigurationElement
    {
        public SettingsData()
        {
            base.Properties.Add(protocol);
        }

        private static readonly ConfigurationProperty protocol = new ConfigurationProperty("protocol", typeof(string), "http", ConfigurationPropertyOptions.IsRequired);

        [ConfigurationProperty("protocol", IsRequired = true)]
        public string Protocol
        {
            get
            {
                //TODO: THIS IS BAAADDDD!
                if (HttpContext.Current.Request.Url.Scheme == "https") return "https";
                var requestProtocol = HttpContext.Current.Request.Headers["X-Forwarded-Proto"];
                requestProtocol = String.IsNullOrEmpty(requestProtocol) ? ((HttpContext.Current.Request.ServerVariables["HTTPS"].ToLowerInvariant() == "on") ? "https" : "http") : requestProtocol.ToLowerInvariant();
                return requestProtocol;
                //return ConfigurationManager.AppSettings["Protocol"];
                // (string)this[protocol];
            }
        }
    }
}