using System.Linq;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class CountriesToVisitRepositoryDB : BaseRepositoryDB<CountriesToVisit>, ICountriesToVisitRepositoryDB
    {
        public CountriesToVisitRepositoryDB(KatushaDbContext dbContext) : base(dbContext) { }

    }
}