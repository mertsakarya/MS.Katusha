using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repository.Interfaces;

namespace MS.Katusha.Repository.Repositories
{
    public class BoyRepository : BaseFriendlyNameRepository<Boy>, IBoyRepository
    {
        public BoyRepository(KatushaContext context) : base(context) { }
    }

}
