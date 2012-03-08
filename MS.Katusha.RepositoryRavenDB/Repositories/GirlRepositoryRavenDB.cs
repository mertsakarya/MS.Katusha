using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class GirlRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Girl>, IGirlRepositoryRavenDB
    {
        public GirlRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }
    }
}