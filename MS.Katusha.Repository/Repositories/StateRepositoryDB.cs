using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class StateRepositoryDB : BaseRepositoryDB<State>, IStateRepositoryDB
    {
        public StateRepositoryDB(KatushaDbContext dbContext) : base(dbContext) { }
    }
}