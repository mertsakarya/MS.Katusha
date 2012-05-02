using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure.Exceptions.Resources;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public class ResourceManager
    {
        private static readonly IDictionary<string, string> ConfigurationList;
        private static readonly IDictionary<string, string> ResourceList;
        private static readonly IDictionary<string, Dictionary<string, string>> ResourceLookupList;
        private static readonly IDictionary<string, Dictionary<string, byte>> ResourceLookupByteList;
        private static readonly GeoLocation Location;

        private static readonly ReaderWriterLockSlim ListLock;

        static ResourceManager()
        {
            ConfigurationList = new Dictionary<string, string>();
            ResourceList = new Dictionary<string, string>();
            ResourceLookupList = new Dictionary<string, Dictionary<string, string>>();
            ResourceLookupByteList = new Dictionary<string, Dictionary<string, byte>>();
            Location = new GeoLocation();
            ListLock = new ReaderWriterLockSlim();
        }

        private ResourceManager()
        {
            ListLock.EnterReadLock();
            var isEmpty = ResourceLookupList.Count <= 0;
            ListLock.ExitReadLock();
            if (isEmpty)  LoadResourceLookupFromDb(new ResourceLookupRepositoryDB(new KatushaDbContext()));
            
            ListLock.EnterReadLock();
            isEmpty = ResourceList.Count <= 0;
            ListLock.ExitReadLock();
            if (isEmpty) LoadResourceFromDb(new ResourceRepositoryDB(new KatushaDbContext()));

            ListLock.EnterReadLock();
            isEmpty = ConfigurationList.Count <= 0;
            ListLock.ExitReadLock();
            if (isEmpty) LoadConfigurationDataFromDb(new ConfigurationDataRepositoryDB(new KatushaDbContext()));

            ListLock.EnterReadLock();
            isEmpty = !Location.Initialized;
            ListLock.ExitReadLock();
            if (isEmpty) {
                var dbContext = new KatushaDbContext();
                LoadGeoLocationDataFromDb(new GeoCountryRepositoryDB(dbContext), new GeoLanguageRepositoryDB(dbContext), new GeoNameRepositoryDB(dbContext), new GeoTimeZoneRepositoryDB(dbContext));
            }
        }

        private static void LoadGeoLocationDataFromDb(GeoCountryRepositoryDB countryRepository, GeoLanguageRepositoryDB languageRepository, GeoNameRepositoryDB nameRepository, GeoTimeZoneRepositoryDB timeZoneRepository)
        {
            ListLock.EnterWriteLock();
            try {
                Location.Countries.Clear();
                foreach (var item in countryRepository.GetAll()) {
                    try {
                        Location.Countries.Add(item.ISO.ToLowerInvariant(), item);
                    } catch (Exception ex) {
                        throw new KatushaGeoLocationException(item.ISO.ToLowerInvariant(), item.Country, ex);
                    }
                }
                Location.Languages.Clear();
                foreach (var item in languageRepository.GetAll()) {
                    try {
                        if (!String.IsNullOrWhiteSpace(item.ISO639_1))
                            Location.Languages.Add(item.ISO639_1.ToLowerInvariant(), item);
                    } catch (Exception ex) {
                        throw new KatushaGeoLocationException(item.ISO639_1.ToLowerInvariant(), item.LanguageName, ex);
                    }
                }
                Location.TimeZones.Clear();
                foreach (var item in timeZoneRepository.GetAll()) {
                    try {
                        Location.TimeZones.Add(item.TimeZoneId.ToLowerInvariant(), item);
                    } catch (Exception ex) {
                        throw new KatushaGeoLocationException(item.TimeZoneId.ToLowerInvariant(), item.TimeZoneId, ex);
                    }
                }
                Location.Names.Clear();
                foreach (var item in nameRepository.GetAll()) {
                    try {
                        Location.Names.Add(item.GeoNameId.ToString(CultureInfo.InvariantCulture), item);
                    } catch (Exception ex) {
                        throw new KatushaGeoLocationException(item.GeoNameId.ToString(CultureInfo.InvariantCulture), item.Name, ex);
                    }
                }
                Location.Initialize();;
            } finally {
                ListLock.ExitWriteLock();
            }
        }

        public static void LoadResourceFromDb(IResourceRepository resourceRepository)
        {
            ListLock.EnterWriteLock();
            try {
                ResourceList.Clear();
                foreach (var item in resourceRepository.GetActiveValues()) {
                    try {
                        ResourceList.Add(item.Key, item.Value);
                    } catch (Exception ex) {
                        throw new KatushaConfigurationException(item.Key, item.Value, ex);
                    }
                }
            } finally {
                ListLock.ExitWriteLock();
            }
        }

        public static void LoadConfigurationDataFromDb(IConfigurationDataRepository resourceRepository)
        {
            ListLock.EnterWriteLock();
            try {
                ConfigurationList.Clear();
                foreach (var item in resourceRepository.GetActiveValues()) {
                    try {
                        ConfigurationList.Add(item.Key, item.Value);
                    } catch (Exception ex) {
                        throw new KatushaResourceException(item.Key, item.Value, ex);
                    }
                }
            } finally {
                ListLock.ExitWriteLock();
            }
        }

        private class LookupItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public byte Order { get; set; }
            public byte ByteValue { get; set; }
        }

        private static int CompareLookupItem(LookupItem x, LookupItem y)
        {
            return x.Order.CompareTo(y.Order);
        }

        public static void LoadResourceLookupFromDb(IResourceLookupRepository resourceLookupRepository)
        {
            ListLock.EnterWriteLock();
            try {
                ResourceLookupList.Clear();
                ResourceLookupByteList.Clear();
                var items = resourceLookupRepository.GetActiveValues();
                if (items.Length > 0) {
                    var lookupName = items[0].LookupName;
                    var language = items[0].Language;
                    var list = new List<LookupItem>();
                    foreach (var item in items) {
                        if (item.LookupName == lookupName && item.Language == language)
                            list.Add(new LookupItem {Key = item.ResourceKey, Value = item.Value, Order = item.Order, ByteValue = item.LookupValue});
                        else {
                            try {
                                list.Sort(CompareLookupItem);
                                ResourceLookupList.Add(lookupName + language, list.ToDictionary(r => r.Key, r => r.Value));
                                ResourceLookupByteList.Add(lookupName + language, list.ToDictionary(r => r.Key, r => r.ByteValue));
                            } catch (Exception ex) {
                                throw new KatushaResourceLookupException(lookupName, ex);
                            }
                            lookupName = item.LookupName;
                            language = item.Language;
                            list = new List<LookupItem> { new LookupItem { Key = item.ResourceKey, Value = item.Value, Order = item.Order, ByteValue = item.LookupValue } };
                        }
                    }
                }
            } finally {
                ListLock.ExitWriteLock();
            }
        }

        public string ConfigurationValue(string key)
        {
            ListLock.EnterReadLock();
            try {
                return ConfigurationList.ContainsKey(key) ? ConfigurationList[key] : String.Format("{0} Code not found", key);
            } finally {
                ListLock.ExitReadLock();
            }
        }

        public string ConfigurationValue(string propertyName, string key, bool mustFind = false)
        {
            ListLock.EnterReadLock();
            try {
                if (ConfigurationList.ContainsKey(propertyName + "." + key)) return ConfigurationList[propertyName + "." + key];
                if (ConfigurationList.ContainsKey(key)) return ConfigurationList[key];
                return (mustFind) ? String.Format("{0} Code not found", key) : null;
            } finally {
                ListLock.ExitReadLock();
            }
        }

        public string ResourceValue(string resourceName, string language = "")
        {
            language = GetLanguage(language);
            var key = String.Format("{0}{1}", resourceName, language);
            ListLock.EnterReadLock();
            try {
                return ResourceList.ContainsKey(key) ? ResourceList[key] : String.Format("{0} Code not found", key);
            } finally {
                ListLock.ExitReadLock();
            }
        }

        public string ResourceValue(string propertyName, string key, bool mustFind, string language)
        {
            language = GetLanguage(language);
            var name = String.Format("{0}.{1}{2}", propertyName, key, language);
            ListLock.EnterReadLock();
            try {
                if (ResourceList.ContainsKey(name)) return ResourceList[name];
                name = String.Format("{0}{1}", key, language);
                if (ResourceList.ContainsKey(name)) return ResourceList[name];
                return (mustFind) ? String.Format("{0} Code not found", key) : null;
            } finally {
                ListLock.ExitReadLock();
            }
        }

        public IDictionary<string, string> GetCountries() { return Location.GetCountries(); }
        public IDictionary<string, string> GetLanguages() { return Location.GetLanguages(); }
        public IList<string> GetCities(string countryCode) { return Location.GetCities(countryCode); }

        public IDictionary<string, string> GetLookup(string lookupName, string countryCode = "")
        {
            IDictionary<string, string> resourceValue;
            ListLock.EnterReadLock();
            try {
                resourceValue = Location.GetLookup(lookupName, countryCode);
            } finally {
                ListLock.ExitReadLock();
            }
            if (resourceValue == null || resourceValue.Count == 0) {
                var language = GetLanguage("");
                resourceValue = new Dictionary<string, string>();
                var key = String.Format("{0}{1}", lookupName, language);
                if (!string.IsNullOrEmpty(key)) {
                    ListLock.EnterReadLock();
                    try {
                        if (ResourceLookupList.ContainsKey(key))
                            resourceValue = ResourceLookupList[key];
                    } finally {
                        ListLock.ExitReadLock();
                    }
                }
            }
            return resourceValue;
        }

        public string GetLookupEnumKey(string lookupName, byte value, string language = "")
        {
            if (value == 0) return "";
            language = GetLanguage(language);
            string key = String.Format("{0}{1}", lookupName, language);
            if (!string.IsNullOrEmpty(key)) {
                ListLock.EnterReadLock();
                try {
                    if (ResourceLookupByteList.ContainsKey(key)) {
                        var resourceValue = ResourceLookupByteList[key];
                        foreach(var item in resourceValue) {
                            if (item.Value == value)
                                return item.Key;
                        }
                    }
                } finally {
                    ListLock.ExitReadLock();
                }
            }

            return key + "." + value.ToString(CultureInfo.InvariantCulture);
        }

        public bool ContainsKey(string lookupName, string key, string countryCode = "")
        {
            var lookup = GetLookup(lookupName, countryCode);
            return lookup.ContainsKey(key);
        }

        public string GetLookupText(string lookupName, string key, string countryCode = "")
        {
            if (String.IsNullOrWhiteSpace(key) || key == "0") return "";
            var retval = Location.GetValue(lookupName, key, countryCode);
            return !String.IsNullOrWhiteSpace(retval) ? retval : GetLookupTextOthers(lookupName, key, "");
        }

        private static string GetLookupTextOthers(string resourceName, string name, string language = "")
        {

            if (String.IsNullOrWhiteSpace(name) || name == "0") return "";

            language = GetLanguage(language);
            string key = String.Format("{0}{1}", resourceName, language);
            if (!string.IsNullOrEmpty(key)) {
                ListLock.EnterReadLock();
                try {
                    if (ResourceLookupList.ContainsKey(key)) {
                        var resourceValue = ResourceLookupList[key];
                        if (resourceValue.ContainsKey(name))
                            return resourceValue[name];
                    }
                } finally {
                    ListLock.ExitReadLock();
                }
            }

            return key + "." + name;
        }

        private static string GetLanguage(string language)
        {
            if (String.IsNullOrWhiteSpace(language)) {
                var cultureName = Thread.CurrentThread.CurrentCulture.Name;
                if (String.IsNullOrWhiteSpace(cultureName) || cultureName.Length < 2) return "en";
                return cultureName.Substring(0, 2).ToLowerInvariant();
            }
            return language;
        }

        private static ResourceManager _instance = null;
        public static ResourceManager GetInstance() { return _instance ?? (_instance = new ResourceManager()); }
    }
}
