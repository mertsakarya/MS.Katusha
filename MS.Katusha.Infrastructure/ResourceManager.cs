using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
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
        private static IDictionary<string, string> _configurationList;
        private static IDictionary<string, string> _resourceList;
        private static readonly IDictionary<string, Dictionary<string, string>> _resourceLookupList;
        private static readonly ReaderWriterLockSlim ListLock;

        static ResourceManager()
        {
            _configurationList = new Dictionary<string, string>();
            _resourceList = new Dictionary<string, string>();
            _resourceLookupList = new Dictionary<string, Dictionary<string, string>>();
            ListLock = new ReaderWriterLockSlim();
        }

        public ResourceManager()
        {
            bool isEmpty;

            ListLock.EnterReadLock();
            isEmpty = _resourceLookupList.Count <= 0;
            ListLock.ExitReadLock();
            if (isEmpty)  LoadResourceLookupFromDb(new ResourceLookupRepositoryDB(new KatushaDbContext()));
            
            ListLock.EnterReadLock();
            isEmpty = _resourceList.Count <= 0;
            ListLock.ExitReadLock();
            if (isEmpty) LoadResourceFromDb(new ResourceRepositoryDB(new KatushaDbContext()));
            
            ListLock.EnterReadLock();
            isEmpty = _configurationList.Count <= 0;
            ListLock.ExitReadLock();
            if (isEmpty) LoadConfigurationDataFromDb(new ConfigurationDataRepositoryDB(new KatushaDbContext()));
        }

        public void LoadResourceFromDb(IResourceRepository resourceRepository)
        {
            ListLock.EnterWriteLock();
            try {
                _resourceList.Clear();
                foreach (var item in resourceRepository.GetActiveValues()) {
                    try {
                        _resourceList.Add(item.Key, item.Value);
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
                _configurationList.Clear();
                foreach (var item in resourceRepository.GetActiveValues()) {
                    try {
                        _configurationList.Add(item.Key, item.Value);
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
                _resourceLookupList.Clear();
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
                                _resourceLookupList.Add(lookupName+language, list.ToDictionary(r => r.Key, r => r.Value));
                            } catch (Exception ex) {
                                throw new KatushaResourceLookupException(lookupName, ex);
                            }
                            lookupName = item.LookupName;
                            language = item.Language;
                            list = new List<LookupItem>();
                            list.Add(new LookupItem {Key = item.ResourceKey, Value = item.Value, Order = item.Order});
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
                return _configurationList.ContainsKey(key) ? _configurationList[key] : String.Format("{0} Code not found", key);
            } finally {
                ListLock.ExitReadLock();
            }
        }

        public string _C(string propertyName, string key, bool mustFind = false)
        {
            ListLock.EnterReadLock();
            try {
                if (_configurationList.ContainsKey(propertyName + "." + key)) return _configurationList[propertyName + "." + key];
                if (_configurationList.ContainsKey(key)) return _configurationList[key];
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
                return _resourceList.ContainsKey(key) ? _resourceList[key] : String.Format("{0} Code not found", key);
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
                if (_resourceList.ContainsKey(name)) return _resourceList[name];
                name = String.Format("{0}{1}", key, language);
                if (_resourceList.ContainsKey(name)) return _resourceList[name];
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
                    if (_resourceLookupList.ContainsKey(key)) 
                        resourceValue = _resourceLookupList[key];
                } finally {
                    ListLock.ExitReadLock();
                }
            }

            return resourceValue;
        }

        public string _LText(string resourceName, string name, byte language = 0) {
            language = GetLanguage(language);
            string key = String.Format("{0}{1}", resourceName, language);
            if (!string.IsNullOrEmpty(key)) {
                ListLock.EnterReadLock();
                try {
                    if (_resourceLookupList.ContainsKey(key)) {
                        var resourceValue = _resourceLookupList[key];
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
    }
}
