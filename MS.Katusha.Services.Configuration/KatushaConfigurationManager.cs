using System;
using System.Configuration;
using MS.Katusha.Services.Configuration.Data;

namespace MS.Katusha.Services.Configuration
{
    public class KatushaConfigurationManager
    {
        private readonly KatushaConfigurationHandler config;
        private static readonly KatushaConfigurationManager instance = new KatushaConfigurationManager();

        public static KatushaConfigurationManager Instance { get { return instance; } }
        
        private KatushaConfigurationManager() { config = (KatushaConfigurationHandler)ConfigurationManager.GetSection("katusha"); if (config == null) throw new Exception("Cannot read config file"); }

        public EncryptionData GetEncryption() { return config.Encryption; }

        public BucketData GetBucket(string bucketName = "")
        {
            return config.Buckets[(String.IsNullOrWhiteSpace(bucketName) ? ConfigurationManager.AppSettings["S3.Default.Bucket"] : bucketName)];
        }

        public SettingsData GetSettings()
        {
            return config.Settings;
        }

        public string VirtualPath { get { return GetSettings().Protocol+ConfigurationManager.AppSettings["VirtualPath"]; } }


    }
}
