using System.Configuration;
using MS.Katusha.Configuration.Data;

namespace MS.Katusha.Configuration
{
    public class KatushaConfigurationHandler : ConfigurationSection
    {

        private static readonly ConfigurationProperty encryptionElement = new ConfigurationProperty("encryption", typeof (EncryptionData), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty bucketsElement = new ConfigurationProperty("s3Buckets", typeof(BucketDataCollection), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty settingsElement = new ConfigurationProperty("settings", typeof(SettingsData), null, ConfigurationPropertyOptions.IsRequired);

        [ConfigurationProperty("encryption", IsRequired = true)]
        public EncryptionData Encryption { get { return (EncryptionData)this[encryptionElement]; } }

        [ConfigurationProperty("s3Buckets", IsRequired = true)]
        public BucketDataCollection Buckets { get { return (BucketDataCollection)this[bucketsElement]; } }

        [ConfigurationProperty("settings", IsRequired = true)]
        public SettingsData Settings { get { return (SettingsData)this[settingsElement]; } }

    }
}