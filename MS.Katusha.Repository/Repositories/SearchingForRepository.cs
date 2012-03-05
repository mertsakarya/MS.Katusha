using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repository.Interfaces;

namespace MS.Katusha.Repository.Repositories
{
    public class SearchingForRepository : BaseRepository<SearchingFor>, ISearchingForRepository
    {
        public SearchingForRepository(KatushaContext context) : base(context) { }
    }
}