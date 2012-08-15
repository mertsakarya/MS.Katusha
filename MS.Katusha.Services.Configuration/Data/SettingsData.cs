using System.Configuration;

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
                return ConfigurationManager.AppSettings["Protocol"];
                // (string)this[protocol];
            }
        }
    }
}