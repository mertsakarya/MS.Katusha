using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;


namespace MS.Katusha.Repositories.RavenDB
{
    public class ProfileRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Profile>, IProfileRepositoryRavenDB
    {
        public ProfileRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }
    }

}
