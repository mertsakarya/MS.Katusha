using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;


namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class BoyRepository : BaseFriendlyNameRepository<Boy>, IBoyRepository
    {
        public BoyRepository(string ravenDbConnectionString) : base(ravenDbConnectionString) { }
    }

}
