using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface ILanguagesSpokenRepositoryDB : IRepository<LanguagesSpoken>
    {
        void DeleteByProfileId(long profileId, Language language);
        void AddByProfileId(long profileId, Language language);
    }

}
