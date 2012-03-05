using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repository.Interfaces;

namespace MS.Katusha.Repository.Repositories
{
    public class LanguagesSpokenRepository : BaseRepository<LanguagesSpoken>, ILanguagesSpokenRepository
    {
        public LanguagesSpokenRepository(KatushaContext context) : base(context) { }
    }
}