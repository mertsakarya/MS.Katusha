using System.Configuration;

namespace MS.Katusha.Services.Configuration.Data
{
    [ConfigurationCollection(typeof(BucketData), AddItemName = "bucket", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class BucketDataCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() { return new BucketData(); }
        protected override object GetElementKey(ConfigurationElement element) { return ((BucketData)element).BucketName; }

        public BucketData Bucket(int index) { return (BucketData)BaseGet(index); }

        public BucketData Bucket(string value) { return (BucketData)BaseGet(value); }

        public new BucketData this[string name] { get { return (BucketData)BaseGet(name); } }
        public BucketData this[int index] { get { return (BucketData)BaseGet(index); } }
    }
}