using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class SearchingForRepository : BaseRepository<SearchingFor>, ISearchingForRepository
    {
        public SearchingForRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}