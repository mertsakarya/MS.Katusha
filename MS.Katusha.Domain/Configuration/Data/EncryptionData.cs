using System.Configuration;

namespace MS.Katusha.Configuration.Data
{
    public class EncryptionData : ConfigurationElement
    {
        private const string _passPhrase = "K@teryna"; // can be any string
        private const string _saltValue = "Bor02na"; // can be any string
        private const string _hashAlgorithm = "SHA1"; // can be "MD5"
        private const int _passwordIterations = 3; // can be any number
        private const string _initVector = "M3rtS@kAry@Miray"; // must be 16 bytes
        private const int _keySize = 128; // can be 192 or 128

        private static readonly ConfigurationProperty passPhrase = new ConfigurationProperty("passPhrase", typeof (string), _passPhrase, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty saltValue = new ConfigurationProperty("saltValue", typeof (string), _saltValue, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty hashAlgorithm = new ConfigurationProperty("hashAlgorithm", typeof (string), _hashAlgorithm, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty passwordIterations = new ConfigurationProperty("passwordIterations", typeof (int), _passwordIterations, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty initVector = new ConfigurationProperty("initVector", typeof (string), _initVector, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty keySize = new ConfigurationProperty("keySize", typeof (int), _keySize, ConfigurationPropertyOptions.IsRequired);

        public EncryptionData()
        {
            base.Properties.Add(passPhrase);
            base.Properties.Add(saltValue);
            base.Properties.Add(hashAlgorithm);
            base.Properties.Add(passwordIterations);
            base.Properties.Add(initVector);
            base.Properties.Add(keySize);

        }

        [ConfigurationProperty("passPhrase", IsRequired = true)]
        public string PassPhrase { get { return (string) this[passPhrase]; } }

        [ConfigurationProperty("saltValue", IsRequired = true)]
        public string SaltValue { get { return (string) this[saltValue]; } }

        [ConfigurationProperty("hashAlgorithm", IsRequired = true)]
        public string HashAlgorithm { get { return (string) this[hashAlgorithm]; } }

        [ConfigurationProperty("passwordIterations", IsRequired = true)]
        public int PasswordIterations { get { return (int) this[passwordIterations]; } }

        [ConfigurationProperty("initVector", IsRequired = true)]
        public string InitVector { get { return (string) this[initVector]; } }

        [ConfigurationProperty("keySize", IsRequired = true)]
        public int KeySize { get { return (int) this[keySize]; } }
    }
}