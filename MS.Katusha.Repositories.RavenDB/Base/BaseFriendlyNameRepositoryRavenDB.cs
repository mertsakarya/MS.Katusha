using System;
using System.Linq;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Interfaces.Repositories;
using Raven.Client;

namespace MS.Katusha.Repositories.RavenDB.Base
{
    public abstract class BaseFriendlyNameRepositoryRavenDB<T> : BaseGuidRepositoryRavenDB<T>, IRavenFriendlyNameRepository<T> where T : BaseFriendlyModel
    {

        protected BaseFriendlyNameRepositoryRavenDB(IKatushaRavenStore documentStore)
            : base(documentStore)
        { }

        public long GetProfileIdByFriendlyName(string friendlyName) { return Single(p => p.FriendlyName == friendlyName).Id; }

        public long GetProfileIdByGuid(Guid guid) { return Single(p => p.Guid == guid).Id; }

        public bool CheckIfFriendlyNameExists(string friendlyName, long id = 0)
        {
            if(id <= 0)
                return Query(p => p.FriendlyName == friendlyName, null, false).Any();
            return Query(p => p.FriendlyName == friendlyName && p.Id != id, null, false).Any();
        }
    }
}