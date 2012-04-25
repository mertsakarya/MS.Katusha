using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using MS.Katusha.Location;

namespace MS.Katusha.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var now = DateTime.Now;
            //var dto = new DateTimeOffset(now);
            //System.Console.WriteLine(now);
            //System.Console.WriteLine(dto);
            //System.Console.WriteLine(dto.ToLocalTime());
            //System.Console.WriteLine(dto.ToUniversalTime());
            //System.Console.WriteLine(now.ToLocalTime());
            //System.Console.WriteLine(now.ToUniversalTime());

            //now = DateTime.UtcNow;
            //dto = new DateTimeOffset(now);
            //System.Console.WriteLine(now);
            //System.Console.WriteLine(dto);

            //now = DateTime.UtcNow;
            //dto = DateTimeOffset.UtcNow;
            //System.Console.WriteLine(now);
            //System.Console.WriteLine(dto);

            //var dm = new DownloadManager();
            //dm.DownloadFiles();
            var c = GetAllCountries();
            System.Console.WriteLine(c);
            System.Console.ReadLine();
        }

        public static List<string> GetAllCountries()
        {
            var countryList = new List<string>();
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)) {

                RegionInfo ri = null;
                try {
                    ri = new RegionInfo(ci.LCID);
                } catch {}
                if (ri != null)
                    countryList.Add(ri.EnglishName + " / " + ri.NativeName);
            }
            return countryList.Distinct().ToList();
        }

        private static void Main1()
        {
            string connectionString = ""; // GetConnectionString();
            using (var sourceConnection = new SqlConnection(connectionString)) { 
                sourceConnection.Open();
                var commandRowCount = new SqlCommand("SELECT COUNT(*) FROM GeoNames(nolock);", sourceConnection);
                var countStart = Convert.ToInt32(commandRowCount.ExecuteScalar());
                var commandSourceData = new SqlCommand("SELECT ProductID, Name, ProductNumber FROM Production.Product;", sourceConnection);
                using (var reader = commandSourceData.ExecuteReader()) {
                    using (var destinationConnection = new SqlConnection(connectionString)) {
                        destinationConnection.Open();

                        using (var bulkCopy = new SqlBulkCopy(destinationConnection)) {
                            bulkCopy.DestinationTableName = "GeoNames";
                            try {
                                bulkCopy.WriteToServer(reader);
                            } finally {
                                reader.Close();
                            }
                        }
                        long countEnd = System.Convert.ToInt32(commandRowCount.ExecuteScalar());
                    }
                }
            }
        }
    }
}