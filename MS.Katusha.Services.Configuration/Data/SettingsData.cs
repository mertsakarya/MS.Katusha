using System.Configuration;

namespace MS.Katusha.Services.Configuration.Data
{
    public class SettingsData : ConfigurationElement
    {
        public SettingsData()
        {
            base.Properties.Add(https);
        }

        private static readonly ConfigurationProperty https = new ConfigurationProperty("https", typeof(bool), false, ConfigurationPropertyOptions.IsRequired);

        [ConfigurationProperty("https", IsRequired = true)]
        public string Https { get { return (string)this[https]; } }
    }
}