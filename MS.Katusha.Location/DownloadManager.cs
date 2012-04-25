using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MS.Katusha.Location
{
    public class DownloadManager
    {
        public const string root = "http://download.geonames.org/export/dump/";
        public const string downloadFolder = @"P:\GIT\MS.Katusha\GeoNames\";
        public string[] files;

        public void DownloadFiles() {
            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);

            files = File.ReadAllLines(@"P:\GIT\MS.Katusha\geonames_org_files.txt"); 
            foreach(var filename in files) {
                DownloadFiles(root, downloadFolder, filename);
            }
            while (Directory.GetFiles(downloadFolder).Length != files.Length) {}
            Console.WriteLine("DONE");
        }

        private void DownloadFiles(string webroot, string localroot, string filename)
        {
            var webClient = new WebClient();
            webClient.DownloadFileCompleted += Completed;
            webClient.DownloadFileAsync(new Uri(webroot + filename), localroot + filename, filename);
            Console.WriteLine("Downloading " + filename);
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            var filename = e.UserState as string;
            if (e.Error != null)
                Console.WriteLine(filename + ":" + e.Error.Message);
            else if (e.Cancelled)
                Console.WriteLine(filename + ": Cancelled");
            else
                Console.WriteLine(filename + ": Downloaded");
        }
    }
}
