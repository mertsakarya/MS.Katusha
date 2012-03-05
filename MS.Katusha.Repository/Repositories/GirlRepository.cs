using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Repository.Repositories
{
    public class GirlRepository : BaseFriendlyNameRepository<Girl>
    {
        public GirlRepository(KatushaContext context) : base(context) { }
    }
}