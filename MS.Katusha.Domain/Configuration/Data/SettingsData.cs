using System;
using System.Configuration;
using System.Web;
using MS.Katusha.Enumerations;

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
                if (KatushaConfigurationManager.Mode == MSKatushaMode.Windows) return "http";
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
                    if (KatushaConfigurationManager.Mode == MSKatushaMode.Windows) return "127.0.0.1";
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
                if (String.IsNullOrWhiteSpace(retVal)) retVal = (KatushaConfigurationManager.Mode == MSKatushaMode.Windows) ? 
                    ""
                    : HttpContext.Current.Server.MapPath(@"~\");
                return retVal;
            }
        }

    }
}