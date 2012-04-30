using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public class ConfigParser
    {
        private readonly KatushaDbContext _dbContext;

        protected enum Section
        {
            None = 0,
            Configuration,
            Resource,
            ResourceLookup,
            Start
        };

        protected class ConfigurationType
        {
            public object Repository { get; set; }
            public int MinimumAllowed { get; set; }
            public int MaximumAllowed { get; set; }
            public int LanguageOrder { get; set; }
            public int OrderOrder { get; set; }
            public Section Section { get; private set; }
            private string _language;
            private byte _order;
            public ConfigurationType(Section section) { Section = section; }

            public string ParseLine(int line, string text, List<string> values)
            {
                if (values.Count < MinimumAllowed) return String.Format("{0} line ({1}) has less than Minimum ({3}) information ->  {2} ", Section, line, text, MinimumAllowed);
                if (values.Count > MaximumAllowed)
                    return String.Format("{0} line ({1}) has less than Maximum ({3}) information ->  {2} ", Section, line, text, MaximumAllowed);
                if (LanguageOrder >= 0) {
                    var result = GetLanguage(values, out _language);
                    if (result != null) return result;
                }
                if (OrderOrder >= 0) {
                    if (!(values.Count < OrderOrder + 1)) {
                        if (!Byte.TryParse(values[OrderOrder], out _order)) {
                            return String.Format("{0} Cannot parse order at line ({1}) at TAB order ({3}) information ->  {2} ", Section, line, text, OrderOrder);
                        }
                    }
                }
                GenerateModel(values);
                return null;
            }

            private void GenerateModel(IList<string> values)
            {
                switch (Section) {
                    case Section.Configuration: {
                        var configurationDataRepositoryDB = Repository as ConfigurationDataRepositoryDB;
                        if (configurationDataRepositoryDB != null) configurationDataRepositoryDB.Add(new ConfigurationData {Key = values[0], Value = ((values[1] == "\"\"") ? "" : values[1])});
                    }
                        break;
                    case Section.Resource: {
                        var resourceRepositoryDB = Repository as ResourceRepositoryDB;
                        if (resourceRepositoryDB != null) resourceRepositoryDB.Add(new Resource {Language = _language, ResourceKey = values[1], Value = ((values[2] == "\"\"") ? "" : values[2])});
                    }
                        break;
                    case Section.ResourceLookup: {
                        var resourceLookup = new ResourceLookup {Language = _language, LookupName = values[1], ResourceKey = values[2], Value = ((values[3] == "\"\"") ? "" : values[3])};
                        if (_order > 0) resourceLookup.Order = _order;
                        byte value;
                        if (Byte.TryParse(values[5], out value)) {
                            resourceLookup.LookupValue = value;
                        }
                        var resourceLookupRepositoryDB = Repository as ResourceLookupRepositoryDB;
                        if (resourceLookupRepositoryDB != null) resourceLookupRepositoryDB.Add(resourceLookup);
                    }
                        break;
                }
            }

            protected static string GetLanguage(List<string> values, out string language) { language = values[0].Substring(0, 2).ToLowerInvariant(); return null; }

        }

        private readonly Dictionary<Section, ConfigurationType> _dependencies;
        private static readonly string root = HttpContext.Current.Server.MapPath(@"~\ConfigData\");
        private static readonly string ConfigurationFilename = root + @"ConfigurationData.txt";
        //private static readonly string GeoNamesFilename = root + @"allCountries.txt";
        private static readonly string GeoNamesFilename = @"cities15000.txt";
        private static readonly string GeoCountryFilename = @"countryInfo.txt";
        private static readonly string GeoTimeZoneFilename = @"timeZones.txt";
        private static readonly string GeoLanguageFilename = @"iso-languagecodes.txt";
        private readonly GeoTimeZoneRepositoryDB _geoTimeZoneRepository;
        private readonly GeoNameRepositoryDB _geoNameRepository;
        private readonly GeoCountryRepositoryDB _geoCountryRepository;
        private readonly GeoLanguageRepositoryDB _geoLanguageRepository;

        public ConfigParser(KatushaDbContext dbContext)
        {
            _dbContext = dbContext;
            var configurationData = new ConfigurationType(Section.Configuration) {
                                                                                     Repository = new ConfigurationDataRepositoryDB(dbContext),
                                                                                     MinimumAllowed = 2,
                                                                                     MaximumAllowed = 2,
                                                                                     LanguageOrder = -1,
                                                                                     OrderOrder = -1
                                                                                 };

            var resource = new ConfigurationType(Section.Resource) {
                                                                       Repository = new ResourceRepositoryDB(dbContext),
                                                                       MinimumAllowed = 3,
                                                                       MaximumAllowed = 3,
                                                                       LanguageOrder = 0,
                                                                       OrderOrder = -1
                                                                   };

            var resourceLookup = new ConfigurationType(Section.ResourceLookup) {
                                                                                   Repository = new ResourceLookupRepositoryDB(dbContext),
                                                                                   MinimumAllowed = 6,
                                                                                   MaximumAllowed = 6,
                                                                                   LanguageOrder = 0,
                                                                                   OrderOrder = 4
                                                                               };

            _dependencies = new Dictionary<Section, ConfigurationType> {
                                                                           {Section.Configuration, configurationData},
                                                                           {Section.Resource, resource},
                                                                           {Section.ResourceLookup, resourceLookup}
                                                                       };

            _geoTimeZoneRepository = new GeoTimeZoneRepositoryDB(dbContext);
            _geoNameRepository = new GeoNameRepositoryDB(dbContext);
            _geoCountryRepository = new GeoCountryRepositoryDB(dbContext);
            _geoLanguageRepository = new GeoLanguageRepositoryDB(dbContext);
        }

        public string[] Parse()
        {
            var result = new List<string>();
            var mode = Section.Start;
            var line = 0;
            using (var stream = new StreamReader(ConfigurationFilename)) {
                while (!stream.EndOfStream) {
                    string text = stream.ReadLine();
                    line++;
                    if (mode == Section.Start) {
                        if (text == "[START]")
                            mode = Section.None;
                        continue;
                    }
                    if (String.IsNullOrWhiteSpace(text)) continue;
                    text = text.Trim();
                    var firstChar = text[0];
                    if (firstChar == '[') {
                        text = text.ToLowerInvariant();
                        var i = text.IndexOf("]", StringComparison.Ordinal);
                        if (i > 2) {
                            if (!Enum.TryParse(text.Substring(1, i - 1), true, out mode)) mode = Section.None;

                        }
                    } else if (firstChar != '*' && firstChar != '!' && firstChar != '#' && mode != Section.None) {
                        var arr = text.Split('\t');
                        var values = new List<string>();
                        if (values.Count > 0 && values.Count < 2) {
                            result.Add(String.Format("{2} ERROR at line ({1}) \t{0}", text, line, mode));
                        } else {
                            var dependency = _dependencies[mode];
                            values.AddRange(from t in arr select t.Trim() into item where !String.IsNullOrWhiteSpace(item) select item.Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n"));
                            var retval = dependency.ParseLine(line, text, values);
                            if (retval != null)
                                result.Add(retval);
                        }
                    }
                }
            }

            result.Add(BulkCopy(root, GeoLanguageFilename, "GeoLanguages", true, 
                new[] {"ISO639_3", "ISO639_2", "ISO639_1", "LanguageName"},
                new[] { typeof(string), typeof(string), typeof(string), typeof(string)}
            ));
            result.Add(BulkCopy(root, GeoTimeZoneFilename, "GeoTimeZones", true, 
                new[] {"TimeZoneId", "GMTOffset", "DSTOffset", "RawOffset"},
                new[] { typeof(string), typeof(double), typeof(double), typeof(double)}
            ));
            result.Add(BulkCopy(root, GeoCountryFilename, "GeoCountries", true,
                new[] {
                        "ISO", "ISO3", "ISONumeric", "FIPS", 
                        "Country", "Capital", "Area", "Population", 
                        "Continent", "TLD", "CurrencyCode", "CurrencyName", 
                        "Phone", "PostalCodeFormat", "PostalCodeRegEx", "Languages", 
                        "GeoNameId", "Neighbors", "EquivalentFipsCode"
                    },
                new[] {
                    typeof(string), typeof(string), typeof(int), typeof(string), 
                    typeof(string), typeof(string), typeof(int), typeof(long), 
                    typeof(string), typeof(string), typeof(string), typeof(string), 
                    typeof(string), typeof(string), typeof(string), typeof(string), 
                    typeof(int), typeof(string), typeof(string)
                }
            ));
            result.Add(BulkCopy(root, GeoNamesFilename, "GeoNames", false,
                new[] {
                        "GeoNameId", "Name", "AsciiName", "AlternateNames", 
                        "Latitude", "Longitude", "FeatureClass", "FeatureCode",
                        "CountryCode", "CC2", "Admin1code", "Admin2code", 
                        "Admin3code", "Admin4code", "Population", "Elevation", 
                        "DEM", "TimeZone", "ModificationDate"
                        },
                new[] {
                    typeof(int), typeof(string), typeof(int), typeof(string), 
                    typeof(double), typeof(double), typeof(int), typeof(long), 
                    typeof(string), typeof(string), typeof(string), typeof(string), 
                    typeof(string), typeof(string), typeof(long), typeof(int), 
                    typeof(int), typeof(string), typeof(string)
                }
            ));
            return result.ToArray();
        }

        private string BulkCopy(string basePath, string filename, string tableName, bool hasHeaders, string[] mapColumnNames, Type[] mapTypes)
        {
            var result = "";
            var sb = new StringBuilder();
            sb.Append("INSERT [");
            sb.Append(tableName);
            sb.Append("] (");
            sb.Append(String.Join(",", mapColumnNames));
            sb.Append(") VALUES (");
            var colarr = new List<string>(mapColumnNames.Length);
            for (var i = 0; i < mapColumnNames.Length; i++) {

                colarr.Add("{" + i + "}");
            }
            sb.Append(String.Join(",", colarr));
            sb.Append(")");
            var command = sb.ToString();
            var rows = GetLineArray(basePath, filename, hasHeaders, mapTypes);
            sb = new StringBuilder();
            //sb.AppendFormat("set IDENTITY_INSERT [{0}] OFF\r\n", tableName);
            var counter = 0;

            try {
                foreach (var row in rows) {
                    sb.AppendLine(String.Format(command, row));
                    counter++;
                    if (counter == 100) {
                        WriteToDatabase(sb);
                        counter = 0;
                        sb = new StringBuilder();
                        //sb.AppendFormat("set IDENTITY_INSERT [{0}] ON\r\n", tableName);
                    }
                }
                WriteToDatabase(sb);                
            } catch (Exception ex) {
                result = ex.Message;
            }
            return result;
        }

        private void WriteToDatabase(StringBuilder sb)
        {
            var val = sb.ToString();
            if (String.IsNullOrWhiteSpace(val)) return;
            using (var sourceConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MS.Katusha.Domain.KatushaDbContext"].ConnectionString)) {
                sourceConnection.Open();
                try {
                    using (var tran = sourceConnection.BeginTransaction()) {
                        var cmd = sourceConnection.CreateCommand();
                        cmd.Transaction = tran;
                        cmd.CommandText = val;
                        var lines = cmd.ExecuteNonQuery();
                        tran.Commit();
                        //Debug.WriteLine(lines);
                    }
                } finally {
                    sourceConnection.Close();
                }
            }
        }

        private static IList<string[]> GetLineArray(string basePath, string filename, bool hasHeaders, Type[] mapTypes)
        {
            IList<string[]> list = new List<string[]>();
            using (var stream = new StreamReader(basePath + filename, Encoding.UTF8)) {
                var line = 0;
                while (!stream.EndOfStream) {
                    var text = stream.ReadLine();
                    line++;
                    if (hasHeaders && line == 1) continue;
                    if (String.IsNullOrWhiteSpace(text)) continue;
                    if (text[0] == '#') continue;
                    var arr = text.Split('\t');
                    IList<string> objects = new List<string>(mapTypes.Length);
                    for (var i = 0; i < mapTypes.Length; i++) {
                        var type = mapTypes[i];
                        object val = null;
                        if (type == typeof (int)) {
                            int v = 0;
                            int.TryParse(arr[i], out v);
                            val = v;
                        } else if (type == typeof (long)) {
                            long v = 0;
                            long.TryParse(arr[i], out v);
                            val = v;
                        } else if (type == typeof (decimal)) {
                            decimal v = 0;
                            decimal.TryParse(arr[i], out v);
                            val = v;
                        } else if (type == typeof (double)) {
                            double v = 0.0;
                            double.TryParse(arr[i], out v);
                            val = v;
                        } else val = "'"+arr[i].Replace("'","''")+"'";
                        objects.Add(val.ToString());
                    }
                    list.Add(objects.ToArray());
                    //dt.Rows.Add(objects.ToArray());
                }
            }
            return list;
        }
    }

}
