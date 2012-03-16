using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public static class ReloadResources
    {
        private static readonly string RootFolder = ConfigurationManager.AppSettings["Root_Folder"];

        public static void DeleteResourceLookups(KatushaDbContext dbContext)
        {
            var repository = new ResourceLookupRepositoryDB(dbContext);
            foreach (var item in repository.GetAll().ToArray())
                repository.Delete(item);
            repository.Save();
        }

        public static void DeleteResources(KatushaDbContext dbContext)
        {
            var repository = new ResourceRepositoryDB(dbContext);
            foreach (var item in repository.GetAll().ToArray())
                repository.Delete(item);
            repository.Save();
        }

        public static void SetResourceLookups(KatushaDbContext dbContext)
        {
            var repository = new ResourceLookupRepositoryDB(dbContext);
            using (var stream = new StreamReader(RootFolder + @"MS.Katusha.Web\Content\ResourceLookup.csv")) {
                while (!stream.EndOfStream) {
                    string text = stream.ReadLine();
                    if (!String.IsNullOrWhiteSpace(text)) {
                        var arr = text.Trim().Split('\t');
                        var values = new List<string>();
                        foreach (string t in arr) {
                            var item = t.Trim();
                            if (!String.IsNullOrWhiteSpace(item))
                                values.Add(item.Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n"));
                            if (values.Count == 5) {
                                AddResourceLookup(repository, values);
                                break;
                            }
                        }
                        if (values.Count == 4) {
                            AddResourceLookup(repository, values, false);
                        }
                        Debug.WriteLineIf((values.Count > 0 && values.Count < 4), "ERROR! \t" + text);
                    }
                }
            }
            dbContext.SaveChanges();
        }

        public static void SetResources(KatushaDbContext dbContext)
        {
            var repository = new ResourceRepositoryDB(dbContext);
            using (var stream = new StreamReader(RootFolder + @"MS.Katusha.Web\Content\Resource.csv")) {
                while (!stream.EndOfStream) {
                    string text = stream.ReadLine();
                    if (!String.IsNullOrWhiteSpace(text)) {
                        var arr = text.Trim().Split('\t');
                        var values = new List<string>();
                        foreach (string t in arr) {
                            var item = t.Trim();
                            if (!String.IsNullOrWhiteSpace(item))
                                values.Add(item.Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n"));
                            if (values.Count == 3) {
                                AddResource(repository, values);
                                break;
                            }
                        }
                        Debug.WriteLineIf((values.Count > 0 && values.Count != 3), "ERROR! \t" + text);
                    }
                }
            }
            dbContext.SaveChanges();
        }

        private static void AddResourceLookup(ResourceLookupRepositoryDB repository, List<string> values, bool hasOrder = true)
        {
            byte language;
            byte order;
            if (GetLanguage(values, out language)) return;
            if (!hasOrder)
                order = 0;
            else {
                if (!Byte.TryParse(values[4], out order)) {
                    Debug.WriteLine(String.Format("ORDER ERROR: {0} {1} {2} {3} {4}", values[0], values[1], values[2],
                                                  values[3], values[4]));
                    return;
                }
            }
            repository.Add(new ResourceLookup { LookupName = values[1], ResourceKey = values[2], Language = language, Value = values[3], Order = order });
        }

        private static bool GetLanguage(List<string> values, out byte language)
        {
            Language ll = 0;
            byte lb = 0;
            if (!Byte.TryParse(values[0], out lb)) {
                if (!Enum.TryParse(values[0], true, out ll)) {
                    Debug.WriteLine(String.Format("LANGUAGE ERROR: {0} {1} {2}", values[0], values[1], values[2]));
                    language = 255;
                    return true;
                }
            }
            language = (byte)(lb + (byte)ll);
            if (language < 0 || language > (byte)Language.MaxLanguage) {
                Debug.WriteLine(String.Format("LANGUAGE ERROR: {0} {1} {2}", values[0], values[1], values[2]));
                return true;
            }
            return false;
        }

        private static void AddResource(ResourceRepositoryDB repository, List<string> values)
        {
            byte language;
            if (GetLanguage(values, out language)) return;
            repository.Add(new Resource { ResourceKey = values[1], Language = language, Value = values[2] });
        }
    }
}
