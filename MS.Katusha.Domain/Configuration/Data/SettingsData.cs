using System;
using System.Configuration;
using System.Web;

namespace MS.Katusha.Configuration.Data
{
    public class SettingsData : ConfigurationElement
    {

        public SettingsData() { base.Properties.Add(protocol); }

        private static readonly ConfigurationProperty protocol = new ConfigurationProperty("protocol", typeof (string), "http", ConfigurationPropertyOptions.IsRequired);

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

        public string Ip
        {
            get
            {
                {
                    var value = HttpContext.Current.Request.Headers["X-Forwarded-For"];
                    return string.IsNullOrEmpty(value) ? HttpContext.Current.Request.UserHostAddress : value;
                }
            }
        }

        public string NotTrackedIpsByGoogleAnalytics { get { return ConfigurationManager.AppSettings["NotTrackedIpsByGoogleAnalytics"]; } }
        public string AdministratorMailAddress { get { return ConfigurationManager.AppSettings["AdministratorMailAddress"]; } }

        public string MailViewFolder
        {
            get
            {
                var retVal = ConfigurationManager.AppSettings["MailViewFolder"];
                if (String.IsNullOrWhiteSpace(retVal)) retVal = HttpContext.Current.Server.MapPath(@"~\");
                return retVal;
            }
        }

    }
}