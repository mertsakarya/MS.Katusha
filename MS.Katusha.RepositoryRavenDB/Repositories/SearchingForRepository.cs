using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class SearchingForRepository : BaseRepository<SearchingFor>, ISearchingForRepository
    {
        public SearchingForRepository(KatushaContext context) : base(context) { }
    }
}