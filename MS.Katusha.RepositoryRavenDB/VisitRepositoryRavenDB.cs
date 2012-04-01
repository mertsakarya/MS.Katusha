using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using Raven.Client;

namespace MS.Katusha.Repositories.RavenDB
{
    public class VisitRepositoryRavenDB : BaseRepositoryRavenDB<Visit>, IVisitRepositoryRavenDB
    {

        public VisitRepositoryRavenDB(IDocumentStore documentStore)
            : base(documentStore)
        {
        }
    }
}