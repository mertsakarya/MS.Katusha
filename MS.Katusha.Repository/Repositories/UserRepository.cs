using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repository.Interfaces;

namespace MS.Katusha.Repository.Repositories
{
    public class UserRepository : BaseGuidRepository<User>, IUserRepository
    {
        public UserRepository(KatushaContext context) : base(context) { }
    }
}