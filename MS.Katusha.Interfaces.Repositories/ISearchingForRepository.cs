using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface ISearchingForRepositoryDB : IRepository<SearchingFor>
    {
        void DeleteByProfileId(long profileId, LookingFor lookingFor);
        void AddByProfileId(long profileId, LookingFor lookingFor);
    }

}
