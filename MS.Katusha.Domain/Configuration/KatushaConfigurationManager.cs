using System;
using System.Configuration;
using MS.Katusha.Enumerations;
using MS.Katusha.Configuration.Data;

namespace MS.Katusha.Configuration
{
    public class KatushaConfigurationManager
    {
        private readonly KatushaConfigurationHandler config;
        private static readonly KatushaConfigurationManager instance = new KatushaConfigurationManager();
        private readonly MSKatushaSource _msKatushaSource;
        private readonly string _connectionString;
        public static KatushaConfigurationManager Instance { get { return instance; } }
        public static MSKatushaMode Mode = (ConfigurationManager.AppSettings["MS.Katusha.Mode"] != null &&  ConfigurationManager.AppSettings["MS.Katusha.Mode"].ToLowerInvariant().Substring(0,3) == "win") ? MSKatushaMode.Windows : MSKatushaMode.Web;
        
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
            Mode = (ConfigurationManager.AppSettings["MS.Katusha.Mode"] != null &&  ConfigurationManager.AppSettings["MS.Katusha.Mode"].ToLowerInvariant().Substring(0,3) == "win") ? MSKatushaMode.Windows : MSKatushaMode.Web;

            _connectionString = (_msKatushaSource != MSKatushaSource.Local) ? ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"] : ConfigurationManager.ConnectionStrings["MS.Katusha.Repositories.DB.Context.KatushaDbContext"].ConnectionString;
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

        public string VirtualPath
        {
            get
            {
                var _virtualPath = config.Settings.Protocol + ConfigurationManager.AppSettings["VirtualPath"];
                return _virtualPath;
            }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
    }
}
