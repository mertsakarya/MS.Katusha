using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;


namespace MS.Katusha.Repositories.RavenDB
{
    public class BoyRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Boy>, IBoyRepositoryRavenDB
    {
        public BoyRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }
    }

}
