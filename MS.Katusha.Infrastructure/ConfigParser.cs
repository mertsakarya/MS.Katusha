using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public  class ConfigParser
    {
        protected enum Section
        {
            None = 0,
            Configuration,
            Resource,
            ResourceLookup
        };

        protected class ConfigurationType {
            public object Repository { get; set; }
            public int MinimumAllowed { get; set; }
            public int MaximumAllowed { get; set; }
            public int LanguageOrder { get; set; }
            public int OrderOrder { get; set; }
            public Section Section { get; private set; }
            private byte _language;
            private byte _order;
            public ConfigurationType(Section section) { Section = section; }

            public string ParseLine(int line, string text, List<string> values)
            {
                if (values.Count < MinimumAllowed) return String.Format("{0} line ({1}) has less than Minimum ({3}) information ->  {2} ", Section, line, text, MinimumAllowed);
                if(values.Count > MaximumAllowed)
                    return String.Format("{0} line ({1}) has less than Maximum ({3}) information ->  {2} ", Section, line, text, MaximumAllowed);
                if (LanguageOrder >= 0) {
                    var result = GetLanguage(values, out _language);
                    if(result != null) return result;
                }
                if(OrderOrder >= 0) {
                    if(!(values.Count < OrderOrder+1)) {
                        if (!Byte.TryParse(values[OrderOrder], out _order)) {
                            return String.Format("{0} Cannot parse order at line ({1}) at TAB order ({3}) information ->  {2} ", Section, line, text, OrderOrder);
                        }
                    }
                }
                GenerateModel(values);
                return null;
            }

            private void GenerateModel(List<string> values)
            {
                if(Section == Section.Configuration) {
                    var configurationDataRepositoryDB = Repository as ConfigurationDataRepositoryDB;
                    if (configurationDataRepositoryDB != null) configurationDataRepositoryDB.Add(new ConfigurationData {Key = values[0], Value = ((values[1] == "\"\"") ? "" : values[1]) });
                } else if (Section == Section.Resource) {
                    var resourceRepositoryDB = Repository as ResourceRepositoryDB;
                    if (resourceRepositoryDB != null) resourceRepositoryDB.Add(new Resource { Language = _language, ResourceKey = values[1], Value = ((values[2] == "\"\"") ? "" : values[2]) });
                } else if (Section == Section.ResourceLookup) {
                    var resourceLookup = new ResourceLookup { Language = _language, LookupName = values[1], ResourceKey = values[2], Value = ((values[3] == "\"\"") ? "" : values[3]) };
                    if (_order > 0) resourceLookup.Order = _order;
                    var resourceLookupRepositoryDB = Repository as ResourceLookupRepositoryDB;
                    if (resourceLookupRepositoryDB != null) resourceLookupRepositoryDB.Add(resourceLookup);
                }
            }

            protected static string GetLanguage(List<string> values, out byte language)
            {
                Language ll = 0;
                byte lb;
                if (!Byte.TryParse(values[0], out lb)) {
                    if (!Enum.TryParse(values[0], true, out ll)) {
                        var cultureName = values[0];
                        if (cultureName.Length >= 2) {
                            switch (cultureName.Substring(0, 2).ToLowerInvariant()) {
                                case "tr":
                                    lb = (byte) Language.Turkish;
                                    break;
                                case "ru":
                                    lb = (byte) Language.Russian;
                                    break;
                                case "en":
                                    lb = (byte) Language.English;
                                    break;
                            }
                            if (lb == 0) {
                                language = 255;
                                return String.Format("LANGUAGE ERROR: {0} {1} {2}", values[0], values[1], values[2]);
                            }
                        }
                    }
                }
                language = (byte)(lb + (byte)ll);
                if (language <= 0 || language > (byte)Language.MaxLanguage) {
                    language = 255;
                    return String.Format("LANGUAGE ERROR: {0} {1} {2}", values[0], values[1], values[2]);
                }
                return null;
            }

        }

        private readonly Dictionary<Section, ConfigurationType> _dependencies;
        private static readonly string ConfigurationFilename = ConfigurationManager.AppSettings["Root_Folder"] + @"MS.Katusha.Web\Content\ConfigurationData.csv";

        public ConfigParser(KatushaDbContext dbContext)
        {
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
                MinimumAllowed = 4,
                MaximumAllowed = 5,
                LanguageOrder = 0,
                OrderOrder = 4
            };

            _dependencies = new Dictionary<Section, ConfigurationType> {
                                                                {Section.Configuration, configurationData },
                                                                {Section.Resource, resource},
                                                                {Section.ResourceLookup, resourceLookup}
                                                            };
        }

        public string[] Parse()
        {
            var mode = Section.None;
            var result = new List<string>();
            var line = 0;
            using (var stream = new StreamReader(ConfigurationFilename)) {
                while (!stream.EndOfStream) {
                    string text = stream.ReadLine();
                    line++;
                    if (!String.IsNullOrWhiteSpace(text)) {
                        text = text.Trim();
                        char firstChar = text[0];
                        if (firstChar == '[') {
                            text = text.ToLowerInvariant();
                            var i = text.IndexOf("]", StringComparison.Ordinal);
                            if(i > 2) {
                                if(!Enum.TryParse(text.Substring(1, i-1), true, out mode)) mode = Section.None;
                                
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
                                if(retval != null)
                                    result.Add(retval);
                            }
                        }
                    }
                }
            }
            return result.ToArray();
        }

    }

}
