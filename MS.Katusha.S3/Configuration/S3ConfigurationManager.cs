using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MS.Katusha.S3.Configuration
{
    public class S3ConfigurationManager
    {
        private readonly S3ConfigurationHandler config;

        /// <summary>
        /// Singleton instance of the ConfigManager
        /// </summary>
        private static readonly S3ConfigurationManager instance = new S3ConfigurationManager();
        public static S3ConfigurationManager Instance
        {
            get
            {
                return instance;
            }
        }
        private S3ConfigurationManager()
        {
            config = (S3ConfigurationHandler)ConfigurationManager.GetSection("s3");
            if (config == null) {
                throw new Exception("Cannot read config file");
            }
        }

        public Bucket GetBucket(string bucketName = "")
        {
            return config.Buckets[(String.IsNullOrWhiteSpace(bucketName) ? ConfigurationManager.AppSettings["S3.Default.Bucket"] : bucketName)];
        }
    }
}
