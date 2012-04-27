using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface ILanguagesSpokenRepositoryDB : IRepository<LanguagesSpoken>
    {
        void DeleteByProfileId(long profileId, string language);
        void AddByProfileId(long profileId, string language);
    }

}
