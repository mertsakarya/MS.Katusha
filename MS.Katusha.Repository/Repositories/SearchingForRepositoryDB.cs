using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class SearchingForRepositoryDB : BaseRepositoryDB<SearchingFor>, ISearchingForRepositoryDB
    {
        public SearchingForRepositoryDB(KatushaDbContext dbContext) : base(dbContext) { }
    }
}