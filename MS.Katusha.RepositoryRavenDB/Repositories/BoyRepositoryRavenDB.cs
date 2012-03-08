using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;


namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class BoyRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Boy>, IBoyRepositoryRavenDB
    {
        public BoyRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }
    }

}
