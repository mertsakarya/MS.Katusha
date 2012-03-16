using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public interface IResourceManager
    {
        string _R(string resourceName, byte language = 0);
        Dictionary<string, string> _L(string resourceName, byte language = 0);
        //List<string> GetValuesFromCodeList(List<string> resourceCodeList);
    }

    public class ResourceManager : IResourceManager
    {
        private static IDictionary<string, string> _resourceList;
        private static readonly IDictionary<string, Dictionary<string, string>> _resourceLookupList;
        private static readonly ReaderWriterLockSlim ResourceListLock;
        private static readonly ReaderWriterLockSlim ResourceLookupListLock;

        static ResourceManager()
        {
            _resourceList = new Dictionary<string, string>();
            _resourceLookupList = new Dictionary<string, Dictionary<string, string>>();
            ResourceListLock = new ReaderWriterLockSlim();
            ResourceLookupListLock = new ReaderWriterLockSlim();
        }

        public ResourceManager()
        {
            bool isEmpty;
            ResourceLookupListLock.EnterReadLock();
            isEmpty = _resourceLookupList.Count <= 0;
            ResourceLookupListLock.ExitReadLock();
            if (isEmpty)  LoadResourceLookupFromDb(new ResourceLookupRepositoryDB(new KatushaDbContext()));
            ResourceListLock.EnterReadLock();
            isEmpty = _resourceList.Count <= 0;
            ResourceListLock.ExitReadLock();
            if(isEmpty) LoadResourceFromDb(new ResourceRepositoryDB(new KatushaDbContext()));
        }

        public void LoadResourceFromDb(IResourceRepository resourceRepository)
        {
            ResourceListLock.EnterWriteLock();
            try
            {
                _resourceList.Clear();
                _resourceList = resourceRepository.GetActiveResources().ToDictionary(r => r.Key, r => r.Value);
            }
            finally
            {
                ResourceListLock.ExitWriteLock();
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

            ResourceLookupListLock.EnterWriteLock();
            try
            {
                _resourceLookupList.Clear();
                var items = resourceLookupRepository.GetActiveResources();
                if (items.Length > 0)
                {
                    var lookupName = items[0].LookupName;
                    var list = new List<LookupItem>();
                    foreach (var item in items)
                    {
                        if (item.LookupName == lookupName)
                            list.Add(new LookupItem {Key = item.Key, Value = item.Value, Order = item.Order});
                        else {
                            list.Sort(CompareLookupItem);
                            _resourceLookupList.Add(lookupName, list.ToDictionary(r => r.Key, r => r.Value));
                            lookupName = item.LookupName;
                            list = new List<LookupItem>();
                        }
                    }
                }
            }
            finally
            {
                ResourceLookupListLock.ExitWriteLock();
            }
        }

        public string _R(string resourceName, byte language = 0)
        {
            language = GetLanguage(language);
            string resourceValue = string.Empty;
            string key = resourceName + language.ToString();
            if (!string.IsNullOrEmpty(key)) {
                ResourceListLock.EnterReadLock();
                try {
                    resourceValue = _resourceList.ContainsKey(key) ? _resourceList[key] : String.Format("{0} Code not found", key);
                } finally {
                    ResourceListLock.ExitReadLock();
                }
            }

            return resourceValue;
        }

        private static byte GetLanguage(byte language)
        {
            if (language == 0) {
                var cultureName = Thread.CurrentThread.CurrentCulture.Name;
                switch (cultureName.Substring(0, 2).ToLower()) {
                    case "tr":
                        return (byte)Language.Turkish;
                    case "ru":
                        return (byte)Language.Russian;
                    //case "en":
                    default:
                        return (byte)Language.English;
                }
            } 
            return language;
        }

        public Dictionary<string, string> _L(string resourceName, byte language = 0)
        {
            language = GetLanguage(language);
            var resourceValue = new Dictionary<string, string>();
            string key = resourceName + language.ToString();
            if (!string.IsNullOrEmpty(key)) {
                ResourceListLock.EnterReadLock();
                try {
                    if (_resourceLookupList.ContainsKey(key)) 
                        resourceValue = _resourceLookupList[key];
                } finally {
                    ResourceListLock.ExitReadLock();
                }
            }

            return resourceValue;
        }
    }
}
