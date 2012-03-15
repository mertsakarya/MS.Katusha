using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MS.Katusha.Domain;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public interface IResourceManager
	{
		string GetValueFromCode(string resourceCode, byte language);
		void LoadResourceList();
		//List<string> GetValuesFromCodeList(List<string> resourceCodeList);
	}
	
    public class ResourceManager : IResourceManager
	{
		static IDictionary<string, string> _resourceList;
		static readonly ReaderWriterLockSlim ResourceListLock;
        readonly IResourceRepository _resourceRepository;

		static ResourceManager()
		{
			_resourceList = new Dictionary<string, string>();
			ResourceListLock = new ReaderWriterLockSlim();
		}

        public ResourceManager() : this(new ResourceRepositoryDB(new KatushaDbContext()))
        {
        }

		public ResourceManager(IResourceRepository resourceRepository)
		{
			_resourceRepository = resourceRepository;

			LoadResourceList();
		}

		public void LoadResourceList()
		{
			bool isListEmpty = false;
			ResourceListLock.EnterReadLock();
			isListEmpty = _resourceList.Count <= 0;
			ResourceListLock.ExitReadLock();

			if (isListEmpty)
			{
				LoadResourceFromDb();
			}
		}

		public void LoadResourceFromDb()
		{
			ResourceListLock.EnterWriteLock();

			try
			{
				_resourceList.Clear();

				_resourceList = _resourceRepository
									.GetActiveResources()
									.ToDictionary(r => r.Key, r => r.Value);
			}
			finally
			{
				ResourceListLock.ExitWriteLock();
			}
		}

		public string GetValueFromCode(string resourceCode, byte language)
		{
			string resourceValue = string.Empty;
		    string key = resourceCode + language.ToString();
			if (!string.IsNullOrEmpty(key))
			{
				ResourceListLock.EnterReadLock();

				try
				{
					if (_resourceList.ContainsKey(key))
					{
						resourceValue = _resourceList[key];
					}
					else
					{
						resourceValue = string.Format("{0} kodu veritabanında bulunamadı", key);
					}
				}
				finally
				{
					ResourceListLock.ExitReadLock();
				}
			}

			return resourceValue;
		}

        //public List<string> GetValuesFromCodeList(List<string> resourceCodeList)
        //{
        //    List<string> resourceValueList = null;

        //    if (resourceCodeList != null)
        //    {
        //        ResourceListLock.EnterReadLock();

        //        try
        //        {
        //            resourceValueList = 
        //                _resourceList
        //                    .Where(r => resourceCodeList.Contains(r.Key))
        //                    .Select(r => r.Value)
        //                    .ToList();

        //            if(resourceValueList.Count != resourceCodeList.Count)
        //            {
        //                resourceValueList
        //                    .ToList()
        //                    .AddRange(
        //                        resourceCodeList
        //                            .Where(r => !_resourceList.Keys.Contains(r))
        //                            .Select(r => string.Format("{0} kodu veritabanında bulunamadı", r))
        //                            .ToList()
        //                    );
        //            }
        //        }
        //        finally
        //        {
        //            ResourceListLock.ExitReadLock();
        //        }
        //    }

        //    return resourceValueList;
        //}
	}
}
