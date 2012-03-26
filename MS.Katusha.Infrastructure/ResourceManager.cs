using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MS.Katusha.Domain;
using MS.Katusha.Exceptions.Resources;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public interface IResourceManager
    {
        string _C(string key);
        string _C(string propertyName, string key, bool mustFind = false);
        string _R(string resourceName, byte language = 0);
        string _R(string propertyName, string key, bool mustFind = false, byte language = 0);
        Dictionary<string, string> _L(string resourceName, byte language = 0);
        string _LText(string resourceName, string name, byte language = 0);
        //List<string> GetValuesFromCodeList(List<string> resourceCodeList);
    }

    public class ResourceManager : IResourceManager
    {
        private static readonly IDictionary<string, string> ConfigurationList;
        private static readonly IDictionary<string, string> ResourceList;
        private static readonly IDictionary<string, Dictionary<string, string>> ResourceLookupList;
        private static readonly ReaderWriterLockSlim ListLock;

        static ResourceManager()
        {
            ConfigurationList = new Dictionary<string, string>();
            ResourceList = new Dictionary<string, string>();
            ResourceLookupList = new Dictionary<string, Dictionary<string, string>>();
            ListLock = new ReaderWriterLockSlim();
        }

        public ResourceManager()
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
        }

        public void LoadResourceFromDb(IResourceRepository resourceRepository)
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

        public void LoadConfigurationDataFromDb(IConfigurationDataRepository resourceRepository)
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
        }

        private static int CompareLookupItem(LookupItem x, LookupItem y)
        {
            return x.Order.CompareTo(y.Order);
        }

        public void LoadResourceLookupFromDb(IResourceLookupRepository resourceLookupRepository)
        {
            ListLock.EnterWriteLock();
            try {
                ResourceLookupList.Clear();
                var items = resourceLookupRepository.GetActiveValues();
                if (items.Length > 0) {
                    var lookupName = items[0].LookupName;
                    var language = items[0].Language;
                    var list = new List<LookupItem>();
                    foreach (var item in items) {
                        if (item.LookupName == lookupName && item.Language == language)
                            list.Add(new LookupItem {Key = item.ResourceKey, Value = item.Value, Order = item.Order});
                        else {
                            try {
                                list.Sort(CompareLookupItem);
                                ResourceLookupList.Add(lookupName+language, list.ToDictionary(r => r.Key, r => r.Value));
                            } catch (Exception ex) {
                                throw new KatushaResourceLookupException(lookupName, ex);
                            }
                            lookupName = item.LookupName;
                            language = item.Language;
                            list = new List<LookupItem> {new LookupItem {Key = item.ResourceKey, Value = item.Value, Order = item.Order}};
                        }
                    }
                    //list.Sort(CompareLookupItem);
                    //_resourceLookupList.Add(lookupName+language, list.ToDictionary(r => r.Key, r => r.Value));
                }
            } finally {
                ListLock.ExitWriteLock();
            }
        }

        public string _C(string key)
        {
            ListLock.EnterReadLock();
            try {
                return ConfigurationList.ContainsKey(key) ? ConfigurationList[key] : String.Format("{0} Code not found", key);
            } finally {
                ListLock.ExitReadLock();
            }
        }

        public string _C(string propertyName, string key, bool mustFind = false)
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

        public string _R(string resourceName, byte language = 0)
        {
            language = GetLanguage(language);
            var key = String.Format("{0}{1}",resourceName, language);
            ListLock.EnterReadLock();
            try {
                return ResourceList.ContainsKey(key) ? ResourceList[key] : String.Format("{0} Code not found", key);
            } finally {
                ListLock.ExitReadLock();
            }
        }

        public string _R(string propertyName, string key, bool mustFind = false, byte language = 0)
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

        public Dictionary<string, string> _L(string resourceName, byte language = 0)
        {
            language = GetLanguage(language);
            var resourceValue = new Dictionary<string, string>();
            string key = String.Format("{0}{1}", resourceName, language);
            if (!string.IsNullOrEmpty(key)) {
                ListLock.EnterReadLock();
                try {
                    if (ResourceLookupList.ContainsKey(key)) 
                        resourceValue = ResourceLookupList[key];
                } finally {
                    ListLock.ExitReadLock();
                }
            }

            return resourceValue;
        }

        public string _LText(string resourceName, string name, byte language = 0) {
            
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

        private static byte GetLanguage(byte language)
        {
            if (language == 0) {
                var cultureName = Thread.CurrentThread.CurrentCulture.Name;
                return ParseLanguageText(cultureName);
            }
            return language;
        }

        public static byte ParseLanguageText(string cultureName)
        {
            if (String.IsNullOrWhiteSpace(cultureName) || cultureName.Length < 2) return (byte) Language.DefaultLanguage;
            switch (cultureName.Substring(0, 2).ToLower()) {
                case "tr":
                    return (byte) Language.Turkish;
                case "ru":
                    return (byte) Language.Russian;
                case "en":
                    return (byte)Language.English;
                default:
                    return (byte) Language.DefaultLanguage;
            }
        }

        private static IResourceManager _instance = null;
        public static IResourceManager GetInstance() { return _instance ?? (_instance = new ResourceManager()); }
    }
}
