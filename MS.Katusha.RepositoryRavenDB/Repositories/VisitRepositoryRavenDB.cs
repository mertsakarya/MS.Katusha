using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class VisitRepositoryRavenDB : BaseGuidRepositoryRavenDB<Visit>, IVisitRepositoryRavenDB
    {

        public VisitRepositoryRavenDB(string connectionStringName = "KatushaRavenDB")
            : base(connectionStringName)
        {
        }
    }
}