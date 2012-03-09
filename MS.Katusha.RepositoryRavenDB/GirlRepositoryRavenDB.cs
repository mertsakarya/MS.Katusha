using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;

namespace MS.Katusha.Repositories.RavenDB
{
    public class GirlRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Girl>, IGirlRepositoryRavenDB
    {
        public GirlRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }
    }
}