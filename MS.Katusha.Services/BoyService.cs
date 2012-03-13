using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{

    public class BoyService : ProfileService<Boy>, IBoyService
    {
        public BoyService(IBoyRepositoryDB repository) : base(repository)
        {
        }
    }
}
