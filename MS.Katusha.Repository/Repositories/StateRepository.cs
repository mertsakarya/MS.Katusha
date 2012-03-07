using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class StateRepository : BaseRepository<State>, IStateRepository
    {
        public StateRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}