using System;
using System.Configuration;
using MS.Katusha.Enumerations;
using MS.Katusha.Services.Configuration.Data;

namespace MS.Katusha.Services.Configuration
{
    public class KatushaConfigurationManager
    {
        private readonly KatushaConfigurationHandler config;
        private static readonly KatushaConfigurationManager instance = new KatushaConfigurationManager();
        private readonly MSKatushaSource _msKatushaSource;
        private readonly string _connectionString;
        private readonly string _virtualPath;
        public static KatushaConfigurationManager Instance { get { return instance; } }
        
        private KatushaConfigurationManager()
        {
            config = (KatushaConfigurationHandler)ConfigurationManager.GetSection("katusha"); 
            if (config == null) throw new Exception("Cannot read config file");
            _msKatushaSource = MSKatushaSource.Local;
            var source = ConfigurationManager.AppSettings["MS.Katusha.Source"];
            if (source != null) {
                switch (source.ToLowerInvariant()) {
                    case "liveeu":
                        _msKatushaSource = MSKatushaSource.LiveEU;
                        break;
                    case "live":
                        _msKatushaSource = MSKatushaSource.Live;
                        break;
                }
            }
            _virtualPath = config.Settings.Protocol + ConfigurationManager.AppSettings["VirtualPath"];
            _connectionString = (_msKatushaSource != MSKatushaSource.Local) ? ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"] : ConfigurationManager.ConnectionStrings["MS.Katusha.Domain.KatushaDbContext"].ConnectionString;

        }

        public EncryptionData GetEncryption() { return config.Encryption; }

        public BucketData GetBucket(string bucketName = "")
        {
            return config.Buckets[(String.IsNullOrWhiteSpace(bucketName) ? ConfigurationManager.AppSettings["S3.Default.Bucket"] : bucketName)];
        }

        public SettingsData GetSettings()
        {
            return config.Settings;
        }

        public string VirtualPath { get
        {
            return _virtualPath;
        }
        }

        public MSKatushaSource MSKatushaSource
        {
            get { return _msKatushaSource; }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
    }
}
