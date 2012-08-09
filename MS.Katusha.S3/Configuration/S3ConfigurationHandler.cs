using System;
using System.Configuration;

namespace MS.Katusha.S3.Configuration
{
    public class S3ConfigurationHandler : ConfigurationSection
    {

        private static readonly ConfigurationProperty bucketsElement =
            new ConfigurationProperty("buckets", typeof (BucketCollection), null, ConfigurationPropertyOptions.IsRequired);

        /// <summary>
        /// Accounts Collection
        /// </summary>
        [ConfigurationProperty("buckets", IsRequired = true)]
        public BucketCollection Buckets { get { return (BucketCollection)this[bucketsElement]; } }
    }

    [ConfigurationCollection(typeof (Bucket), AddItemName = "bucket",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class BucketCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() { return new Bucket(); }
        protected override object GetElementKey(ConfigurationElement element) { return ((Bucket) element).BucketName; }


        public Bucket Bucket(int index) { return (Bucket) BaseGet(index); }

        public Bucket Bucket(String value) { return (Bucket) BaseGet(value); }

        public new Bucket this[string name] { get { return (Bucket) BaseGet(name); } }
        public Bucket this[int index] { get { return (Bucket) BaseGet(index); } }
    }

    /// <summary>
    /// Class holds the <Bucket> element
    /// </summary>
    public class Bucket : ConfigurationElement
    {

        private static readonly ConfigurationProperty bucketName =
            new ConfigurationProperty("bucketName", typeof (string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty accessKey =
            new ConfigurationProperty("accessKey", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty secretKey =
            new ConfigurationProperty("secretKey", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty rootUrl =
            new ConfigurationProperty("rootUrl", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);


        public Bucket()
        {
            base.Properties.Add(bucketName);
            base.Properties.Add(accessKey);
            base.Properties.Add(secretKey);
            base.Properties.Add(rootUrl);
            
        }

        [ConfigurationProperty("rootUrl", IsRequired = true)]
        public string RootUrl { get { return (string)this[rootUrl]; } }

        [ConfigurationProperty("bucketName", IsRequired = true)]
        public string BucketName { get { return (string)this[bucketName]; } }

        [ConfigurationProperty("accessKey", IsRequired = true)]
        public string AccessKey { get { return (string)this[accessKey]; } }

        [ConfigurationProperty("secretKey", IsRequired = true)]
        public string SecretKey { get { return (string)this[secretKey]; } }
    }
}
