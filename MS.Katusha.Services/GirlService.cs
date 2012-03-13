using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{

    public class GirlService : ProfileService<Girl>, IGirlService
    {

        public GirlService(IGirlRepositoryDB repository)  : base(repository)
        {
        }
    }
}
