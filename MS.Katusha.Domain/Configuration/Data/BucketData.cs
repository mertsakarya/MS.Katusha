using System.Configuration;

namespace MS.Katusha.Configuration.Data
{
    public class BucketData : ConfigurationElement
    {

        private static readonly ConfigurationProperty bucketName =
            new ConfigurationProperty("bucketName", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty accessKey =
            new ConfigurationProperty("accessKey", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty secretKey =
            new ConfigurationProperty("secretKey", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty rootUrl =
            new ConfigurationProperty("rootUrl", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);


        public BucketData()
        {
            base.Properties.Add(bucketName);
            base.Properties.Add(accessKey);
            base.Properties.Add(secretKey);
            base.Properties.Add(rootUrl);

        }

        [ConfigurationProperty("rootUrl", IsRequired = true)]
        public string RootUrl { get { return KatushaConfigurationManager.Instance.GetSettings().Protocol + this[rootUrl]; } }

        [ConfigurationProperty("bucketName", IsRequired = true)]
        public string BucketName { get { return (string)this[bucketName]; } }

        [ConfigurationProperty("accessKey", IsRequired = true)]
        public string AccessKey { get { return (string)this[accessKey]; } }

        [ConfigurationProperty("secretKey", IsRequired = true)]
        public string SecretKey { get { return (string)this[secretKey]; } }
    }
}