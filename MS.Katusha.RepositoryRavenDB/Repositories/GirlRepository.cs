using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class GirlRepository : BaseFriendlyNameRepository<Girl>, IGirlRepository
    {
        public GirlRepository(KatushaContext context) : base(context) { }
    }
}