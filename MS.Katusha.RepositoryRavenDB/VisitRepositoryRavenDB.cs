using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;

namespace MS.Katusha.Repositories.RavenDB
{
    public class VisitRepositoryRavenDB : BaseGuidRepositoryRavenDB<Visit>, IVisitRepositoryRavenDB
    {

        public VisitRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }
    }
}