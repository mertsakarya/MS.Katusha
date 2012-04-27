using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface ICountriesToVisitRepositoryDB : IRepository<CountriesToVisit>
    {
        void DeleteByProfileId(long profileId, string country);
        void AddByProfileId(long profileId, string country);
    }
}
